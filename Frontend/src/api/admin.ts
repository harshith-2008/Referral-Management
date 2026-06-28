import api from "./axios";

import type {
  AdminDashboardDTO,
  ReferralLeakageDTO,
  StatusCountDTO,
  FacilityLeakageDTO,
  SpecialtyLoadDTO,
  AppointmentAnalyticsDTO,
  DailyReferralDTO,
  MonthlyReferralDTO,
  ReferralAgingDTO,
  ScheduledDelayDTO,
  TopSpecialistDTO,
  UserListDTO
} from "../types/admin";


// export const getUsers = () =>
//   api.get("/admin/users");

export const getUsers = () =>
  api.get<UserListDTO[]>("/admin/users");


export const getDashboard = () =>
  api.get<AdminDashboardDTO>("/admin/dashboard");

export const getReferralLeakage = () =>
  api.get<ReferralLeakageDTO>("/admin/analytics/referral-leakage");

export const getReferralStatus = () =>
  api.get<StatusCountDTO[]>("/admin/analytics/referral-status");

export const getFacilityLeakage = () =>
  api.get<FacilityLeakageDTO[]>("/admin/analytics/facility-leakage");

export const getSpecialtyLoad = () =>
  api.get<SpecialtyLoadDTO[]>("/admin/analytics/specialty-load");

export const getAppointmentAnalytics = () =>
  api.get<AppointmentAnalyticsDTO>("/admin/analytics/appointments");

export const getDailyReferrals = () =>
  api.get<DailyReferralDTO[]>("/admin/analytics/daily-referrals");

export const getMonthlyReferral = () =>
  api.get<MonthlyReferralDTO[]>("/admin/analytics/monthly-referral");

export const getTopSpecialists = () =>
  api.get<TopSpecialistDTO[]>("/admin/analytics/top-specialists");

export const getReferralAging = () =>
  api.get<ReferralAgingDTO>("/admin/analytics/referral-aging");

export const getScheduledDelays = () =>
  api.get<ScheduledDelayDTO>("/admin/analytics/scheduled-delays");



