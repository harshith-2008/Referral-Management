namespace Referral_Management.Api.DTOs;

public class ReferralDto
{
    public int ReferralId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public string OriginFacility { get; set; } = string.Empty;
    public string DestinationFacility { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Urgency { get; set; } = string.Empty;
    public string Specialty { get; set; } = string.Empty;
    public string? DiagnosisCode { get; set; }
    public DateTime CreatedAt { get; set; }
}