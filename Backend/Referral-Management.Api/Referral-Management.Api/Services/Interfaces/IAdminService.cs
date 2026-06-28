using Referral_Management.Api.DTOs.Admin;

namespace Referral_Management.Api.Services.Interfaces
{
    public interface IAdminService
    {
        Task<AdminDashboardDto> GetDashboardAsync();

        Task<ReferralLeakageDto> GetReferralLeakageAsync();

        Task<List<StatusCountDto>> GetReferralStatusAsync();

        Task<List<FacilityLeakageDto>> GetFacilityLeakageAsync();

        Task<List<SpecialtyLoadDto>> GetSpecialtyLoadAsync();

        Task<AppointmentAnalyticsDto> GetAppointmentAnalyticsAsync();

        Task<List<DailyReferralDto>> GetDailyReferralsAsync();
        Task<List<UserListDto>> GetUsersAsync(int adminUserId);

        Task<List<MonthlyReferralDto>> GetMonthlyReferralAsync();
        Task<List<TopSpecialistDto>> GetTopSpecialistsAsync();

        Task<ReferralAgingDto> GetReferralAgingAsync();
        Task<ScheduledDelayDto> GetScheduledDelaysAsync();



    }
}
