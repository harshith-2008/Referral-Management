namespace Referral_Management.Api.DTOs.Auth;

public class UserProfileDTO
{
    public int UserId { get; set; }
    public string Email { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;

    public int FacilityId { get; set; }
    public string? FacilityName { get; set; }

    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    // Role IDs (optional)
    public int? PatientId { get; set; }
    public int? SpecialistId { get; set; }
    public int? ReferralCoordinatorId { get; set; }
    public int? AdminId { get; set; }
}