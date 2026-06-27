
public class AdminDashboardDto
{
    public int TotalUsers { get; set; }
    public int TotalPatients { get; set; }
    public int TotalSpecialists { get; set; }
    public int TotalReferrals { get; set; }
    public int PendingReferrals { get; set; }
    public int CompletedReferrals { get; set; }
    public int CancelledReferrals { get; set; }
    public int AppointmentsToday { get; set; }

    public double ReferralsPerPatient { get; set; }
    public double AverageReferralsPerFacility { get; set; }

}
