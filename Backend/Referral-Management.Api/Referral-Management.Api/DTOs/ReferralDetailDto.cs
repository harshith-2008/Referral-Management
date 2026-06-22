namespace Referral_Management.Api.DTOs;

public class ReferralDetailDto
{
    // 🔹 Referral info (6)
    public int ReferralId { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Urgency { get; set; } = string.Empty;
    public string Specialty { get; set; } = string.Empty;
    public string? DiagnosisCode { get; set; }
    public DateTime CreatedAt { get; set; }

    // 🔹 Patient info (5)
    public string PatientName { get; set; } = string.Empty;
    public int Age { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string Mrn { get; set; } = string.Empty;
    public string PrimaryFacility { get; set; } = string.Empty;
}