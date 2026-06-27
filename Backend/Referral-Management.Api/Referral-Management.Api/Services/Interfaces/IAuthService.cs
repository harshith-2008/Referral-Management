using Referral_Management.Api.DTOs.Auth;

namespace Referral_Management.Api.Services.Interfaces;

public interface IAuthService
{
    Task<RegisterResponseDTO> RegisterAsync(RegisterUserDTO dto);
    Task<LoginResponseDTO> LoginAsync(LoginDTO dto);

    Task<UserProfileDTO> GetCurrentUserAsync(int userId);
}