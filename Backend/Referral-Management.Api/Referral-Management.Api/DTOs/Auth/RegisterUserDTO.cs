using System.ComponentModel.DataAnnotations;

namespace Referral_Management.Api.DTOs.Auth;

public class RegisterUserDTO
{
    // Common User Fields

    [Required]
    [Range(1, 4, ErrorMessage = "Invalid role selected.")]
    public int RoleId { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
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
    [MinLength(6, ErrorMessage = "Password should have minimum of 6 characters")]
    [MaxLength(100)]
    public string Password { get; set; } = string.Empty;

    [Required]
    public string PhoneNumber { get; set; } = string.Empty;


    // =========================
    // Patient Fields
    // =========================

    public string? Mrn { get; set; }

    public DateOnly? Dob { get; set; }

    [MaxLength(1)]
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