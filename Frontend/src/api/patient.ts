import api from "./axios";

import type { ApiResponseDTO } from "../types/common";

import type {
  PatientLookupDTO,
  UpdatePatientProfileDTO,
  PatientDashboardDTO,
  PatientProfileDTO,
  ReferralDTO,
  ReferralDetailsDTO,
  ReferralStatusDTO,
  AppointmentDTO,
  AppointmentDetailsDTO,
} from "../types/patient";

export const getPatientByMrn = (mrn: string) =>
  api.get<ApiResponseDTO<PatientLookupDTO>>(`/patient/lookup/${mrn}`);

export const getDashboard = () =>
  api.get<ApiResponseDTO<PatientDashboardDTO>>("/patient/dashboard");

export const getProfile = () =>
  api.get<ApiResponseDTO<PatientProfileDTO>>("/patient/profile");

export const updateProfile = (data: UpdatePatientProfileDTO) =>
  api.put<ApiResponseDTO<boolean>>("/patient/profile", data);

export const getReferrals = () =>
  api.get<ApiResponseDTO<ReferralDTO[]>>("/patient/referrals");

export const getReferral = (referralId: number) =>
  api.get<ApiResponseDTO<ReferralDetailsDTO>>(
    `/patient/referrals/${referralId}`,
  );

export const getReferralStatus = (referralId: number) =>
  api.get<ApiResponseDTO<ReferralStatusDTO>>(
    `/patient/referrals/${referralId}/status`,
  );

export const getAppointments = () =>
  api.get<ApiResponseDTO<AppointmentDTO[]>>("/patient/appointments");

export const getUpcomingAppointments = () =>
  api.get<ApiResponseDTO<AppointmentDTO[]>>("/patient/appointments/upcoming");

export const getAppointmentHistory = () =>
  api.get<ApiResponseDTO<AppointmentDTO[]>>("/patient/appointments/history");

export const getAppointment = (appointmentId: number) =>
  api.get<ApiResponseDTO<AppointmentDetailsDTO>>(
    `/patient/appointments/${appointmentId}`,
  );
