using Microsoft.EntityFrameworkCore;
using Referral_Management.Api.Models;
using Referral_Management.Api.Services.Interfaces;
using Referral_Management.Api.Exceptions;
using Microsoft.AspNetCore.SignalR;
using Referral_Management.Api.Hubs;

namespace Referral_Management.Api.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly AppDbContext _context;
        private readonly IHubContext<NotificationHub> _hub;

        public AppointmentService(AppDbContext context, IHubContext<NotificationHub> hub)
        {
            _context = context;
            _hub = hub;
        }

        public async Task<AvailableSlotsResponseDTO> GetAvailableSlotsAsync(int specialistId, DateOnly date)
        {
            var specialist = await _context.Specialists
                .Include(s => s.ShiftBlock)
                .FirstOrDefaultAsync(s => s.SpecialistId == specialistId);

            if (specialist == null)
                throw new BadRequestException("Specialist not found.");

            var bookedSlots = await _context.Appointments
                .Where(a => a.SpecialistId == specialistId && a.AppointmentDate == date)
                .Select(a => a.AppointmentTime)
                .ToListAsync();

            var availableSlots = new List<AvailableSlotDTO>();

            var current = specialist.ShiftBlock.StartTime;

            while (current < specialist.ShiftBlock.EndTime)
            {
                if (!bookedSlots.Contains(current))
                {
                    availableSlots.Add(new AvailableSlotDTO
                    {
                        StartTime = current,
                        EndTime = current.AddHours(1)
                    });
                }

                current = current.AddHours(1);
            }

            return new AvailableSlotsResponseDTO
            {
                SpecialistId = specialistId,
                Date = date,
                Slots = availableSlots
            };
        }

        public async Task<DailyScheduleResponseDTO> GetScheduleAsync(int specialistId, DateOnly date)
        {
            var appointments = await _context.Appointments
                .Where(a => a.SpecialistId == specialistId && a.AppointmentDate == date)
                .Select(a => new AppointmentScheduleDTO
                {
                    AppointmentId = a.AppointmentId,
                    AppointmentTime = a.AppointmentTime,
                    PatientName = a.Patient.User.FirstName + " " + a.Patient.User.LastName
                })
                .ToListAsync();

            return new DailyScheduleResponseDTO
            {
                Date = date,
                Appointments = appointments
            };
        }

        // ✅ MAIN SIGNALR METHOD
        public async Task<AppointmentResponseDTO> CreateAppointmentAsync(
    CreateAppointmentDTO request,
    int userId)
        {
            var referral = await _context.Referrals
                .FirstOrDefaultAsync(r => r.ReferralId == request.ReferralId);

            if (referral == null)
                throw new BadRequestException("Referral not found.");

            var appointment = new Appointment
            {
                ReferralId = referral.ReferralId,
                PatientId = referral.PatientId,
                SpecialistId = request.SpecialistId,
                AppointmentDate = request.AppointmentDate,
                AppointmentTime = request.AppointmentTime,
                AppointmentStatusId = 1
            };

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            // ✅ ✅ ✅ SIGNALR (SAFE BLOCK)
            try
            {
                Console.WriteLine($"Referral.PatientId = {referral.PatientId}");
                Console.WriteLine($"SpecialistId = {request.SpecialistId}");

                var payload = new
                {
                    appointmentId = appointment.AppointmentId,
                    referralId = referral.ReferralId,
                    patientId = referral.PatientId,
                    specialistId = request.SpecialistId,
                    appointmentDate = appointment.AppointmentDate,
                    appointmentTime = appointment.AppointmentTime,
                    message = "Appointment scheduled successfully"
                };

                Console.WriteLine($"Sending to Patient_{referral.PatientId}");
                Console.WriteLine($"Sending to Specialist_{request.SpecialistId}");

                // ✅ Notify Patient
                await _hub.Clients
                    .Group($"Patient_{referral.PatientId}")
                    .SendAsync("AppointmentScheduled", payload);

                // ✅ Notify Specialist
                await _hub.Clients
                    .Group($"Specialist_{request.SpecialistId}")
                    .SendAsync("AppointmentScheduled", payload);

                Console.WriteLine($"✅ SignalR sent for Appointment {appointment.AppointmentId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ SignalR ERROR: {ex.Message}");
            }

            return new AppointmentResponseDTO
            {
                AppointmentId = appointment.AppointmentId,
                Message = "Appointment scheduled successfully"
            };
        }

        public async Task<AppointmentDetailsDTO> GetAppointmentDetailsAsync(int appointmentId)
        {
            var appointment = await _context.Appointments
                .FirstOrDefaultAsync(a => a.AppointmentId == appointmentId);

            if (appointment == null)
                throw new BadRequestException("Appointment not found.");

            return new AppointmentDetailsDTO
            {
                AppointmentId = appointment.AppointmentId,
                ReferralId = appointment.ReferralId
            };
        }

        public async Task<List<UserAppointmentDTO>> GetUserAppointmentsAsync(int userId)
        {
            return await _context.Appointments
                .Select(a => new UserAppointmentDTO
                {
                    AppointmentId = a.AppointmentId
                })
                .ToListAsync();
        }

        public async Task<AppointmentStatusResponseDTO> UpdateAppointmentStatusAsync(UpdateAppointmentStatusDTO request)
        {
            var appointment = await _context.Appointments
                .Include(a => a.Referral)
                .FirstOrDefaultAsync(a => a.AppointmentId == request.AppointmentId);

            if (appointment == null)
                throw new BadRequestException("Appointment not found.");

            appointment.AppointmentStatusId = request.AppointmentStatusId;
            await _context.SaveChangesAsync();

            string statusText = request.AppointmentStatusId switch
            {
                1 => "Scheduled",
                2 => "Completed",
                3 => "Cancelled",
                _ => "Updated"
            };

            try
            {
                var groupName = $"Patient_{appointment.Referral.PatientId}";

                Console.WriteLine($"🔥 Sending to {groupName}");

                await _hub.Clients
                    .Group(groupName)
                    .SendAsync("AppointmentStatusUpdated", new
                    {
                        appointmentId = appointment.AppointmentId,
                        referralId = appointment.ReferralId,
                        status = statusText,
                        message = "Appointment status updated"
                    });

                Console.WriteLine($"✅ AppointmentStatusUpdated sent");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ SignalR ERROR: {ex.Message}");
            }

            return new AppointmentStatusResponseDTO
            {
                AppointmentId = appointment.AppointmentId,
                Status = statusText,
                Message = "Updated successfully"
            };
        }
    }
}