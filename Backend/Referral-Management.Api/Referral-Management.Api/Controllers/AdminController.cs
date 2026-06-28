using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Referral_Management.Api.Services.Interfaces;
using System.Security.Claims;

[ApiController]
[Route("api/admin")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly IAdminService _service;

    public AdminController(IAdminService service)
    {
        _service = service;
    }



    [HttpGet("users")]
    public async Task<IActionResult> GetUsers()
    {
        var adminUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var users = await _service.GetUsersAsync(adminUserId);
        return Ok(users);
    }


    [HttpGet("dashboard")]
    public async Task<IActionResult> Dashboard()
    {
        return Ok(await _service.GetDashboardAsync());
    }

    [HttpGet("analytics/referral-leakage")]
    public async Task<IActionResult> ReferralLeakage()
    {
        return Ok(await _service.GetReferralLeakageAsync());
    }

    [HttpGet("analytics/referral-status")]
    public async Task<IActionResult> ReferralStatus()
    {
        return Ok(await _service.GetReferralStatusAsync());
    }

    [HttpGet("analytics/facility-leakage")]
    public async Task<IActionResult> FacilityLeakage()
    {
        return Ok(await _service.GetFacilityLeakageAsync());
    }

    [HttpGet("analytics/specialty-load")]
    public async Task<IActionResult> SpecialtyLoad()
    {
        return Ok(await _service.GetSpecialtyLoadAsync());
    }

    [HttpGet("analytics/appointments")]
    public async Task<IActionResult> AppointmentAnalytics()
    {
        return Ok(await _service.GetAppointmentAnalyticsAsync());
    }

    [HttpGet("analytics/daily-referrals")]
    public async Task<IActionResult> DailyReferrals()
    {
        return Ok(await _service.GetDailyReferralsAsync());
    }
    [HttpGet("analytics/monthly-referral")]
    public async Task<IActionResult> GetReferralTrends()
    {
        return Ok(await _service.GetMonthlyReferralAsync());
    }

    [HttpGet("analytics/top-specialists")]
    public async Task<IActionResult> GetTopSpecialists()
    {
        return Ok(await _service.GetTopSpecialistsAsync());
    }

    [HttpGet("analytics/referral-aging")]
    public async Task<IActionResult> GetReferralAging()
    {
        return Ok(await _service.GetReferralAgingAsync());
    }

    [HttpGet("analytics/scheduled-delays")]
    public async Task<IActionResult> GetScheduledDelays()
    {
        return Ok(await _service.GetScheduledDelaysAsync());
    }

}
