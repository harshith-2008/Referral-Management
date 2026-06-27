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

    [AllowAnonymous]
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

    [AllowAnonymous]
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

    [Authorize(Roles = "Admin,Patient,ReferralCoordinator,Specialist")]
    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId == null)
            return Unauthorized();

        var result = await _authService.GetCurrentUserAsync(int.Parse(userId));

        return Ok(new ApiResponseDTO<UserProfileDTO>
        {
            Success = true,
            Message = "Profile fetched successfully",
            Data = result
        });
    }
}
