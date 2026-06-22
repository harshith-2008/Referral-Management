using Referral_Management.Api.DTOs.Patient;
using Referral_Management.Api.Services;

namespace Referral_Management.Api.Services
{
    public interface IPatientService
    {
        // ==========================
        // Dashboard
        // ==========================
        Task<PatientDashboardDto> GetDashboardAsync(int userId);

        // ==========================
        // Profile
        // ==========================
        Task<PatientProfileDto> GetProfileAsync(int userId);

        Task<string> UpdateProfileAsync(
            int userId,
            UpdatePatientProfileDto dto);

        // ==========================
        // Referrals
        // ==========================
        Task<List<ReferralDto>> GetReferralsAsync(int userId);

        Task<ReferralDetailsDto> GetReferralByIdAsync(
            int userId,
            int referralId);

        Task<ReferralStatusDto> GetReferralStatusAsync(
            int userId,
            int referralId);

        // ==========================
        // Appointments
        // ==========================
        Task<List<AppointmentDto>> GetAppointmentsAsync(int userId);

        Task<List<AppointmentDto>> GetUpcomingAppointmentsAsync(int userId);

        Task<List<AppointmentDto>> GetAppointmentHistoryAsync(int userId);

        Task<AppointmentDetailsDto> GetAppointmentByIdAsync(
            int userId,
            int appointmentId);
    }
}

