export interface PatientLookupDTO {
  patientId: number;
  mrn: string;
  fullName: string;
  dob: string;
  gender: string;
  facilityName: string;
}

export interface UpdatePatientProfileDTO {
  phoneNumber: string;
  patientAddress: string;
  insuranceProviderName?: string;
  insuranceStatus?: string;
}

export interface PatientProfileDTO {
  patientId: number;

  mrn: string;

  firstName: string;
  lastName: string;

  email: string;
  phoneNumber: string;

  dob: string;

  gender: string;

  insuranceProviderName: string;
  insuranceStatus: string;

  patientAddress: string;

  facilityName: string;

  status: boolean;
}

export interface AppointmentDTO {
  appointmentId: number;

  appointmentDate: string;

  appointmentTime: string;

  specialistName: string;

  appointmentStatus: string;
}

export interface AppointmentDetailsDTO {
  appointmentId: number;

  appointmentDate: string;

  appointmentTime: string;

  specialistName: string;

  facilityName: string;

  appointmentStatus: string;

  specialty: string;
}

export interface ReferralDTO {
  referralId: number;

  specialty: string;

  referralStatus: string;

  urgency: string;

  originFacility: string;

  destinationFacility: string;

  createdAt: string;
}

export interface ReferralDetailsDTO {
  referralId: number;

  referralReason: string;

  diagnosisCode: string;

  specialty: string;

  referralStatus: string;

  urgency: string;

  originFacility: string;

  destinationFacility: string;

  createdAt: string;
}

export interface ReferralStatusDTO {
  referralId: number;

  referralStatus: string;

  lastUpdated: string;
}

export interface PatientDashboardDTO {
  profile: PatientProfileDTO;

  totalReferrals: number;

  pendingReferrals: number;

  completedReferrals: number;

  upcomingAppointments: number;

  upcomingAppointmentList: AppointmentDTO[];

  recentReferrals: ReferralDTO[];
}
