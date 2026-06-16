using System;
using System.Collections.Generic;

namespace Referral_Management.Api.Models;

public partial class ReferralAssignment
{
    public int ReferralAssignmentId { get; set; }

    public int ReferralId { get; set; }

    public int AssignedBy { get; set; }

    public int? FromSpecialistId { get; set; }

    public int ToSpecialistId { get; set; }

    public string? Reason { get; set; }

    public DateTime ReassignedAt { get; set; }

    public virtual ReferralCoordinator AssignedByNavigation { get; set; } = null!;

    public virtual Specialist? FromSpecialist { get; set; }

    public virtual Referral Referral { get; set; } = null!;

    public virtual Specialist ToSpecialist { get; set; } = null!;
}
