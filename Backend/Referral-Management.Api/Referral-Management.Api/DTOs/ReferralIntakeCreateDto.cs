namespace Referral_Management.Api.DTOs;

public class ReferralIntakeCreateDto
{
    public int PatientId { get; set; }
    public string ReferralReason { get; set; } = string.Empty;
    public string? DiagnosisCode { get; set; }
    public int UrgencyLevelId { get; set; }
    public int SpecialtyRequestId { get; set; }
}