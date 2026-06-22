namespace Referral_Management.Api.Services.Interfaces
{
    public interface IAppointmentService
    {
        Task<AvailableSlotsResponseDTO> GetAvailableSlotsAsync(int specialistId, DateOnly date);

        Task<DailyScheduleResponseDTO> GetScheduleAsync(int specialistId, DateOnly date);

        Task<AppointmentResponseDTO> CreateAppointmentAsync(CreateAppointmentDTO request);

        Task<AppointmentDetailsDTO> GetAppointmentDetailsAsync(int appointmentId);

        Task<List<UserAppointmentDTO>> GetUserAppointmentsAsync(int userId);

        Task<AppointmentStatusResponseDTO> UpdateAppointmentStatusAsync(UpdateAppointmentStatusDTO request);
    }
}
