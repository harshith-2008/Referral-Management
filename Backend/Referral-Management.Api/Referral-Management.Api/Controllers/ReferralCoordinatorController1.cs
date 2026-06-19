using Microsoft.AspNetCore.Mvc;
using Referral_Management.Api.DTOs;
using Referral_Management.Api.Models;
using Referral_Management.Api.Services;
using System.Threading.Tasks;

namespace Referral_Management.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReferralCoordinatorController : ControllerBase
    {
        private readonly IReferralService1 _referralService;

        public ReferralCoordinatorController(IReferralService1 referralService)
        {
            _referralService = referralService;
        }


        // Endpoint for dropdown list
        [HttpGet("{referralId}/facilities-dropdown")]
        public async Task<IActionResult> GetFacilitiesDropdown(int referralId)
        {
            var facilities = await _referralService.GetFacilitiesForReferralDropdownAsync(referralId);

            if (facilities == null || !facilities.Any())
                return NotFound("No facilities found for this referral");

            return Ok(facilities);
        }
        //End point for submitting Referrral request from Coordinator1->Coordinator2
        [HttpPost("route")]
        public async Task<IActionResult> RouteReferral([FromBody] CreateReferralRequest request)
        {
            var referrals = await _referralService.RouteReferralAsync(request);
            return Ok(referrals);
        }

        //Endpoint for getting all the urgencylevels for dropdown
        [HttpGet("urgencyLevels")]
        public async Task<IActionResult> GetUrgencies()
        {
            var urgencylevels = await _referralService.GetAllUrgenciesAsync();

            if (urgencylevels == null || !urgencylevels.Any())
                return NotFound("No urgency records found");

            return Ok(urgencylevels);
        }

        //Endpoint to get all teh Specialities
        [HttpGet("specialities")]
        public async Task<IActionResult> GetSpecialities()
        {
            var specialities = await _referralService.GetAllSpecialitiesAsync();

            if (specialities == null || !specialities.Any())
                return NotFound("No specialities found");

            return Ok(specialities);
        }


    }
}
