using System;
using System.Collections.Generic;

namespace Referral_Management.Api.Models;

public partial class Patient
{
    public int PatientId { get; set; }

    public string Mrn { get; set; } = null!;

    public int UserId { get; set; }

    public int PrimaryFacilityId { get; set; }

    public DateOnly Dob { get; set; }

    public string Gender { get; set; } = null!;

    public string? InsuranceProviderName { get; set; }

    public string? InsuranceStatus { get; set; }

    public string PatientAddress { get; set; } = null!;

    public bool Status { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual Facility PrimaryFacility { get; set; } = null!;

    public virtual ICollection<Referral> Referrals { get; set; } = new List<Referral>();

    public virtual User User { get; set; } = null!;
}
