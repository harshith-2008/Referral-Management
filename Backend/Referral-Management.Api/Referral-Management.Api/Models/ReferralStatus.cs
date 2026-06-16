using System;
using System.Collections.Generic;

namespace Referral_Management.Api.Models;

public partial class ReferralStatus
{
    public int ReferralStatusId { get; set; }

    public string StatusName { get; set; } = null!;

    public virtual ICollection<Referral> Referrals { get; set; } = new List<Referral>();
}
