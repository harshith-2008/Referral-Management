using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Referral_Management.Api.DTOs;
using Referral_Management.Api.DTOs.Common;
using Referral_Management.Api.Services.Interfaces;
using System.Security.Claims;

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

    [HttpGet("requested")]
    public async Task<IActionResult> GetRequestedReferrals()
    {
        var coordinatorIdClaim =
            User.FindFirst("ReferralCoordinatorId")?.Value;

        if (string.IsNullOrEmpty(coordinatorIdClaim))
            return Unauthorized();

        var coordinatorId = int.Parse(coordinatorIdClaim);

        var result = await _referralService
            .GetRequestedReferralsForCoordinator(coordinatorId);

        return Ok(new ApiResponseDTO<List<ReferralDto>>
        {
            Success = true,
            Message = "Requested referrals fetched successfully.",
            Data = result
        });
    }

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

    [Authorize]
    [HttpPost("route")]
    public async Task<IActionResult> RouteReferral(
        [FromBody] CreateReferralRequest request)
    {
        var coordinatorIdClaim =
            User.FindFirst("ReferralCoordinatorId")?.Value;

        Console.WriteLine($"Coordinator Claim: {coordinatorIdClaim}");

        if (string.IsNullOrEmpty(coordinatorIdClaim))
            return Unauthorized();

        var coordinatorId = int.Parse(coordinatorIdClaim);

        Console.WriteLine(request.CreatedByCoordinatorId);

        request.CreatedByCoordinatorId = coordinatorId;

        var referrals =
            await _referralService.RouteReferralAsync(request);

        return Ok(referrals);
    }

    [HttpGet("submitted")]
    public async Task<IActionResult> GetSubmittedReferrals()
    {
        var coordinatorIdClaim =
            User.FindFirst("ReferralCoordinatorId")?.Value;

        if (string.IsNullOrEmpty(coordinatorIdClaim))
            return Unauthorized();

        var coordinatorId = int.Parse(coordinatorIdClaim);

        var result = await _referralService
            .GetSubmittedReferralsForCoordinator(coordinatorId);

        return Ok(new ApiResponseDTO<List<ReferralDto>>
        {
            Success = true,
            Message = "Submitted referrals fetched successfully.",
            Data = result
        });
    }
}