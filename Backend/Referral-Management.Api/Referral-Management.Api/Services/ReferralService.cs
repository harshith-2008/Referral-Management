using Referral_Management.Api.DTOs;
using Referral_Management.Api.Models;
using Referral_Management.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Referral_Management.Api.Exceptions;
using System;
using System.Linq;
using System.Threading.Tasks;
            
namespace Referral_Management.Api.Services;

public class ReferralService : IReferralService
{
    private readonly AppDbContext _context;

    public ReferralService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<ReferralDto>> GetRequestedReferralsForCoordinator(
    int coordinatorId)
    {
        var coordinator = await _context.ReferralCoordinators
            .AsNoTracking()
            .FirstOrDefaultAsync(c =>
                c.ReferralCoordinatorId == coordinatorId);

        if (coordinator == null)
            return new List<ReferralDto>();

        var facilityId = coordinator.FacilityId;

        return await _context.Referrals
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
                r.ReferralStatus.StatusName == "Requested")
            .Select(r => new ReferralDto
            {
                ReferralId = r.ReferralId,

                PatientName =
                    r.Patient.User.FirstName + " " +
                    r.Patient.User.LastName,

                OriginFacility =
                    r.OriginFacility.FacilityName,

                DestinationFacility =
                    r.DestinationFacility.FacilityName,

                Status =
                    r.ReferralStatus.StatusName,

                Urgency =
                    r.UrgencyLevel.LevelName,

                Specialty =
                    r.SpecialtyRequest.SpecialtyName,

                DiagnosisCode =
                    r.DiagnosisCode,

                CreatedAt =
                    r.CreatedAt ?? DateTime.UtcNow
            })
            .ToListAsync();
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
            CreatedAt = System.DateTime.UtcNow,

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
    public async Task<FacilitiesDropdownResponseDto> GetFacilitiesForReferralDropdownAsync(
        int referralId,
        int coordinatorId)
    {
        var coordinator = await _context.ReferralCoordinators
            .AsNoTracking()
            .Include(c => c.Facility)
            .FirstOrDefaultAsync(c => c.ReferralCoordinatorId == coordinatorId);

        if (coordinator == null)
            throw new BadRequestException("Referral coordinator not found.");

        // Load referral with origin facility and hospital
        var referral = await _context.Referrals
            .Include(r => r.OriginFacility)
                .ThenInclude(f => f.Hospital)
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.ReferralId == referralId);

        if (referral == null)
              throw new BadRequestException("Referral not found.");

        var originHospitalId = referral.OriginFacility.HospitalId;
        var originFacilityId = referral.OriginFacilityId;
        var requestedSpecialityId = referral.SpecialtyRequestId;

        // Query facilities (excluding the origin facility) that have specialists with the requested specialty.
        var facilities = await _context.Facilities
            .AsNoTracking()
            .Include(f => f.Hospital)
            .Where(f =>
                f.FacilityId != originFacilityId &&
                f.Specialists.Any(s =>
                    s.Status &&
                    s.SpecialistSpecialities.Any(ss =>
                        ss.SpecialtyId == requestedSpecialityId)))
            .Select(f => new
            {
                f.FacilityId,
                f.FacilityName,
                f.City,
                f.State,
                f.HospitalId,
                HospitalName = f.Hospital.HospitalName,
                f.Latitude,
                f.Longitude,
                AvailableSpecialists = f.Specialists.Count(s =>
                    s.Status &&
                    s.SpecialistSpecialities.Any(ss => ss.SpecialtyId == requestedSpecialityId))
            })
            .ToListAsync();

        var response = new FacilitiesDropdownResponseDto();

        foreach (var f in facilities)
        {
            var dto = new FacilityDropdownDto
            {
                FacilityId = f.FacilityId,
                FacilityName = f.FacilityName,
                AvailableSpecialists = f.AvailableSpecialists,
                HospitalId = f.HospitalId,
                HospitalName = f.HospitalName ?? string.Empty,
                City = f.City,
                State = f.State,
                InNetwork = f.HospitalId == originHospitalId,
                DistanceKm = CalculateDistanceKm(
                    coordinator.Facility.Latitude,
                    coordinator.Facility.Longitude,
                    f.Latitude,
                    f.Longitude)
            };

            if (dto.InNetwork)
                response.InNetwork.Add(dto);
            else
                response.OutOfNetwork.Add(dto);
        }

        // Nearest facilities first; facilities without coordinates are placed last.
        response.InNetwork = response.InNetwork
            .OrderBy(x => x.DistanceKm ?? double.MaxValue)
            .ThenByDescending(x => x.AvailableSpecialists)
            .ToList();
        response.OutOfNetwork = response.OutOfNetwork
            .OrderBy(x => x.DistanceKm ?? double.MaxValue)
            .ThenByDescending(x => x.AvailableSpecialists)
            .ToList();

        return response;
    }

    private static double? CalculateDistanceKm(
        decimal? originLatitude,
        decimal? originLongitude,
        decimal? destinationLatitude,
        decimal? destinationLongitude)
    {
        if (!originLatitude.HasValue || !originLongitude.HasValue ||
            !destinationLatitude.HasValue || !destinationLongitude.HasValue)
            return null;

        const double earthRadiusKm = 6371;
        var latitudeDifference = ToRadians((double)(destinationLatitude.Value - originLatitude.Value));
        var longitudeDifference = ToRadians((double)(destinationLongitude.Value - originLongitude.Value));
        var originLatitudeRadians = ToRadians((double)originLatitude.Value);
        var destinationLatitudeRadians = ToRadians((double)destinationLatitude.Value);

        var haversine = Math.Sin(latitudeDifference / 2) * Math.Sin(latitudeDifference / 2) +
                        Math.Cos(originLatitudeRadians) * Math.Cos(destinationLatitudeRadians) *
                        Math.Sin(longitudeDifference / 2) * Math.Sin(longitudeDifference / 2);

        return Math.Round(earthRadiusKm * 2 * Math.Atan2(Math.Sqrt(haversine), Math.Sqrt(1 - haversine)), 1);
    }

    private static double ToRadians(double degrees) => degrees * Math.PI / 180;

    //send the referral details to the coordinators in other facilities
    public async Task<List<ReferralDto>> RouteReferralAsync(
        CreateReferralRequest request)
    {
        // Validate input and throw exceptions so middleware returns ApiErrorResponseDTO
        var originalReferral = await _context.Referrals
            .FirstOrDefaultAsync(r => r.ReferralId == request.ReferralId);

        if (originalReferral == null)
            throw new BadRequestException("Original referral not found.");

        if (request.DestinationFacilityIds == null || !request.DestinationFacilityIds.Any())
            throw new BadRequestException("No destination facilities provided.");

        // Determine group id only for multi-send
        Guid? groupId = null;
        if (request.DestinationFacilityIds.Count > 1)
        {
            groupId = Guid.NewGuid();
        }

        // Update original referral with first destination
        originalReferral.DestinationFacilityId = request.DestinationFacilityIds.First();
        originalReferral.CreatedByCoordinatorId = request.CreatedByCoordinatorId;
        originalReferral.UpdatedAt = DateTime.UtcNow;

        if (groupId.HasValue)
            originalReferral.ReferralGroupId = groupId;

        var requestedStatusId = await _context.ReferralStatuses
            .Where(s => s.StatusName == "Requested")
            .Select(s => s.ReferralStatusId)
            .FirstAsync();

        originalReferral.ReferralStatusId = requestedStatusId;

        var createdReferrals = new List<Referral> { originalReferral };

        // Create copies for the remaining facilities (if any)
        foreach (var facilityId in request.DestinationFacilityIds.Skip(1))
        {
            var referral = new Referral
            {
                PatientId = originalReferral.PatientId,
                OriginFacilityId = originalReferral.OriginFacilityId,
                DestinationFacilityId = facilityId,
                ReferralStatusId = requestedStatusId,
                CreatedByCoordinatorId = request.CreatedByCoordinatorId,
                FromSpecialistId = originalReferral.FromSpecialistId,
                SpecialtyRequestId = originalReferral.SpecialtyRequestId,
                ReferralReason = originalReferral.ReferralReason,
                DiagnosisCode = originalReferral.DiagnosisCode,
                UrgencyLevelId = originalReferral.UrgencyLevelId,
                ReferralGroupId = groupId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Referrals.Add(referral);
            createdReferrals.Add(referral);
        }

        await _context.SaveChangesAsync();

        // Map created referrals to ReferralDto (consistent with other methods)
        var createdIds = createdReferrals.Select(r => r.ReferralId).ToList();

        var referredDtos = await _context.Referrals
            .AsNoTracking()
            .Include(r => r.Patient)
                .ThenInclude(p => p.User)
            .Include(r => r.OriginFacility)
            .Include(r => r.DestinationFacility)
            .Include(r => r.ReferralStatus)
            .Include(r => r.SpecialtyRequest)
            .Include(r => r.UrgencyLevel)
            .Where(r => createdIds.Contains(r.ReferralId))
            .Select(r => new ReferralDto
            {
                ReferralId = r.ReferralId,
                PatientName = r.Patient.User.FirstName + " " + r.Patient.User.LastName,
                OriginFacility = r.OriginFacility.FacilityName,
                DestinationFacility = r.DestinationFacility.FacilityName,
                Status = r.ReferralStatus.StatusName,
                Urgency = r.UrgencyLevel.LevelName,
                Specialty = r.SpecialtyRequest.SpecialtyName,
                DiagnosisCode = r.DiagnosisCode,
                CreatedAt = r.CreatedAt ?? DateTime.UtcNow,
                // optional: include group id on DTO if your DTO supports it
                ReferralGroupId = r.ReferralGroupId
            })
            .ToListAsync();

        return referredDtos;
    }

    public async Task<List<ReferralDto>> GetSubmittedReferralsForCoordinator(int coordinatorId)
    {
        var coordinator = await _context.ReferralCoordinators
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.ReferralCoordinatorId == coordinatorId);

        if (coordinator == null)
            return new List<ReferralDto>();

        var facilityId = coordinator.FacilityId;

        return await _context.Referrals
            .AsNoTracking()
            .Include(r => r.Patient)
                .ThenInclude(p => p.User)
            .Include(r => r.OriginFacility)
            .Include(r => r.DestinationFacility)
            .Include(r => r.ReferralStatus)
            .Include(r => r.SpecialtyRequest)
            .Include(r => r.UrgencyLevel)
            .Where(r =>
                r.OriginFacilityId == facilityId &&
                r.ReferralStatus.StatusName == "Submitted")
            .Select(r => new ReferralDto
            {
                ReferralId = r.ReferralId,

                PatientName =
                    r.Patient.User.FirstName + " " +
                    r.Patient.User.LastName,

                OriginFacility =
                    r.OriginFacility.FacilityName,

                DestinationFacility =
                    r.DestinationFacility.FacilityName,

                Status =
                    r.ReferralStatus.StatusName,

                Urgency =
                    r.UrgencyLevel.LevelName,

                Specialty =
                    r.SpecialtyRequest.SpecialtyName,

                DiagnosisCode =
                    r.DiagnosisCode,

                CreatedAt =
                    r.CreatedAt!.Value
            })
            .ToListAsync();
    }

    public async Task<List<ReferralDto>>
    GetOriginFacilityReferralsForCoordinator(int coordinatorId)
    {
        var coordinator = await _context.ReferralCoordinators
            .AsNoTracking()
            .FirstOrDefaultAsync(c =>
                c.ReferralCoordinatorId == coordinatorId);

        if (coordinator == null)
            return new List<ReferralDto>();

        var facilityId = coordinator.FacilityId;

        return await _context.Referrals
            .AsNoTracking()
            .Include(r => r.Patient)
                .ThenInclude(p => p.User)
            .Include(r => r.OriginFacility)
            .Include(r => r.DestinationFacility)
            .Include(r => r.ReferralStatus)
            .Include(r => r.SpecialtyRequest)
            .Include(r => r.UrgencyLevel)
            .Where(r => r.OriginFacilityId == facilityId)
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new ReferralDto
            {
                ReferralId = r.ReferralId,

                PatientName =
                    r.Patient.User.FirstName + " " +
                    r.Patient.User.LastName,

                OriginFacility =
                    r.OriginFacility.FacilityName,

                DestinationFacility =
                    r.DestinationFacility.FacilityName,

                Status =
                    r.ReferralStatus.StatusName,

                Urgency =
                    r.UrgencyLevel.LevelName,

                Specialty =
                    r.SpecialtyRequest.SpecialtyName,

                DiagnosisCode =
                    r.DiagnosisCode,

                CreatedAt =
                    r.CreatedAt ?? DateTime.UtcNow
            })
            .ToListAsync();
    }

    public async Task<List<ReferralDto>>
GetReferralsRaisedBySpecialistAsync(int specialistId)
    {
        return await _context.Referrals
            .AsNoTracking()
            .Include(r => r.Patient)
                .ThenInclude(p => p.User)
            .Include(r => r.OriginFacility)
            .Include(r => r.DestinationFacility)
            .Include(r => r.ReferralStatus)
            .Include(r => r.SpecialtyRequest)
            .Include(r => r.UrgencyLevel)
            .Where(r => r.FromSpecialistId == specialistId)
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new ReferralDto
            {
                ReferralId = r.ReferralId,

                PatientName =
                    r.Patient.User.FirstName + " " +
                    r.Patient.User.LastName,

                OriginFacility =
                    r.OriginFacility.FacilityName,

                DestinationFacility =
                    r.DestinationFacility.FacilityName,

                Status =
                    r.ReferralStatus.StatusName,

                Urgency =
                    r.UrgencyLevel.LevelName,

                Specialty =
                    r.SpecialtyRequest.SpecialtyName,

                DiagnosisCode =
                    r.DiagnosisCode,

                CreatedAt =
                    r.CreatedAt ?? DateTime.UtcNow
            })
            .ToListAsync();
    }

}
