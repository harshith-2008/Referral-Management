public class CreateAppointmentDTO
{
    public int ReferralId { get; set; }

    public int SpecialistId { get; set; }

    public DateOnly AppointmentDate { get; set; }

    public TimeOnly AppointmentTime { get; set; }
}