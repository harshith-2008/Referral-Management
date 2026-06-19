using Microsoft.EntityFrameworkCore;
using Referral_Management.Api.DTOs;
using Referral_Management.Api.Models;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;

namespace Referral_Management.Api.Services
{
    public class ReferralService1 : IReferralService1
    {
        private readonly AppDbContext _context;

        public ReferralService1(AppDbContext context)
        {
            _context = context;
        }
        
        //get facilities with the mentioned specialityid for cOordinator dashboard
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

}
