using System;
using System.Collections.Generic;

namespace Referral_Management.Api.Models;

public partial class SpecialistSpeciality
{
    public int Id { get; set; }

    public int SpecialtyId { get; set; }

    public int SpecialistId { get; set; }

    public bool IsPrimary { get; set; }

    public virtual Specialist Specialist { get; set; } = null!;

    public virtual Specialty Specialty { get; set; } = null!;
}
