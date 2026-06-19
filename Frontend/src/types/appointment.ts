export interface AppointmentDetailsDTO {
  appointmentId: number;
  referralId: number;
  appointmentDate: string;
  appointmentTime: string;
  appointmentStatus: string;
  patientId: number;
  patientName: string;
  mrn: string;
  specialistId: number;
  specialistName: string;
  referralReason: string;
}

export interface AppointmentResponseDTO {
  appointmentId: number;
  message: string;
}

export interface AppointmentScheduleDTO {
  appointmentId: number;
  appointmentTime: string;
  patientName: string;
  mrn: string;
  appointmentStatus: string;
}

export interface AppointmentStatusResponseDTO {
  appointmentId: number;
  status: string;
  message: string;
}

export interface AvailableSlotDTO {
  startTime: string;
  endTime: string;
}

export interface AvailableSlotsResponseDTO {
  specialistId: number;
  date: string;
  slots: AvailableSlotDTO[];
}

export interface CreateAppointmentDTO {
  referralId: number;
  specialistId: number;
  appointmentDate: string;
  appointmentTime: string;
}

export interface DailyScheduleResponseDTO {
  date: string;
  appointments: AppointmentScheduleDTO[];
}

export interface UpdateAppointmentStatusDTO {
  appointmentId: number;
  appointmentStatusId: number;
}

export interface UserAppointmentDTO {
  appointmentId: number;
  referralId: number;
  appointmentDate: string;
  appointmentTime: string;
  specialistName: string;
  appointmentStatus: string;
}
