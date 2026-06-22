public class AvailableSlotsResponseDTO
{
    public int SpecialistId { get; set; }

    public DateOnly Date { get; set; }

    public List<AvailableSlotDTO> Slots { get; set; } = [];
}