namespace Referral_Management.Api.DTOs.Patient
{
    public class ReferralDetailsDto
    {
        public int ReferralId { get; set; }

        public string ReferralReason { get; set; } = string.Empty;

        public string DiagnosisCode { get; set; } = string.Empty;

        public string Specialty { get; set; } = string.Empty;

        public string ReferralStatus { get; set; } = string.Empty;

        public string Urgency { get; set; } = string.Empty;

        public string OriginFacility { get; set; } = string.Empty;

        public string DestinationFacility { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
    }
}