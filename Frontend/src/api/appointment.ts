import api from "./axios";

import type {
  CreateAppointmentDTO,
  UpdateAppointmentStatusDTO,
} from "../types/appointment";

export const getAvailableSlots = (specialistId: number, date: string) =>
  api.get(`/appointments/available-slots/${specialistId}/${date}`);

export const getSchedule = (date: string) =>
  api.get(`/appointments/schedule/${date}`);

export const createAppointment = (data: CreateAppointmentDTO) =>
  api.post("/appointments", data);

export const getAppointmentDetails = (appointmentId: number) =>
  api.get(`/appointments/${appointmentId}`);

export const getUserAppointments = () => api.get("/appointments/user");

export const updateAppointmentStatus = (data: UpdateAppointmentStatusDTO) =>
  api.put("/appointments/update-status", data);
