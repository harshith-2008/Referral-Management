public class AppointmentScheduleDTO
{
    public int AppointmentId { get; set; }

    public TimeOnly AppointmentTime { get; set; }

    public string PatientName { get; set; } = string.Empty;

    public string Mrn { get; set; } = string.Empty;

    public string AppointmentStatus { get; set; } = string.Empty;
}