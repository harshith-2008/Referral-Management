import api from "./axios";

import type { UpdatePatientProfileDTO } from "../types/patient";

export const getPatientByMrn = (mrn: string) =>
  api.get(`/patient/lookup/${mrn}`);

export const getDashboard = () => api.get("/patient/dashboard");

export const getProfile = () => api.get("/patient/profile");

export const updateProfile = (data: UpdatePatientProfileDTO) =>
  api.put("/patient/profile", data);

export const getReferrals = () => api.get("/patient/referrals");

export const getReferral = (referralId: number) =>
  api.get(`/patient/referrals/${referralId}`);

export const getReferralStatus = (referralId: number) =>
  api.get(`/patient/referrals/${referralId}/status`);

export const getAppointments = () => api.get("/patient/appointments");

export const getUpcomingAppointments = () =>
  api.get("/patient/appointments/upcoming");

export const getAppointmentHistory = () =>
  api.get("/patient/appointments/history");

export const getAppointment = (appointmentId: number) =>
  api.get(`/patient/appointments/${appointmentId}`);
