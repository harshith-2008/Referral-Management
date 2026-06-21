export type NavIcon =
  | "dashboard"
  | "referrals"
  | "settings"
  | "profile"
  | "create"
  | "directory"
  | "notifications"
  | "patients"
  | "users"
  | "reports";

export interface NavLink {
  label: string;
  path: string;
  icon: NavIcon;
}

export interface SidebarUser {
  name: string;
  role: string;
  initials: string;
}
