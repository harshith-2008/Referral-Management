using System;
using System.Collections.Generic;

namespace Referral_Management.Api.Models;

public partial class ShiftBlock
{
    public int ShiftBlockId { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public virtual ICollection<Specialist> Specialists { get; set; } = new List<Specialist>();
}
