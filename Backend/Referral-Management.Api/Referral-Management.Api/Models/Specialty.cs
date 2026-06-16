using System;
using System.Collections.Generic;

namespace Referral_Management.Api.Models;

public partial class Specialty
{
    public int SpecialtyId { get; set; }

    public string SpecialtyName { get; set; } = null!;

    public string Code { get; set; } = null!;

    public virtual ICollection<Referral> Referrals { get; set; } = new List<Referral>();

    public virtual ICollection<SpecialistSpeciality> SpecialistSpecialities { get; set; } = new List<SpecialistSpeciality>();
}
