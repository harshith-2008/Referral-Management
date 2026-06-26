namespace Referral_Management.Api.DTOs;

public class FacilitiesDropdownResponseDto
{
    // Facilities in the same hospital as the origin facility
    public List<FacilityDropdownDto> InNetwork { get; set; } = new();

    // Facilities in other hospitals (out-of-network)
    public List<FacilityDropdownDto> OutOfNetwork { get; set; } = new();

    // Optional convenience flattened list (backwards compatibility)
    public List<FacilityDropdownDto> All => InNetwork.Concat(OutOfNetwork).ToList();
}
