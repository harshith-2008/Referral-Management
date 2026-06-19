using Referral_Management.Api.DTOs;
using Referral_Management.Api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Referral_Management.Api.Services
{
    public interface IReferralService1
    {
        Task<IEnumerable<FacilityDto>> GetFacilitiesForReferralDropdownAsync(int referralId);

        Task<List<Referral>> RouteReferralAsync(CreateReferralRequest request);
        Task<IEnumerable<GetUrgencyLevelsDto>> GetAllUrgenciesAsync();   // new method
        Task<IEnumerable<GetSpecialityDto>> GetAllSpecialitiesAsync();  // new method
    }
}
