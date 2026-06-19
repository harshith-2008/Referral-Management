namespace Referral_Management.Api.DTOs.Patient
{
    public class ReferralStatusDto
    {
        public int ReferralId { get; set; }

        public string ReferralStatus { get; set; } = string.Empty;

        public DateTime LastUpdated { get; set; }
    }
}