public class AppointmentDetailsDTO
{
    public int AppointmentId { get; set; }

    public int ReferralId { get; set; }

    public DateOnly AppointmentDate { get; set; }

    public TimeOnly AppointmentTime { get; set; }

    public string AppointmentStatus { get; set; } = string.Empty;

    public int PatientId { get; set; }

    public string PatientName { get; set; } = string.Empty;

    public string Mrn { get; set; } = string.Empty;

    public int SpecialistId { get; set; }

    public string SpecialistName { get; set; } = string.Empty;

    public string ReferralReason { get; set; } = string.Empty;
}