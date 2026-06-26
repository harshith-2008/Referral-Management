using Microsoft.EntityFrameworkCore;
using Referral_Management.Api.DTOs;
using Referral_Management.Api.Exceptions;
using Referral_Management.Api.Models;
using Referral_Management.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using Referral_Management.Api.Hubs;

namespace Referral_Management.Api.Services;

public class SpecialistService : ISpecialistService
{
    private readonly AppDbContext _context;
    private readonly IHubContext<NotificationHub> _hub;

    public SpecialistService(AppDbContext context, IHubContext<NotificationHub> hub)
    {
        _context = context;
        _hub = hub;
    }

    public async Task<List<SpecialistPatientDto>> GetAssignedPatients(int specialistId)
    {
        var referrals = await _context.ReferralAssignments
            .AsNoTracking()
            .Where(ra => ra.ToSpecialistId == specialistId)
            .Include(ra => ra.Referral)
                .ThenInclude(r => r.Patient)
                    .ThenInclude(p => p.User)
            .Include(ra => ra.Referral)
                .ThenInclude(r => r.SpecialtyRequest)
            .Include(ra => ra.Referral)
                .ThenInclude(r => r.UrgencyLevel)
            .Include(ra => ra.Referral)
                .ThenInclude(r => r.Appointments)
            .Include(ra => ra.Referral)
                .ThenInclude(r => r.ReferralStatus)
            .Where(ra => ra.Referral.ReferralStatus.StatusName == "Scheduled")
            .ToListAsync();

        var result = new List<SpecialistPatientDto>();

        foreach (var ra in referrals)
        {
            var patient = ra.Referral.Patient;

            // ✅ Age calculation (DateOnly → DateTime)
            var dob = patient.Dob.ToDateTime(TimeOnly.MinValue);
            var today = DateTime.Today;

            var age = today.Year - dob.Year;
            if (dob > today.AddYears(-age))
                age--;

            // ✅ AppointmentDate FIX (DateOnly → DateTime?)
            DateTime? appointmentDate = ra.Referral.Appointments
                .OrderBy(a => a.AppointmentDate)
                .Select(a => a.AppointmentDate.ToDateTime(TimeOnly.MinValue))
                .Cast<DateTime?>()
                .FirstOrDefault();

            result.Add(new SpecialistPatientDto
            {
                ReferralId = ra.Referral.ReferralId,

                PatientId = patient.PatientId,
                PatientName =
                    patient.User.FirstName + " " +
                    patient.User.LastName,
                Age = age,
                Gender = patient.Gender,
                Mrn = patient.Mrn,

                Specialty = ra.Referral.SpecialtyRequest.SpecialtyName,
                Urgency = ra.Referral.UrgencyLevel.LevelName,
                DiagnosisCode = ra.Referral.DiagnosisCode,

                AppointmentDate = appointmentDate
            });
        }

        return result;
    }

    public async Task<int> CreateDraftReferral(
    int specialistId,
    ReferralIntakeCreateDto dto)
    {
        try
        {
            Console.WriteLine("👉 Creating referral...");

            var patient = await _context.Patients
                .FirstOrDefaultAsync(p => p.PatientId == dto.PatientId);

            if (patient == null)
                throw new NotFoundException("Patient not found");

            var requestedStatusId = await _context.ReferralStatuses
                .Where(rs => rs.StatusName == "Submitted")
                .Select(rs => rs.ReferralStatusId)
                .FirstOrDefaultAsync();

            if (requestedStatusId == 0)
                throw new Exception("Submitted status not found");

            var referral = new Referral
            {
                PatientId = dto.PatientId,
                FromSpecialistId = specialistId,
                ReferralReason = dto.ReferralReason,
                DiagnosisCode = dto.DiagnosisCode,
                UrgencyLevelId = dto.UrgencyLevelId,
                SpecialtyRequestId = dto.SpecialtyRequestId,
                OriginFacilityId = patient.PrimaryFacilityId,
                ReferralStatusId = requestedStatusId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Referrals.Add(referral);
            await _context.SaveChangesAsync();

            Console.WriteLine($"✅ Referral created: {referral.ReferralId}");

            // ✅ SAFE SIGNALR BLOCK (CRITICAL FIX)
            var groupName = $"Facility_{referral.OriginFacilityId}_Coordinators";

            try
            {
                if (!string.IsNullOrEmpty(groupName))
                {
                    await _hub.Clients
                        .Group(groupName)
                        .SendAsync("ReferralCreated", new
                        {
                            referralId = referral.ReferralId,
                            message = "New referral created by specialist"
                        });

                    Console.WriteLine($"✅ SignalR sent to {groupName}");
                }
                else
                {
                    Console.WriteLine("❌ Group name is null");
                }
            }
            catch (Exception ex)
            {
                // ✅ DO NOT THROW → prevents connection crash
                Console.WriteLine($"❌ SignalR ERROR: {ex.Message}");
            }

            return referral.ReferralId;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ CreateReferral ERROR: {ex.Message}");
            throw;
        }
    }


    public async Task<IEnumerable<GetUrgencyLevelsDto>> GetAllUrgenciesAsync()
    {
        return await _context.UrgencyLevels
            .Select(u => new GetUrgencyLevelsDto
            {
                UrgencyLevelId = u.UrgencyLevelId,
                LevelName = u.LevelName
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<GetSpecialityDto>> GetAllSpecialitiesAsync()
    {
        return await _context.Specialties
            .Select(s => new GetSpecialityDto
            {
                SpecialityId = s.SpecialtyId,
                SpecialityName = s.SpecialtyName
            })
            .ToListAsync();
    }

}