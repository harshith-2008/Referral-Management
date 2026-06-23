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

    // ✅ GET: api/specialist/assigned-patients/{specialistId}
    [HttpGet("assigned-patients/{specialistId:int}")]
    public async Task<IActionResult> GetAssignedPatients(int specialistId)
    {
        if (specialistId <= 0)
        {
            return Ok(new ApiResponseDTO<object>
            {
                Success = false,
                Message = "SpecialistId must be a positive integer.",
                Data = null
            });
        }

        var result = await _specialistService.GetAssignedPatients(specialistId);

        if (!result.Any())
        {
            return Ok(new ApiResponseDTO<List<SpecialistPatientDto>>
            {
                Success = true,
                Message = "No patients assigned to this specialist.",
                Data = result
            });
        }

        return Ok(new ApiResponseDTO<List<SpecialistPatientDto>>
        {
            Success = true,
            Message = "Assigned patients fetched successfully.",
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