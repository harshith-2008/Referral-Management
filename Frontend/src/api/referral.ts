import api from "./axios";

import type { ApiResponseDTO } from "../types/common";

import type {
  CreateReferralRequestDTO,
  ReferralDTO,
  ReferralDetailDTO,
  FacilityDTO,
} from "../types/referral";

import type { SpecialistMatchDTO } from "../types/specialist";

export const getRequestedReferrals = () =>
  api.get<ApiResponseDTO<ReferralDTO[]>>("/referral/requested");

export const getReferralDetails = (referralId: number) =>
  api.get<ApiResponseDTO<ReferralDetailDTO>>(`/referral/details/${referralId}`);

export const getMatchingSpecialists = (referralId: number) =>
  api.get<ApiResponseDTO<SpecialistMatchDTO[]>>(
    `/referral/specialists/${referralId}`,
  );

export const getFacilitiesDropdown = (referralId: number) =>
  api.get<FacilityDTO[]>(`/referral/${referralId}/facilities-dropdown`);

export const routeReferral = (data: CreateReferralRequestDTO) =>
  api.post<ApiResponseDTO<any>>("/referral/route", data);

export const getSubmittedPendingReferrals = () =>
  api.get(`/referral/submitted`);

export const getOriginFacilityReferrals = () =>
  api.get<ApiResponseDTO<ReferralDTO[]>>("/referral/origin-facility");

export const getMyReferrals = () => api.get("/referral/my-referrals");
