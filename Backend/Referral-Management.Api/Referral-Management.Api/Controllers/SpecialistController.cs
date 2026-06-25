using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Referral_Management.Api.DTOs;
using Referral_Management.Api.DTOs.Common;
using Referral_Management.Api.Services;
using Referral_Management.Api.Services.Interfaces;
using System.Security.Claims;

namespace Referral_Management.Api.Controllers;

[ApiController]
[Route("api/specialist")]
public class SpecialistController : ControllerBase
{
    private readonly ISpecialistService _specialistService;

    public SpecialistController(ISpecialistService specialistService)
    {
        _specialistService = specialistService;
    }

    [Authorize(Roles = "Specialist")]
    [HttpGet("assigned-patients")]
    public async Task<IActionResult> GetAssignedPatients()
    {
        var specialistIdClaim =
            User.FindFirst("SpecialistId")?.Value;

        if (string.IsNullOrEmpty(specialistIdClaim))
            return Unauthorized();

        var specialistId = int.Parse(specialistIdClaim);

        var result =
            await _specialistService.GetAssignedPatients(specialistId);

        return Ok(new ApiResponseDTO<List<SpecialistPatientDto>>
        {
            Success = true,
            Message = result.Any()
                ? "Assigned patients fetched successfully."
                : "No patients assigned to this specialist.",
            Data = result
        });
    }

    [HttpPost("referral-intake")]
    public async Task<IActionResult> CreateReferralIntake(
    [FromBody] ReferralIntakeCreateDto dto)
    {
        var specialistIdClaim = User.FindFirst("SpecialistId")?.Value;

        if (string.IsNullOrEmpty(specialistIdClaim))
            return Unauthorized();

        var specialistId = int.Parse(specialistIdClaim);

        Console.WriteLine("specilaist id in controller", specialistId);

        var referralId = await _specialistService
            .CreateDraftReferral(specialistId, dto);

        return Ok(new ApiResponseDTO<int>
        {
            Success = true,
            Message = "Draft referral created successfully.",
            Data = referralId
        });
    }

    //Endpoint for getting all the urgencylevels for dropdown
    [HttpGet("urgencyLevels")]
    public async Task<IActionResult> GetUrgencies()
    {
        var urgencylevels = await _specialistService.GetAllUrgenciesAsync();

        if (urgencylevels == null || !urgencylevels.Any())
            return NotFound("No urgency records found");

        return Ok(urgencylevels);
    }

    //Endpoint to get all teh Specialities
    [HttpGet("specialities")]
    public async Task<IActionResult> GetSpecialities()
    {
        var specialities = await _specialistService.GetAllSpecialitiesAsync();

        if (specialities == null || !specialities.Any())
            return NotFound("No specialities found");

        return Ok(specialities);
    }

}