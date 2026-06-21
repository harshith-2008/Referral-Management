export type ReferralUrgency = "Urgent" | "Routine" | "Emergency";

export type ReferralStatus =
  | "Under Review"
  | "Scheduled"
  | "Pending"
  | "Accepted"
  | "Completed"
  | "Treatment Ongoing"
  | "Rejected"
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
