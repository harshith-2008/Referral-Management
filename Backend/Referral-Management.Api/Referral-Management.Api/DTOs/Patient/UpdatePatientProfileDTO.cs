using System.ComponentModel.DataAnnotations;

namespace Referral_Management.Api.DTOs.Patient
{
    public class UpdatePatientProfileDto
    {
        [Required]
        [Phone]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        public string PatientAddress { get; set; } = string.Empty;

        public string? InsuranceProviderName { get; set; }

        public string? InsuranceStatus { get; set; }
    }
}