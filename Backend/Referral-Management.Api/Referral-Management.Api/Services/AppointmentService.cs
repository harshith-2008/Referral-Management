using Microsoft.EntityFrameworkCore;
using Referral_Management.Api.Models;
using Referral_Management.Api.Services.Interfaces;
using Referral_Management.Api.Exceptions;

namespace Referral_Management.Api.Services
{
    public class AppointmentService : IAppointmentService
    {

        private readonly AppDbContext _context;

        public AppointmentService(AppDbContext context)
        {
            _context = context;
        }


        public async Task<AvailableSlotsResponseDTO> GetAvailableSlotsAsync(int specialistId, DateOnly date)
        {
            var today = DateOnly.FromDateTime(DateTime.Now);
            var nowTime = TimeOnly.FromDateTime(DateTime.Now);

            if (date < today)
            {
                throw new BadRequestException(
                    "Cannot retrieve slots for past dates.");

            }

            var specialist = await _context.Specialists
                .AsNoTracking()
                .Include(s => s.ShiftBlock)
                .FirstOrDefaultAsync(s => s.SpecialistId == specialistId);

            if (specialist == null)
            {
                throw new BadRequestException("Specialist not found.");
            }

            if (!specialist.Status)
            {
                throw new BadRequestException(
                    "Specialist is inactive.");
            }

            if (specialist.ShiftBlock == null)
            {
                throw new BadRequestException("Shift block not assigned.");
            }

            var bookedSlots = (await _context.Appointments
                .AsNoTracking()
                .Where(a =>
                    a.SpecialistId == specialistId &&
                    a.AppointmentDate == date)
                .Select(a => a.AppointmentTime)
                .ToListAsync())
                .ToHashSet();

            var availableSlots = new List<AvailableSlotDTO>();

            var currentSlot = specialist.ShiftBlock.StartTime;

            while (currentSlot < specialist.ShiftBlock.EndTime)
            {
                var isPastSlotToday = date == today && currentSlot <= nowTime;

                if (!isPastSlotToday && !bookedSlots.Contains(currentSlot))
                {
                    availableSlots.Add(new AvailableSlotDTO
                    {
                        StartTime = currentSlot,
                        EndTime = currentSlot.AddHours(1)

                    });
                }

                currentSlot = currentSlot.AddHours(1);
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
            var today = DateOnly.FromDateTime(DateTime.Now);
            var nowTime = TimeOnly.FromDateTime(DateTime.Now);

            if (date < today)
                throw new BadRequestException("Cannot retrieve schedule for past dates.");

            var appointments = await _context.Appointments
                .AsNoTracking()
                .Where(a =>
                    a.SpecialistId == specialistId &&
                    a.AppointmentDate == date &&
                    (a.AppointmentDate > today || a.AppointmentTime > nowTime))
                .OrderBy(a => a.AppointmentTime)
                .Select(a => new AppointmentScheduleDTO
                {
                    AppointmentId = a.AppointmentId,

                    AppointmentTime = a.AppointmentTime,

                    PatientName =
                        a.Patient.User.FirstName + " " +
                        a.Patient.User.LastName,

                    Mrn = a.Patient.Mrn,

                    AppointmentStatus =
                        a.AppointmentStatus.StatusName
                })
                .ToListAsync();

            return new DailyScheduleResponseDTO
            {
                Date = date,
                Appointments = appointments
            };
        }
        public async Task<AppointmentResponseDTO> CreateAppointmentAsync(
            CreateAppointmentDTO request,
            int coordinatorId)
        {
            var today = DateOnly.FromDateTime(DateTime.Now);
            var nowTime = TimeOnly.FromDateTime(DateTime.Now);

            if (request.AppointmentDate < today ||
                (request.AppointmentDate == today && request.AppointmentTime <= nowTime))
                throw new BadRequestException("Cannot schedule appointment in the past.");

            var referral = await _context.Referrals
                .FirstOrDefaultAsync(r => r.ReferralId == request.ReferralId);

            if (referral == null)
                throw new BadRequestException("Referral not found.");

            var specialist = await _context.Specialists
                .Include(s => s.ShiftBlock)
                .FirstOrDefaultAsync(s => s.SpecialistId == request.SpecialistId);

            if (specialist == null)
                throw new BadRequestException("Specialist not found.");

            if (!specialist.Status)
                throw new BadRequestException("Specialist is inactive.");

            if (specialist.ShiftBlock == null)
                throw new BadRequestException("Shift block not assigned.");

            if (request.AppointmentTime < specialist.ShiftBlock.StartTime ||
                request.AppointmentTime >= specialist.ShiftBlock.EndTime)
            {
                throw new BadRequestException("Invalid appointment slot.");
            }

            if (request.AppointmentTime.Minute != 0)
                throw new BadRequestException("Appointments must start on the hour.");

            bool slotBooked = await _context.Appointments.AnyAsync(a =>
                a.SpecialistId == request.SpecialistId &&
                a.AppointmentDate == request.AppointmentDate &&
                a.AppointmentTime == request.AppointmentTime);

            if (slotBooked)
                throw new BadRequestException("Selected slot is already booked.");

            var acceptedReferralStatusId = await _context.ReferralStatuses
                .Where(s => s.StatusName == "Accepted")
                .Select(s => s.ReferralStatusId)
                .FirstOrDefaultAsync();

            if (acceptedReferralStatusId == 0)
                throw new BadRequestException("Referral status 'Accepted' not found.");

            var scheduledAppointmentStatusId = await _context.AppointmentStatuses
                .Where(s => s.StatusName == "Scheduled")
                .Select(s => s.AppointmentStatusId)
                .FirstOrDefaultAsync();

            if (scheduledAppointmentStatusId == 0)
                throw new BadRequestException("Appointment status 'Scheduled' not found.");

            var appointment = new Appointment
            {
                ReferralId = referral.ReferralId,
                PatientId = referral.PatientId,
                SpecialistId = request.SpecialistId,
                AppointmentDate = request.AppointmentDate,
                AppointmentTime = request.AppointmentTime,
                AppointmentStatusId = scheduledAppointmentStatusId
            };

            _context.Appointments.Add(appointment);

            var assignment = new ReferralAssignment
            {
                ReferralId = referral.ReferralId,
                AssignedBy = coordinatorId,
                ToSpecialistId = request.SpecialistId,
                FromSpecialistId = referral.FromSpecialistId,
                Reason = "Initial assignment",
                ReassignedAt = DateTime.UtcNow
            };

            _context.ReferralAssignments.Add(assignment);

            referral.ReferralStatusId = acceptedReferralStatusId;

            await _context.SaveChangesAsync();

            return new AppointmentResponseDTO
            {
                AppointmentId = appointment.AppointmentId,
                Message = "Appointment scheduled successfully."
            };
        }
        public async Task<AppointmentDetailsDTO> GetAppointmentDetailsAsync(int appointmentId)
        {
            var today = DateOnly.FromDateTime(DateTime.Now);
            var nowTime = TimeOnly.FromDateTime(DateTime.Now);

            var appointment = await _context.Appointments
                .AsNoTracking()
                .Where(a =>
                    a.AppointmentId == appointmentId &&
                    (a.AppointmentDate > today ||
                     (a.AppointmentDate == today && a.AppointmentTime > nowTime)))
                .Select(a => new AppointmentDetailsDTO
                {
                    AppointmentId = a.AppointmentId,
                    ReferralId = a.ReferralId,
                    AppointmentDate = a.AppointmentDate,
                    AppointmentTime = a.AppointmentTime,
                    AppointmentStatus = a.AppointmentStatus.StatusName,
                    PatientId = a.PatientId,
                    PatientName = a.Patient.User.FirstName + " " + a.Patient.User.LastName,
                    Mrn = a.Patient.Mrn,
                    SpecialistId = a.SpecialistId,
                    SpecialistName = a.Specialist.User.FirstName + " " + a.Specialist.User.LastName,
                    ReferralReason = a.Referral.ReferralReason
                })
                .FirstOrDefaultAsync();

            if (appointment == null)
            {
                throw new BadRequestException("Appointment not found.");
            }

            return appointment;
        }

        public async Task<List<UserAppointmentDTO>> GetUserAppointmentsAsync(int userId)
        {
            var today = DateOnly.FromDateTime(DateTime.Now);
            var nowTime = TimeOnly.FromDateTime(DateTime.Now);

            var patient = await _context.Patients
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if(patient == null)
            {
                throw new BadRequestException("Patient not found.");
            }

            return await _context.Appointments
                .AsNoTracking()
                .Where(a =>
                    a.PatientId == patient.PatientId &&
                    (a.AppointmentDate > today ||
                     (a.AppointmentDate == today && a.AppointmentTime > nowTime)))
                .OrderBy(a => a.AppointmentStatusId)
                .ThenBy(a => a.AppointmentDate)
                .ThenBy(a => a.AppointmentTime)
                .Select(a => new UserAppointmentDTO
                {
                    AppointmentId = a.AppointmentId,
                    ReferralId = a.ReferralId,
                    AppointmentDate = a.AppointmentDate,
                    AppointmentTime = a.AppointmentTime,
                    SpecialistName =
                        a.Specialist.User.FirstName + " " +
                        a.Specialist.User.LastName,
                    AppointmentStatus =
                        a.AppointmentStatus.StatusName
                })
                .ToListAsync();
        }

        public async Task<AppointmentStatusResponseDTO>UpdateAppointmentStatusAsync(UpdateAppointmentStatusDTO request)
        {
            var appointment = await _context.Appointments
                .Include(a => a.AppointmentStatus)
                .FirstOrDefaultAsync(a =>
                    a.AppointmentId == request.AppointmentId);

            if (appointment == null)
            {
                throw new BadRequestException("Appointment not found.");
            }

            if (appointment.AppointmentStatusId == 2)
            {
                throw new BadRequestException("Cancelled appointments cannot be modified.");
            }

            if (appointment.AppointmentStatusId == 3)
            {
                throw new BadRequestException("Completed appointments cannot be modified.");
            }

            bool statusExists = await _context.AppointmentStatuses
                .AnyAsync(s =>
                    s.AppointmentStatusId ==
                    request.AppointmentStatusId);

            if (!statusExists)
            {
                throw new BadRequestException("Invalid appointment status.");
            }

            appointment.AppointmentStatusId = request.AppointmentStatusId;

            await _context.SaveChangesAsync();

            var statusName = await _context.AppointmentStatuses
                .Where(s =>
                    s.AppointmentStatusId ==
                    request.AppointmentStatusId)
                .Select(s => s.StatusName)
                .FirstAsync();

            return new AppointmentStatusResponseDTO
            {
                AppointmentId = appointment.AppointmentId,
                Status = statusName,
                Message = "Appointment status updated successfully."
            };
        }

        public async Task<AppointmentStatusResponseDTO> MarkAppointmentCompletedAsync(
            int appointmentId,
            int specialistId)
        {
            var appointment = await _context.Appointments
                .Include(a => a.AppointmentStatus)
                .Include(a => a.Referral)
                .FirstOrDefaultAsync(a => a.AppointmentId == appointmentId);

            if (appointment == null)
                throw new BadRequestException("Appointment not found.");

            if (appointment.SpecialistId != specialistId)
                throw new BadRequestException("You can complete only appointments assigned to you.");

            if (appointment.AppointmentStatus.StatusName == "Cancelled")
                throw new BadRequestException("Cancelled appointments cannot be completed.");

            if (appointment.AppointmentStatus.StatusName == "Completed")
                throw new BadRequestException("Appointment is already completed.");

            var completedAppointmentStatusId = await _context.AppointmentStatuses
                .Where(s => s.StatusName == "Completed")
                .Select(s => s.AppointmentStatusId)
                .FirstOrDefaultAsync();

            if (completedAppointmentStatusId == 0)
                throw new BadRequestException("Appointment status 'Completed' not found.");

            var completedReferralStatusId = await _context.ReferralStatuses
                .Where(s => s.StatusName == "Completed")
                .Select(s => s.ReferralStatusId)
                .FirstOrDefaultAsync();

            if (completedReferralStatusId == 0)
                throw new BadRequestException("Referral status 'Completed' not found.");

            appointment.AppointmentStatusId = completedAppointmentStatusId;
            appointment.Referral.ReferralStatusId = completedReferralStatusId;
            appointment.Referral.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new AppointmentStatusResponseDTO
            {
                AppointmentId = appointment.AppointmentId,
                Status = "Completed",
                Message = "Appointment and referral marked as completed."
            };
        }
    }

}
