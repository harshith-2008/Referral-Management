import type { NavLink } from "../types/navigation";

export const specialistNavLinks: NavLink[] = [
  { label: "Dashboard", path: "/specialist", icon: "dashboard" },
  {
    label: "Create Referral",
    path: "/specialist/create-referral",
    icon: "create",
  },
  {
    label: "Assigned Referrals",
    path: "/specialist/referrals",
    icon: "referrals",
  },
  { label: "Profile", path: "/specialist/profile", icon: "profile" },
];

export const coordinatorNavLinks: NavLink[] = [
  { label: "Dashboard", path: "/coordinator", icon: "dashboard" },
  { label: "Referral List", path: "/coordinator/referrals", icon: "referrals" },
  {
    label: "Routing Pending",
    path: "/coordinator/routing-pending",
    icon: "referrals",
  },
  {
    label: "Incoming Requests",
    path: "/coordinator/incoming-requests",
    icon: "referrals",
  },
  { label: "Profile", path: "/coordinator/profile", icon: "profile" },
];

export const patientNavLinks: NavLink[] = [
  { label: "Dashboard", path: "/patient", icon: "dashboard" },
  { label: "My Referrals", path: "/patient/referrals", icon: "referrals" },
  { label: "Profile", path: "/patient/profile", icon: "profile" },
];

export const adminNavLinks: NavLink[] = [
  { label: "Dashboard", path: "/admin", icon: "dashboard" },
  { label: "Users", path: "/admin/users", icon: "users" },
  { label: "Reports", path: "/admin/reports", icon: "reports" },
  { label: "Profile", path: "/admin/profile", icon: "profile" },
];
