namespace Referral_Management.Api.DTOs.Patient
{
    public class PatientDashboardDto
    {
        public PatientProfileDto Profile { get; set; } = new();

        public int TotalReferrals { get; set; }

        public int PendingReferrals { get; set; }

        public int CompletedReferrals { get; set; }

        public int UpcomingAppointments { get; set; }

        public List<AppointmentDto> UpcomingAppointmentList { get; set; } = new();

        public List<ReferralDto> RecentReferrals { get; set; } = new();
    }
}