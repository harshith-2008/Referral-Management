export type ReferralUrgency = "Urgent" | "Routine" | "Emergency";

export type ReferralStatus =
  | "Submitted"
  | "requested"
  | "accepted"
  | "rejected"
  | "scheduled"
  | "closed"
  | "cancelled"
  | "Assigned";

export interface Referral {
  id: string;
  patientName: string;
  urgency: ReferralUrgency;
  status: ReferralStatus;
  date: string;
  reviewTitle: string;
  dob: string;
  mrn: string;
  insurance: string;
  primaryDiagnosis: string;
  referralReason: string;
}

export interface ReferralIntakeCreateDTO {
  patientId: number;
  referralReason: string;
  diagnosisCode?: string;
  urgencyLevelId: number;
  specialtyRequestId: number;
}

export interface GetUrgencyLevelDTO {
  urgencyLevelId: number;
  levelName: string;
}

export interface GetSpecialityDTO {
  specialityId: number;
  specialityName: string;
}

export interface ReferralIntakeResponseDTO {
  success: boolean;
  message: string;
  data: number; // ReferralId
}
