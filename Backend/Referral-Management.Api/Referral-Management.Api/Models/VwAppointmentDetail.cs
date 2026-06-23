using System;
using System.Collections.Generic;

namespace Referral_Management.Api.Models;

public partial class VwAppointmentDetail
{
    public int AppointmentId { get; set; }

    public int ReferralId { get; set; }

    public int PatientId { get; set; }

    public int SpecialistId { get; set; }

    public DateOnly AppointmentDate { get; set; }

    public TimeOnly AppointmentTime { get; set; }

    public string AppointmentStatus { get; set; } = null!;

    public string PatientName { get; set; } = null!;

    public string Mrn { get; set; } = null!;

    public string SpecialistName { get; set; } = null!;

    public string ReferralReason { get; set; } = null!;
}
