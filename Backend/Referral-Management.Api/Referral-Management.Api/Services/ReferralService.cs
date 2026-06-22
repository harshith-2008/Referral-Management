using Microsoft.EntityFrameworkCore;
using Referral_Management.Api.DTOs;
using Referral_Management.Api.Models;
using Referral_Management.Api.Services.Interfaces;

namespace Referral_Management.Api.Services;

public class ReferralService : IReferralService
{
    private readonly AppDbContext _context;

    public ReferralService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<ReferralDto>> GetRequestedReferralsForCoordinator(int referralCoordinatorId)
    {
        var coordinator = await _context.ReferralCoordinators
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.ReferralCoordinatorId == referralCoordinatorId);

        if (coordinator == null)
            return new List<ReferralDto>();

        var facilityId = coordinator.FacilityId;

        var referrals = await _context.Referrals
            .AsNoTracking()
            .Include(r => r.Patient)
                .ThenInclude(p => p.User)
            .Include(r => r.OriginFacility)
            .Include(r => r.DestinationFacility)
            .Include(r => r.ReferralStatus)
            .Include(r => r.SpecialtyRequest)
            .Include(r => r.UrgencyLevel)
            .Where(r =>
                r.DestinationFacilityId == facilityId &&
                r.ReferralStatus.StatusName == "Requested"
            )
            .Select(r => new ReferralDto
            {
                ReferralId = r.ReferralId,
                PatientName =
                    r.Patient.User.FirstName + " " +
                    r.Patient.User.LastName,
                OriginFacility = r.OriginFacility.FacilityName,
                DestinationFacility = r.DestinationFacility.FacilityName,
                Status = r.ReferralStatus.StatusName,
                Urgency = r.UrgencyLevel.LevelName,
                Specialty = r.SpecialtyRequest.SpecialtyName,
                DiagnosisCode = r.DiagnosisCode,
                CreatedAt = r.CreatedAt
            })
            .ToListAsync();

        return referrals;
    }

    public async Task<ReferralDetailDto?> GetReferralDetailsById(int referralId)
    {
        var referral = await _context.Referrals
            .AsNoTracking()
            .Include(r => r.Patient)
                .ThenInclude(p => p.User)
            .Include(r => r.OriginFacility)
            .Include(r => r.DestinationFacility)
            .Include(r => r.ReferralStatus)
            .Include(r => r.UrgencyLevel)
            .Include(r => r.SpecialtyRequest)
            .FirstOrDefaultAsync(r => r.ReferralId == referralId);

        if (referral == null)
            return null;

        // ✅ Age calculation (DateOnly safe)
        var dob = referral.Patient.Dob.ToDateTime(TimeOnly.MinValue);
        var today = DateTime.Today;
        var age = today.Year - dob.Year;
        if (dob > today.AddYears(-age))
            age--;

        // ✅ BUSINESS RULE:
        // Before Accepted → Origin Facility
        // Accepted and after → Destination Facility
        var primaryFacilityName =
            referral.ReferralStatus.StatusName == "Accepted"
            || referral.ReferralStatus.StatusName == "Scheduled"
            || referral.ReferralStatus.StatusName == "Completed"
                ? referral.DestinationFacility.FacilityName
                : referral.OriginFacility.FacilityName;

        return new ReferralDetailDto
        {
            // 🔹 Referral
            ReferralId = referral.ReferralId,
            Status = referral.ReferralStatus.StatusName,
            Urgency = referral.UrgencyLevel.LevelName,
            Specialty = referral.SpecialtyRequest.SpecialtyName,
            DiagnosisCode = referral.DiagnosisCode,
            CreatedAt = referral.CreatedAt,

            // 🔹 Patient
            PatientName =
                referral.Patient.User.FirstName + " " +
                referral.Patient.User.LastName,
            Age = age,
            Gender = referral.Patient.Gender,
            Mrn = referral.Patient.Mrn,

            // ✅ Dynamic primary facility
            PrimaryFacility = primaryFacilityName
        };
    }
    public async Task<List<SpecialistMatchDto>> GetMatchingSpecialistsForReferral(int referralId)
    {
        // ✅ Get referral with required data
        var referral = await _context.Referrals
            .AsNoTracking()
            .Include(r => r.SpecialtyRequest)
            .Include(r => r.DestinationFacility)
            .FirstOrDefaultAsync(r => r.ReferralId == referralId);

        if (referral == null)
            return new List<SpecialistMatchDto>();

        var facilityId = referral.DestinationFacilityId;
        var requestedSpecialtyId = referral.SpecialtyRequestId;

        // ✅ Get specialists in facility with specialties
        var specialists = await _context.Specialists
            .AsNoTracking()
            .Include(s => s.User)
            .Include(s => s.Facility)
            .Include(s => s.ShiftBlock)
            .Include(s => s.SpecialistSpecialities)
                .ThenInclude(ss => ss.Specialty)
            .Where(s => s.FacilityId == facilityId && s.Status)
            .ToListAsync();

        // ✅ Step 1: Primary specialty match
        var primaryMatches = specialists
            .Where(s =>
                s.SpecialistSpecialities.Any(ss =>
                    ss.IsPrimary &&
                    ss.SpecialtyId == requestedSpecialtyId))
            .Select(s => MapSpecialist(s))
            .ToList();

        if (primaryMatches.Any())
            return primaryMatches;

        // ✅ Step 2: Secondary specialty match
        var secondaryMatches = specialists
            .Where(s =>
                s.SpecialistSpecialities.Any(ss =>
                    !ss.IsPrimary &&
                    ss.SpecialtyId == requestedSpecialtyId))
            .Select(s => MapSpecialist(s))
            .ToList();

        return secondaryMatches;
    }

    // ✅ Helper mapper (keeps code clean)
    private SpecialistMatchDto MapSpecialist(Specialist s)
    {
        return new SpecialistMatchDto
        {
            SpecialistId = s.SpecialistId,
            SpecialistName = s.User.FirstName + " " + s.User.LastName,
            
            ShiftBlock = s.ShiftBlock.StartTime + " - " + s.ShiftBlock.EndTime
        };
    }
    public async Task<IEnumerable<FacilityDto>> GetFacilitiesForReferralDropdownAsync(int referralId)
    {
        // Load referral with origin facility and hospital
        var referral = await _context.Referrals
            .Include(r => r.OriginFacility)
            .ThenInclude(f => f.Hospital)
            .FirstOrDefaultAsync(r => r.ReferralId == referralId);

        if (referral == null)
            return Enumerable.Empty<FacilityDto>();

        var hospitalId = referral.OriginFacility.HospitalId;
        var originFacilityId = referral.OriginFacilityId;
        var requestedSpecialityId = referral.SpecialtyRequestId; // matches your schema

        // Query facilities in same hospital, different from origin facility,
        // that have specialists with the requested specialty
        var facilities = await _context.Facilities
            .Include(f => f.Specialists)
            .ThenInclude(s => s.SpecialistSpecialities)
            .Where(f => f.HospitalId == hospitalId &&
                        f.FacilityId != originFacilityId &&
                        f.Specialists.Any(s => s.SpecialistSpecialities
                            .Any(ss => ss.SpecialtyId == requestedSpecialityId)))
            .ToListAsync();

        return facilities.Select(f => new FacilityDto
        {
            FacilityId = f.FacilityId,
            FacilityName = f.FacilityName
        });
    }

    //send the referral details to the coordinators in other facilities
    public async Task<List<Referral>> RouteReferralAsync(CreateReferralRequest request)
    {
        var referrals = new List<Referral>();

        foreach (var destFacilityId in request.DestinationFacilityIds)
        {
            var referral = new Referral
            {
                PatientId = request.PatientId,
                OriginFacilityId = request.OriginFacilityId,
                DestinationFacilityId = destFacilityId,
                CreatedByCoordinatorId = request.CreatedByCoordinatorId,
                SpecialtyRequestId = request.SpecialtyRequestId,
                ReferralReason = request.ReferralReason,
                DiagnosisCode = request.DiagnosisCode,
                UrgencyLevelId = request.UrgencyLevelId,
                ReferralStatusId = request.ReferralStatusId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Referrals.Add(referral);
            referrals.Add(referral);
        }

        await _context.SaveChangesAsync();
        return referrals;
    }

    //get urgency level for dropdown
   


}