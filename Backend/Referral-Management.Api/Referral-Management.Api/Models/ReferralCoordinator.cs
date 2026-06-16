using System;
using System.Collections.Generic;

namespace Referral_Management.Api.Models;

public partial class ReferralCoordinator
{
    public int ReferralCoordinatorId { get; set; }

    public int UserId { get; set; }

    public int FacilityId { get; set; }

    public virtual Facility Facility { get; set; } = null!;

    public virtual ICollection<ReferralAssignment> ReferralAssignments { get; set; } = new List<ReferralAssignment>();

    public virtual ICollection<Referral> Referrals { get; set; } = new List<Referral>();

    public virtual User User { get; set; } = null!;
}
