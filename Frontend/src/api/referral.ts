import api from "./axios";

import type {
  ReferralIntakeCreateDTO,
  GetUrgencyLevelDTO,
  GetSpecialityDTO,
  ReferralIntakeResponseDTO,
} from "../types/referral";

export const createReferralIntake = (
  specialistId: number,
  data: ReferralIntakeCreateDTO,
) =>
  api.post<ReferralIntakeResponseDTO>(
    `/specialist/referral-intake/${specialistId}`,
    data,
  );

export const getUrgencyLevels = () =>
  api.get<GetUrgencyLevelDTO[]>("/specialist/urgencyLevels");

export const getSpecialities = () =>
  api.get<GetSpecialityDTO[]>("/specialist/specialities");
