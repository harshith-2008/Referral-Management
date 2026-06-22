namespace Referral_Management.Api.DTOs.Patient
{
    public class PatientProfileDto
    {
        public int PatientId { get; set; }

        public string Mrn { get; set; } = string.Empty;

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public DateOnly Dob { get; set; }

        public string Gender { get; set; } = string.Empty;

        public string InsuranceProviderName { get; set; } = string.Empty;

        public string InsuranceStatus { get; set; } = string.Empty;

        public string PatientAddress { get; set; } = string.Empty;

        public string FacilityName { get; set; } = string.Empty;

        public bool Status { get; set; }
    }
}