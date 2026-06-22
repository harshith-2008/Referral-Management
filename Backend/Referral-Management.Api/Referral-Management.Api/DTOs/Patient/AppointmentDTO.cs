namespace Referral_Management.Api.DTOs.Patient
{
    public class AppointmentDto
    {
        public int AppointmentId { get; set; }

        public DateOnly AppointmentDate { get; set; }

        public TimeOnly AppointmentTime { get; set; }

        public string SpecialistName { get; set; } = string.Empty;

        public string AppointmentStatus { get; set; } = string.Empty;
    }
}