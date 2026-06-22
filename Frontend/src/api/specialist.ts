import api from "./axios";

import type {
  ReferralIntakeCreateDTO,
  GetUrgencyLevelDTO,
  GetSpecialityDTO,
  SpecialistPatientDTO,
} from "../types/specialist";

import type { ApiResponseDTO } from "../types/common";

export const getAssignedPatients = (specialistId: number) =>
  api.get<ApiResponseDTO<SpecialistPatientDTO[]>>(
    `/specialist/assigned-patients/${specialistId}`
  );

export const createReferralIntake = (
  specialistId: number,
  data: ReferralIntakeCreateDTO
) =>
  api.post<ApiResponseDTO<number>>(
    `/specialist/referral-intake/${specialistId}`,
    data
  );

export const getUrgencyLevels = () =>
  api.get<GetUrgencyLevelDTO[]>("/specialist/urgencyLevels");

export const getSpecialities = () =>
  api.get<GetSpecialityDTO[]>("/specialist/specialities");
