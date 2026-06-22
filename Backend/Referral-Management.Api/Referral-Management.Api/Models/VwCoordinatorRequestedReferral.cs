using System;
using System.Collections.Generic;

namespace Referral_Management.Api.Models;

public partial class VwCoordinatorRequestedReferral
{
    public int ReferralId { get; set; }

    public int? DestinationFacilityId { get; set; }

    public int PatientId { get; set; }

    public string PatientName { get; set; } = null!;

    public string OriginFacility { get; set; } = null!;

    public string DestinationFacility { get; set; } = null!;

    public string Status { get; set; } = null!;

    public string Urgency { get; set; } = null!;

    public string Specialty { get; set; } = null!;

    public string? DiagnosisCode { get; set; }

    public DateTime? CreatedAt { get; set; }
}
