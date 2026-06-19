using Microsoft.AspNetCore.Mvc;
using Referral_Management.Api.DTOs;
using Referral_Management.Api.DTOs.Common;
using Referral_Management.Api.Services.Interfaces;

namespace Referral_Management.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReferralController : ControllerBase
{
    private readonly IReferralService _referralService;

    public ReferralController(IReferralService referralService)
    {
        _referralService = referralService;
    }

    // ✅ GET: api/referral/requested/{coordinatorId}
    [HttpGet("requested/{coordinatorId:int}")]
    public async Task<IActionResult> GetRequestedReferrals(int coordinatorId)
    {
        // ✅ Validation: positive integer
        if (coordinatorId <= 0)
        {
            return Ok(new ApiResponseDTO<object>
            {
                Success = false,
                Message = "CoordinatorId must be a positive integer.",
                Data = null
            });
        }

        var result = await _referralService
            .GetRequestedReferralsForCoordinator(coordinatorId);

        // ✅ Coordinator not found
        if (result == null)
        {
            return Ok(new ApiResponseDTO<object>
            {
                Success = false,
                Message = $"Coordinator with id {coordinatorId} not found.",
                Data = null
            });
        }

        // ✅ No referrals
        if (!result.Any())
        {
            return Ok(new ApiResponseDTO<List<ReferralDto>>
            {
                Success = true,
                Message = "No requested referrals found for this coordinator.",
                Data = result
            });
        }

        // ✅ Success
        return Ok(new ApiResponseDTO<List<ReferralDto>>
        {
            Success = true,
            Message = "Requested referrals fetched successfully.",
            Data = result
        });
    }

    // ✅ GET: api/referral/details/{referralId}
    [HttpGet("details/{referralId:int}")]
    public async Task<IActionResult> GetReferralDetails(int referralId)
    {
        // ✅ Validation: positive integer
        if (referralId <= 0)
        {
            return Ok(new ApiResponseDTO<object>
            {
                Success = false,
                Message = "ReferralId must be a positive integer.",
                Data = null
            });
        }

        var result = await _referralService.GetReferralDetailsById(referralId);

        // ✅ Referral not found
        if (result == null)
        {
            return Ok(new ApiResponseDTO<object>
            {
                Success = false,
                Message = $"Referral with id {referralId} not found.",
                Data = null
            });
        }

        // ✅ Success
        return Ok(new ApiResponseDTO<ReferralDetailDto>
        {
            Success = true,
            Message = "Referral details fetched successfully.",
            Data = result
        });
    }
    [HttpGet("specialists/{referralId:int}")]
    public async Task<IActionResult> GetMatchingSpecialists(int referralId)
    {
        if (referralId <= 0)
        {
            return Ok(new ApiResponseDTO<object>
            {
                Success = false,
                Message = "ReferralId must be a positive integer.",
                Data = null
            });
        }

        var result = await _referralService
            .GetMatchingSpecialistsForReferral(referralId);

        if (!result.Any())
        {
            return Ok(new ApiResponseDTO<List<SpecialistMatchDto>>
            {
                Success = true,
                Message = "No specialists found matching the requested specialty.",
                Data = result
            });
        }

        return Ok(new ApiResponseDTO<List<SpecialistMatchDto>>
        {
            Success = true,
            Message = "Matching specialists fetched successfully.",
            Data = result
        });
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

    

}