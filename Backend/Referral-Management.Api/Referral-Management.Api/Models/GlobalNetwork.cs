using System;
using System.Collections.Generic;

namespace Referral_Management.Api.Models;

public partial class GlobalNetwork
{
    public int Id { get; set; }

    public string NetworkName { get; set; } = null!;

    public virtual ICollection<Hospital> Hospitals { get; set; } = new List<Hospital>();
}
