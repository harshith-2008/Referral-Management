using System;
using System.Collections.Generic;

namespace Referral_Management.Api.Models;

public partial class Hospital
{
    public int HospitalId { get; set; }

    public string HospitalName { get; set; } = null!;

    public string HeadOfficeAddress { get; set; } = null!;

    public int? GlobalNetworkId { get; set; }

    public decimal? Latitude { get; set; }

    public decimal? Longitude { get; set; }

    public virtual ICollection<Facility> Facilities { get; set; } = new List<Facility>();

    public virtual GlobalNetwork? GlobalNetwork { get; set; }
}
