using Referral_Management.Api.Models;
using Referral_Management.Api.DTOs;

namespace Referral_Management.Api.Services.Interfaces;

public interface IReferralService
{
    Task<List<ReferralDto>> GetRequestedReferralsForCoordinator(int coordinatorId);
    Task<ReferralDetailDto?> GetReferralDetailsById(int referralId);
    Task<List<SpecialistMatchDto>> GetMatchingSpecialistsForReferral(int referralId);

    // Updated: returns partitioned in-network / out-of-network response
    Task<FacilitiesDropdownResponseDto> GetFacilitiesForReferralDropdownAsync(int referralId, int coordinatorId);

    // changed to return DTOs (consistent with controllers' ApiResponseDTO<T>)
    Task<List<ReferralDto>> RouteReferralAsync(CreateReferralRequest request);
    Task RejectReferralAsync(int referralId, int coordinatorId);

    Task<List<ReferralDto>> GetSubmittedReferralsForCoordinator(int coordinatorId);

    Task<List<ReferralDto>> GetOriginFacilityReferralsForCoordinator(int coordinatorId);

    Task<List<ReferralDto>> GetReferralsRaisedBySpecialistAsync(int specialistId);
}
