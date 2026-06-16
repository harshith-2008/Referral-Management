using System.ComponentModel.DataAnnotations;

namespace Referral_Management.Api.DTOs.Auth;

public class RegisterUserDTO
{
    // Common User Fields

    [Required]
    public int RoleId { get; set; }

    [Required]
    public int FacilityId { get; set; }

    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;

    [Required]
    public string PhoneNumber { get; set; } = string.Empty;


    // =========================
    // Patient Fields
    // =========================

    public string? Mrn { get; set; }

    public DateOnly? Dob { get; set; }

    public string? Gender { get; set; }

    public string? InsuranceProviderName { get; set; }

    public string? InsuranceStatus { get; set; }

    public string? PatientAddress { get; set; }


    // =========================
    // Specialist Fields
    // =========================

    public int? ShiftBlockId { get; set; }

    public List<SpecialistSpecialtyDTO>? Specialties { get; set; }
}