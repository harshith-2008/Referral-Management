using Microsoft.EntityFrameworkCore;
using Referral_Management.Api.DTOs;
using Referral_Management.Api.Exceptions;
using Referral_Management.Api.Models;
using Referral_Management.Api.Services.Interfaces;

namespace Referral_Management.Api.Services;

public class SpecialistService : ISpecialistService
{
    private readonly AppDbContext _context;

    public SpecialistService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<SpecialistPatientDto>> GetAssignedPatients(int specialistId)
    {
        var todayDate = DateOnly.FromDateTime(DateTime.Now);
        var nowTime = TimeOnly.FromDateTime(DateTime.Now);

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
            .Where(ra => ra.Referral.ReferralStatus.StatusName == "Accepted")
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
            var upcomingAppointment = ra.Referral.Appointments
                .Where(a =>
                    a.AppointmentDate > todayDate ||
                    (a.AppointmentDate == todayDate && a.AppointmentTime > nowTime))
                .OrderBy(a => a.AppointmentDate)
                .ThenBy(a => a.AppointmentTime)
                .FirstOrDefault();

            if (upcomingAppointment == null)
                continue;

            DateTime? appointmentDate = upcomingAppointment.AppointmentDate
                .ToDateTime(TimeOnly.MinValue);

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
        var specialist = await _context.Specialists
            .Include(s => s.Facility)
            .FirstOrDefaultAsync(s => s.SpecialistId == specialistId);

        if (specialist == null)
            throw new NotFoundException("Specialist not found");

        var patient = await _context.Patients
            .Include(p => p.PrimaryFacility)
            .FirstOrDefaultAsync(p => p.PatientId == dto.PatientId);

        if (patient == null)
            throw new NotFoundException("Patient not found");

        if (patient.PrimaryFacility.HospitalId != specialist.Facility.HospitalId)
            throw new BadRequestException("You can create referrals only for patients in your hospital.");

        var requestedStatusId = await _context.ReferralStatuses
            .Where(rs => rs.StatusName == "Submitted")
            .Select(rs => rs.ReferralStatusId)
            .FirstAsync();

        var referral = new Referral
        {
            PatientId = dto.PatientId,
            FromSpecialistId = specialistId,
            ReferralReason = dto.ReferralReason,
            DiagnosisCode = dto.DiagnosisCode,
            UrgencyLevelId = dto.UrgencyLevelId,
            SpecialtyRequestId = dto.SpecialtyRequestId,

            // Derived from patient
            OriginFacilityId = patient.PrimaryFacilityId,

            // Status
            ReferralStatusId = requestedStatusId,

            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

            _context.Referrals.Add(referral);
            await _context.SaveChangesAsync();

            return referral.ReferralId;
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
