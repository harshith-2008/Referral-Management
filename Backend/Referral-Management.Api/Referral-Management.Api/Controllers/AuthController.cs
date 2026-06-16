using Microsoft.AspNetCore.Mvc;
using Referral_Management.Api.DTOs.Auth;
using Referral_Management.Api.Services.Interfaces;

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
        try
        {
            var response = await _authService.RegisterAsync(registerUserDTO);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                Message = ex.Message,
                InnerException = ex.InnerException?.Message
            });
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDTO>> Login(
    LoginDTO loginDTO)
    {
        var response =
            await _authService.LoginAsync(loginDTO);

        return Ok(response);
    }
}