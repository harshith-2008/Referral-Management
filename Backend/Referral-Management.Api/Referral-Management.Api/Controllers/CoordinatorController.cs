using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Referral_Management.Api.DTOs;
using Referral_Management.Api.DTOs.Common;
using Referral_Management.Api.Services.Interfaces;

namespace Referral_Management.Api.Controllers;

[ApiController]
[Route("api/coordinator")]
[Authorize(Roles = "ReferralCoordinator")]
public class CoordinatorController : ControllerBase
{
    private readonly ICoordinatorService _coordinatorService;

    public CoordinatorController(ICoordinatorService coordinatorService)
    {
        _coordinatorService = coordinatorService;
    }

    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboard()
    {
        var coordinatorIdClaim =
            User.FindFirst("ReferralCoordinatorId")?.Value;

        if (string.IsNullOrEmpty(coordinatorIdClaim))
            return Unauthorized();

        var coordinatorId = int.Parse(coordinatorIdClaim);

        var result =
            await _coordinatorService.GetDashboardAsync(coordinatorId);

        return Ok(new ApiResponseDTO<CoordinatorDashboardDto>
        {
            Success = true,
            Message = "Dashboard data retrieved successfully.",
            Data = result
        });
    }
}
