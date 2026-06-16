using System;
using System.Collections.Generic;

namespace Referral_Management.Api.Models;

public partial class Facility
{
    public int FacilityId { get; set; }

    public int HospitalId { get; set; }

    public string FacilityName { get; set; } = null!;

    public string? City { get; set; }

    public string? Address { get; set; }

    public string? State { get; set; }

    public string? PhoneNumber { get; set; }

    public decimal? Latitude { get; set; }

    public decimal? Longitude { get; set; }

    public bool Status { get; set; }

    public virtual Hospital Hospital { get; set; } = null!;

    public virtual ICollection<Patient> Patients { get; set; } = new List<Patient>();

    public virtual ICollection<ReferralCoordinator> ReferralCoordinators { get; set; } = new List<ReferralCoordinator>();

    public virtual ICollection<Referral> ReferralDestinationFacilities { get; set; } = new List<Referral>();

    public virtual ICollection<Referral> ReferralOriginFacilities { get; set; } = new List<Referral>();

    public virtual ICollection<Specialist> Specialists { get; set; } = new List<Specialist>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
