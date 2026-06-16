using System;
using System.Collections.Generic;

namespace Referral_Management.Api.Models;

public partial class UrgencyLevel
{
    public int UrgencyLevelId { get; set; }

    public string LevelName { get; set; } = null!;

    public virtual ICollection<Referral> Referrals { get; set; } = new List<Referral>();
}
