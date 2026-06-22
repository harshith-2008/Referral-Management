export interface AdminDashboardDTO {
  totalUsers: number;
  totalPatients: number;
  totalSpecialists: number;
  totalReferrals: number;
  pendingReferrals: number;
  completedReferrals: number;
  cancelledReferrals: number;
  appointmentsToday: number;
}

export interface AppointmentAnalyticsDTO {
  totalAppointments: number;
  upcoming: number;
  completed: number;
  missed: number;
}

export interface DailyReferralDTO {
  date: string;
  count: number;
}

export interface FacilityLeakageDTO {
  facilityName: string;
  totalReferrals: number;
  leakageCount: number;
}

export interface SpecialtyAnalyticsDTO {
  specialtyId: number;
  specialtyName: string;
  count: number;
}

export interface OverviewAnalyticsDTO {
  totalReferrals: number;
  totalFacilities: number;
  totalPatients: number;
  totalSpecialists: number;

  completedReferrals: number;
  pendingReferrals: number;
  rejectedReferrals: number;

  leakagePercentage: number;

  topSpecialties: SpecialtyAnalyticsDTO[];
}

export interface ReferralLeakageDTO {
  totalReferrals: number;
  leakageCount: number;
  leakagePercentage: number;
  noAppointment: number;
  delayedAppointments: number;
  neverCompleted: number;
}

export interface SpecialtyLoadDTO {
  specialty: string;
  referralCount: number;
}

export interface StatusCountDTO {
  status: string;
  count: number;
}
