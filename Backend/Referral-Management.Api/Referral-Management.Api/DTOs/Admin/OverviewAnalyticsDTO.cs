namespace Referral_Management.Api.DTOs.Admin
{
    public class OverviewAnalyticsDTO
    {
        public int TotalReferrals { get; set; }
        public int TotalFacilities { get; set; }
        public int TotalPatients { get; set; }
        public int TotalSpecialists { get; set; }

        public int CompletedReferrals { get; set; }
        public int PendingReferrals { get; set; }
        public int RejectedReferrals { get; set; }

        public double LeakagePercentage { get; set; }

        public List<SpecialtyAnalyticsDTO> TopSpecialties { get; set; }
    }
}
