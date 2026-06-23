import api from "./axios";

import type { ApiResponseDTO } from "../types/common";

import type {
  AvailableSlotsResponseDTO,
  DailyScheduleResponseDTO,
  CreateAppointmentDTO,
  AppointmentResponseDTO,
  AppointmentDetailsDTO,
  UserAppointmentDTO,
  UpdateAppointmentStatusDTO,
  AppointmentStatusResponseDTO,
} from "../types/appointment";

export const getAvailableSlots = (specialistId: number, date: string) =>
  api.get<ApiResponseDTO<AvailableSlotsResponseDTO>>(
    `/appointments/available-slots/${specialistId}/${date}`
  );

export const getSchedule = (date: string) =>
  api.get<ApiResponseDTO<DailyScheduleResponseDTO>>(
    `/appointments/schedule/${date}`
  );

export const createAppointment = (data: CreateAppointmentDTO) =>
  api.post<ApiResponseDTO<AppointmentResponseDTO>>("/appointments", data);

export const getAppointmentDetails = (appointmentId: number) =>
  api.get<ApiResponseDTO<AppointmentDetailsDTO>>(
    `/appointments/${appointmentId}`
  );

export const getUserAppointments = () =>
  api.get<ApiResponseDTO<UserAppointmentDTO[]>>("/appointments/user");

export const updateAppointmentStatus = (data: UpdateAppointmentStatusDTO) =>
  api.put<ApiResponseDTO<AppointmentStatusResponseDTO>>(
    "/appointments/update-status",
    data
  );
