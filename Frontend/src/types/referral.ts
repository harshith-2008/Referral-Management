export interface FacilityDTO {
  facilityId: number;
  facilityName: string;
  availableSpecialists: number;
  hospitalId: number;
  hospitalName: string;
  inNetwork: boolean;
  city?: string | null;
  state?: string | null;
  distanceKm?: number | null;
}

export interface FacilitiesDropdownResponseDTO {
  inNetwork: FacilityDTO[];
  outOfNetwork: FacilityDTO[];
}

export interface ReferralDTO {
  referralId: number;

  patientName: string;

  originFacility: string;
  destinationFacility: string;

  status: string;
  urgency: string;
  specialty: string;

  diagnosisCode?: string;

  createdAt: string;
}

export interface ReferralDetailDTO {
  referralId: number;

  status: string;
  urgency: string;
  specialty: string;

  diagnosisCode?: string;

  createdAt: string;

  patientName: string;

  age: number;
  gender: string;

  mrn: string;

  primaryFacility: string;
}

export interface CreateReferralRequestDTO {
  referralId: number;

  patientId: number;

  originFacilityId: number;

  destinationFacilityIds: number[];

  specialtyRequestId: number;

  referralReason: string;

  diagnosisCode?: string;

  urgencyLevelId: number;

  referralStatusId: number;
}

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
