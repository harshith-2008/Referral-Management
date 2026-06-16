namespace Referral_Management.Api.DTOs.Auth;

public class LoginResponseDTO
{
    public int UserId { get; set; }

    public string Email { get; set; } = string.Empty;

    public int RoleId { get; set; }

    public string Token { get; set; } = string.Empty;
}