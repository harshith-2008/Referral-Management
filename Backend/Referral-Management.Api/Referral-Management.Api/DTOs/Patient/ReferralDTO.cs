namespace Referral_Management.Api.DTOs.Patient
{
    public class ReferralDto
    {
        public int ReferralId { get; set; }

        public string Specialty { get; set; } = string.Empty;

        public string ReferralStatus { get; set; } = string.Empty;

        public string Urgency { get; set; } = string.Empty;

        public string OriginFacility { get; set; } = string.Empty;

        public string DestinationFacility { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
    }
}