namespace Referral_Management.Api.DTOs;

public class SpecialistPatientDto
{
    public int ReferralId { get; set; }

    // Patient
    public int PatientId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public int Age { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string Mrn { get; set; } = string.Empty;

    // Referral Context
    public string Specialty { get; set; } = string.Empty;
    public string Urgency { get; set; } = string.Empty;
    public string? DiagnosisCode { get; set; }

    // Appointment
    public DateTime? AppointmentDate { get; set; }
}