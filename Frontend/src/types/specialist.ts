export interface ReferralIntakeCreateDTO {
  patientId: number;
  referralReason: string;
  diagnosisCode?: string;
  urgencyLevelId: number;
  specialtyRequestId: number;
}

export interface SpecialistPatientDTO {
  referralId: number;

  patientId: number;
  patientName: string;

  age: number;
  gender: string;
  mrn: string;

  specialty: string;
  urgency: string;
  diagnosisCode?: string;

  appointmentDate?: string;
}

export interface SpecialistMatchDTO {
  specialistId: number;
  specialistName: string;
  shiftBlock: string;
}

export interface GetUrgencyLevelDTO {
  urgencyLevelId: number;
  levelName: string;
}

export interface GetSpecialityDTO {
  specialityId: number;
  specialityName: string;
}
