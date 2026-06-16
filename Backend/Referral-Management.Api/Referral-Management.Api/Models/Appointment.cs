using System;
using System.Collections.Generic;

namespace Referral_Management.Api.Models;

public partial class Appointment
{
    public int AppointmentId { get; set; }

    public int ReferralId { get; set; }

    public int SpecialistId { get; set; }

    public int PatientId { get; set; }

    public DateOnly AppointmentDate { get; set; }

    public TimeOnly AppointmentTime { get; set; }

    public int AppointmentStatusId { get; set; }

    public virtual AppointmentStatus AppointmentStatus { get; set; } = null!;

    public virtual Patient Patient { get; set; } = null!;

    public virtual Referral Referral { get; set; } = null!;

    public virtual Specialist Specialist { get; set; } = null!;
}
