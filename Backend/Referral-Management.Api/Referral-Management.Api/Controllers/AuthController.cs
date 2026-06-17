using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Referral_Management.Api.DTOs.Auth;
using Referral_Management.Api.DTOs.Common;
using Referral_Management.Api.Services.Interfaces;
using System.Security.Claims;

namespace Referral_Management.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<RegisterResponseDTO>> Register(
        [FromBody] RegisterUserDTO registerUserDTO)
    {
        var response = await _authService.RegisterAsync(registerUserDTO);

        return Ok(new ApiResponseDTO<RegisterResponseDTO>{
             Success = true,
             Message = "Registration successful.",
             Data = response
        });
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDTO>> Login(
        [FromBody] LoginDTO loginDTO)
    {
        var response = await _authService.LoginAsync(loginDTO);

        return Ok (new ApiResponseDTO<LoginResponseDTO>
        {
            Success = true,
            Message = "Login successful.",
            Data = response
        });
    }

    [Authorize]
    [HttpGet("me")]
    public IActionResult Me()
    {
        return Ok(new
        {
            UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
            Email =  User.FindFirst(ClaimTypes.Email)?.Value,
            Role = User.FindFirst(ClaimTypes.Role)?.Value,
            FacilityId = User.FindFirst("FacilityId")?.Value
        });
    }
}