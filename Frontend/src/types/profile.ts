export interface UserProfile {
  userId: number;

  firstName: string;
  lastName: string;

  email: string;
  phone: string;

  role: string;

  facilityId: number;
  facilityName?: string;

  patientId?: number;
  specialistId?: number;
  referralCoordinatorId?: number;
  adminId?: number;
}
