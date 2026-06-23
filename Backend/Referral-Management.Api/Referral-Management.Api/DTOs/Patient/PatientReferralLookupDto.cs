public class PatientReferralLookupDto
{
    public int PatientId { get; set; }

    public string Mrn { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public DateOnly Dob { get; set; }

    public string Gender { get; set; } = string.Empty;

    public string FacilityName { get; set; } = string.Empty;
}