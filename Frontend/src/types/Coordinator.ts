import type { ReferralDTO } from "./referral";

export interface CoordinatorDashboardDTO {
  totalReferrals: number;
  submitted: number;
  requested: number;
  accepted: number;
  rejected: number;
  closed: number;

  recentReferrals: ReferralDTO[];
}
