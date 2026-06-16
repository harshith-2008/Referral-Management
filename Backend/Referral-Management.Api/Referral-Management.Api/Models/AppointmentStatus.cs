using System;
using System.Collections.Generic;

namespace Referral_Management.Api.Models;

public partial class AppointmentStatus
{
    public int AppointmentStatusId { get; set; }

    public string StatusName { get; set; } = null!;

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}
