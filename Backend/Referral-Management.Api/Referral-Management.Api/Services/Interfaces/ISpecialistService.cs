using Referral_Management.Api.DTOs;

public interface ISpecialistService
{
    Task<List<SpecialistPatientDto>> GetAssignedPatients(int specialistId);

    Task<int> CreateDraftReferral(
    int specialistId,
    ReferralIntakeCreateDto dto
);
    Task<IEnumerable<GetUrgencyLevelsDto>> GetAllUrgenciesAsync();   // new method
    Task<IEnumerable<GetSpecialityDto>> GetAllSpecialitiesAsync();

}