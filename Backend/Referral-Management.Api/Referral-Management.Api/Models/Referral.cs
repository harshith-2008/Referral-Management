using System;
using System.Collections.Generic;

namespace Referral_Management.Api.Models;

public partial class Referral
{
    public int ReferralId { get; set; }

    public int PatientId { get; set; }

    public int OriginFacilityId { get; set; }

    public int? DestinationFacilityId { get; set; }

    public int? CreatedByCoordinatorId { get; set; }

    public int FromSpecialistId { get; set; }

    public int SpecialtyRequestId { get; set; }

    public int UrgencyLevelId { get; set; }

    public int ReferralStatusId { get; set; }

    // New: group identifier for referrals created from the same routing action
    public Guid? ReferralGroupId { get; set; }

    public string ReferralReason { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? DiagnosisCode { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();

    public virtual ReferralCoordinator? CreatedByCoordinator { get; set; }

    public virtual Facility? DestinationFacility { get; set; }

    public virtual Specialist FromSpecialist { get; set; } = null!;

    public virtual Facility OriginFacility { get; set; } = null!;

    public virtual Patient Patient { get; set; } = null!;

    public virtual ICollection<ReferralAssignment> ReferralAssignments { get; set; } = new List<ReferralAssignment>();

    public virtual ReferralStatus ReferralStatus { get; set; } = null!;

    public virtual Specialty SpecialtyRequest { get; set; } = null!;

    public virtual UrgencyLevel UrgencyLevel { get; set; } = null!;
}
