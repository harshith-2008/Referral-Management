namespace Referral_Management.Api.Services.Interfaces
{
    public interface ICoordinatorService
    {
        Task<CoordinatorDashboardDto> GetDashboardAsync(int coordinatorId);
    }
}
