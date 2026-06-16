using System;
using System.Collections.Generic;

namespace Referral_Management.Api.Models;

public partial class Specialist
{
    public int SpecialistId { get; set; }

    public int UserId { get; set; }

    public int FacilityId { get; set; }

    public bool Status { get; set; }

    public int? ShiftBlockId { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual Facility Facility { get; set; } = null!;

    public virtual ICollection<ReferralAssignment> ReferralAssignmentFromSpecialists { get; set; } = new List<ReferralAssignment>();

    public virtual ICollection<ReferralAssignment> ReferralAssignmentToSpecialists { get; set; } = new List<ReferralAssignment>();

    public virtual ICollection<Referral> Referrals { get; set; } = new List<Referral>();

    public virtual ShiftBlock? ShiftBlock { get; set; }

    public virtual ICollection<SpecialistSpeciality> SpecialistSpecialities { get; set; } = new List<SpecialistSpeciality>();

    public virtual User User { get; set; } = null!;
}
