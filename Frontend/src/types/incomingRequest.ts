import type {
  CoordinatorReferralStatus,
  ReferralUrgency,
} from "./coordinatorReferral";

export interface IncomingRequest {
  referralId: number;

  patientName: string;

  originFacility: string;
  destinationFacility: string;

  specialty: string;
  urgency: ReferralUrgency;

  status: CoordinatorReferralStatus;
}
