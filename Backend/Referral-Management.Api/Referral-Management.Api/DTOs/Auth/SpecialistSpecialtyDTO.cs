using System.ComponentModel.DataAnnotations;

namespace Referral_Management.Api.DTOs.Auth;

public class SpecialistSpecialtyDTO
{
    [Required]
    [Range(1, int.MaxValue)]
    public int SpecialtyId { get; set; }

    public bool IsPrimary { get; set; }
}