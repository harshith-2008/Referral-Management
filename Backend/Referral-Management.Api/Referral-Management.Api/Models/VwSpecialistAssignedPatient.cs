using System;
using System.Collections.Generic;

namespace Referral_Management.Api.Models;

public partial class VwSpecialistAssignedPatient
{
    public int SpecialistId { get; set; }

    public int ReferralId { get; set; }

    public int PatientId { get; set; }

    public string PatientName { get; set; } = null!;

    public string Mrn { get; set; } = null!;

    public string Gender { get; set; } = null!;

    public DateOnly Dob { get; set; }

    public string Specialty { get; set; } = null!;

    public string Urgency { get; set; } = null!;

    public string? DiagnosisCode { get; set; }

    public DateTime? CreatedAt { get; set; }
}
