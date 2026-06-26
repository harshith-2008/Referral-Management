namespace Referral_Management.Api.DTOs;

public class FacilityDropdownDto
{
    public int FacilityId { get; set; }

    public string FacilityName { get; set; } = string.Empty;

    // Number of available specialists for the requested specialty
    public int AvailableSpecialists { get; set; }

    // Hospital info for clarity
    public int HospitalId { get; set; }

    public string HospitalName { get; set; } = string.Empty;

    // True if this facility is in-network (same hospital as the referral's origin hospital)
    public bool InNetwork { get; set; }

    public string? City { get; set; }

    public string? State { get; set; }

    // Great-circle distance from the routing coordinator's facility, in kilometres.
    // Null when either facility does not have coordinates.
    public double? DistanceKm { get; set; }
}
