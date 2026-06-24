using Referral_Management.Api.DTOs;

public class CoordinatorDashboardDto
{
    public int TotalReferrals { get; set; }
    public int Submitted { get; set; }
    public int Requested { get; set; }
    public int Accepted { get; set; }
    public int Rejected { get; set; }
    public int Closed { get; set; }

    public List<ReferralDto> RecentReferrals { get; set; } = [];
}