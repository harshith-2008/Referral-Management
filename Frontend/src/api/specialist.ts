import api from "./axios";

import type {
  ReferralIntakeCreateDTO,
  GetUrgencyLevelDTO,
  GetSpecialityDTO,
  SpecialistPatientDTO,
} from "../types/specialist";

import type { ApiResponseDTO } from "../types/common";

export const getAssignedPatients = () =>
  api.get<ApiResponseDTO<SpecialistPatientDTO[]>>(
    `/specialist/assigned-patients`,
  );

export const createReferralIntake = (data: ReferralIntakeCreateDTO) =>
  api.post<ApiResponseDTO<number>>(`/specialist/referral-intake`, data);

export const getUrgencyLevels = () =>
  api.get<GetUrgencyLevelDTO[]>("/specialist/urgencyLevels");

export const getSpecialities = () =>
  api.get<GetSpecialityDTO[]>("/specialist/specialities");
