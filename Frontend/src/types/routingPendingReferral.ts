import type {
  CoordinatorReferralStatus,
  ReferralUrgency,
} from "./coordinatorReferral";

export interface RoutingPendingReferral {
  referralId: number;
  patientName: string;
  originFacility: string;
  destinationFacility: string;
  status: CoordinatorReferralStatus;
  urgency: ReferralUrgency;
  specialty: string;
  diagnosisCode: string;
}
