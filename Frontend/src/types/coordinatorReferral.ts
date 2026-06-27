export type CoordinatorReferralStatus =
  | "Submitted"
  | "Requested"
  | "Accepted"
  | "Rejected"
  | "Completed"
  | "Closed";

export type ReferralUrgency = "Urgent" | "Routine" | "Emergency";

export interface ReferralHistoryEvent {
  status: CoordinatorReferralStatus;
  title: string;
  description: string;
  timestamp: string;
}

export interface CoordinatorReferral {
  id: string;
  patientName: string;
  hospitalBranch: string;
  urgency: ReferralUrgency;
  assignedSpecialist: string;
  status: CoordinatorReferralStatus;
  date: string;
  dob: string;
  mrn: string;
  insurance: string;
  originSpecialist: string;
  destinationHospital: string;
  primaryDiagnosis: string;
  referralReason: string;
  history: ReferralHistoryEvent[];
}

export const referralStatusDefinitions: Record<
  CoordinatorReferralStatus,
  { label: string; description: string }
> = {
  Submitted: {
    label: "Submitted",
    description: "Specialist created the referral.",
  },
  Requested: {
    label: "Requested",
    description: "Origin specialist sent the referral to the destination hospital.",
  },
  Accepted: {
    label: "Accepted",
    description: "Destination hospital accepted the referral and assigned a docket.",
  },
  Rejected: {
    label: "Rejected",
    description: "Destination hospital declined the referral.",
  },
  Completed: {
    label: "Completed",
    description: "Specialist completed the appointment for this referral.",
  },
  Closed: {
    label: "Closed",
    description: "Referral was closed after another facility was selected.",
  },
};

export const referralStatusOrder: CoordinatorReferralStatus[] = [
  "Submitted",
  "Requested",
  "Accepted",
  "Rejected",
  "Completed",
  "Closed",
];
