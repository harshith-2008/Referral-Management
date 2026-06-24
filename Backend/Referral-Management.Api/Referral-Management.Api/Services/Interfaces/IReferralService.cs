using Referral_Management.Api.DTOs;
using Referral_Management.Api.Models;

namespace Referral_Management.Api.Services.Interfaces;

public interface IReferralService
{
    Task<List<ReferralDto>> GetRequestedReferralsForCoordinator(int coordinatorId);
    Task<ReferralDetailDto?> GetReferralDetailsById(int referralId);
    Task<List<SpecialistMatchDto>> GetMatchingSpecialistsForReferral(int referralId);
    Task<IEnumerable<FacilityDto>> GetFacilitiesForReferralDropdownAsync(int referralId);

    Task<List<Referral>> RouteReferralAsync(CreateReferralRequest request);

    Task<List<ReferralDto>> GetSubmittedReferralsForCoordinator(int coordinatorId);

    Task<List<ReferralDto>> GetOriginFacilityReferralsForCoordinator(int coordinatorId);

    Task<List<ReferralDto>> GetReferralsRaisedBySpecialistAsync(int specialistId);


}