public class DailyScheduleResponseDTO
{
    public DateOnly Date { get; set; }

    public List<AppointmentScheduleDTO> Appointments { get; set; } = [];
}