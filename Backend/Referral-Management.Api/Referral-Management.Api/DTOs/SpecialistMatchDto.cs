namespace Referral_Management.Api.DTOs;

public class SpecialistMatchDto
{
    public int SpecialistId { get; set; }
    public string SpecialistName { get; set; } = string.Empty;
    public string ShiftBlock { get; set; } = string.Empty;
}