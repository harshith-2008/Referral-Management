public class CreateReferralRequest
{
    public int ReferralId { get; set; }
    public int PatientId { get; set; }
    public int OriginFacilityId { get; set; }
    public List<int> DestinationFacilityIds { get; set; } = new();  // multiple facilities
    public int CreatedByCoordinatorId { get; set; }
    
    public int FromSpecialistId { get; set; }
    public int SpecialtyRequestId { get; set; }
    public string ReferralReason { get; set; } = string.Empty;
    public string? DiagnosisCode { get; set; }
    public int UrgencyLevelId { get; set; }
    public int ReferralStatusId { get; set; }
}
