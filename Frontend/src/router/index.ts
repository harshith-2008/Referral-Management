import { createRouter, createWebHistory } from "vue-router";
import { isAuthenticated, getUserRole } from "../utils/auth";

// Pages
import Login from "../pages/auth/Login.vue";

import AdminDashboard from "../pages/admin/AdminDashboard.vue";

import PatientDashboard from "../pages/patient/PatientDashboard.vue";
import PatientReferrals from "../pages/patient/MyReferrals.vue";

import CoordinatorDashboard from "../pages/referral-coordinator/CoordinatorDashboard.vue";
import ReferralList from "../pages/referral-coordinator/ReferralList.vue";

import SpecialistDashboard from "../pages/specialist/SpecialistDashboard.vue";
import AssignedReferrals from "../pages/specialist/AssignedReferrals.vue";
import CreateReferral from "../pages/specialist/CreateReferral.vue";
import Unauthorized from "../pages/auth/Unauthorized.vue";
import IncomingRequests from "../pages/referral-coordinator/IncomingRequests.vue";
import RoutingPending from "../pages/referral-coordinator/RoutingPending.vue";
import UsersPage from "../pages/admin/UsersPage.vue";
import ReportsPage from "../pages/admin/ReportsPage.vue";
import SpecialistAppointment from "../pages/specialist/SpecialistAppointment.vue";
import MyReferrals from "../pages/specialist/MyReferrals.vue";

import ProfileSettingsPage from "../pages/shared/ProfileSettingsPage.vue";

const getHomeRouteForRole = (role: string | null) => {
  switch (role) {
    case "Patient":
      return "/patient";
    case "ReferralCoordinator":
      return "/coordinator";
    case "Specialist":
      return "/specialist";
    case "Admin":
      return "/admin";
    default:
      return "/login";
  }
};

const router = createRouter({
  history: createWebHistory(),

  routes: [
    {
      path: "/",
      redirect: () =>
        isAuthenticated() ? getHomeRouteForRole(getUserRole()) : "/login",
    },

    // ================= AUTH =================
    {
      path: "/login",
      component: Login,
    },
    {
      path: "/unauthorized",
      component: Unauthorized,
    },

    // ================= ADMIN =================
    {
      path: "/admin",
      component: AdminDashboard,
      meta: {
        requiresAuth: true,
        roles: ["Admin"],
      },
    },
    {
      path: "/admin/reports",
      component: ReportsPage,
      meta: {
        requiresAuth: true,
        roles: ["Admin"],
      },
    },
    {
      path: "/admin/users",
      component: UsersPage,
      meta: {
        requiresAuth: true,
        roles: ["Admin"],
      },
    },
    {
      path: "/admin/profile",
      component: ProfileSettingsPage,
      meta: {
        requiresAuth: true,
        roles: ["Admin"],
      },
    },

    // ================= PATIENT =================
    {
      path: "/patient",
      component: PatientDashboard,
      meta: {
        requiresAuth: true,
        roles: ["Patient"],
      },
    },
    {
      path: "/patient/referrals",
      component: PatientReferrals,
      meta: {
        requiresAuth: true,
        roles: ["Patient"],
      },
    },
    {
      path: "/patient/profile",
      component: ProfileSettingsPage,
      meta: {
        requiresAuth: true,
        roles: ["Patient"],
      },
    },

    // ================= COORDINATOR =================
    {
      path: "/coordinator",
      component: CoordinatorDashboard,
      meta: {
        requiresAuth: true,
        roles: ["ReferralCoordinator"],
      },
    },
    {
      path: "/coordinator/referrals",
      component: ReferralList,
      meta: {
        requiresAuth: true,
        roles: ["ReferralCoordinator"],
      },
    },
    {
      path: "/coordinator/incoming-requests",
      component: IncomingRequests,
      meta: {
        requiresAuth: true,
        roles: ["ReferralCoordinator"],
      },
    },
    {
      path: "/coordinator/routing-pending",
      component: RoutingPending,
      meta: {
        requiresAuth: true,
        roles: ["ReferralCoordinator"],
      },
    },
    {
      path: "/coordinator/profile",
      component: ProfileSettingsPage,
      meta: {
        requiresAuth: true,
        roles: ["ReferralCoordinator"],
      },
    },

    // ================= SPECIALIST =================
    {
      path: "/specialist",
      component: SpecialistDashboard,
      meta: {
        requiresAuth: true,
        roles: ["Specialist"],
      },
    },
    {
      path: "/specialist/appointments",
      component: SpecialistAppointment,
      meta: {
        requiresAuth: true,
        roles: ["Specialist"],
      },
    },
    {
      path: "/specialist/my-referrals",
      component: MyReferrals,
      meta: {
        requiresAuth: true,
        roles: ["Specialist"],
      },
    },
    {
      path: "/specialist/create-referral",
      component: CreateReferral,
      meta: {
        requiresAuth: true,
        roles: ["Specialist"],
      },
    },
    {
      path: "/specialist/referrals",
      component: AssignedReferrals,
      meta: {
        requiresAuth: true,
        roles: ["Specialist"],
      },
    },
    {
      path: "/specialist/profile",
      component: ProfileSettingsPage,
      meta: {
        requiresAuth: true,
        roles: ["Specialist"],
      },
    },
  ],
});

router.beforeEach((to, _from, next) => {
  const requiresAuth = to.meta.requiresAuth as boolean;
  const allowedRoles = to.meta.roles as string[] | undefined;

  const auth = isAuthenticated();
  const role = getUserRole();

  if (to.path === "/login" && auth) {
    return next(getHomeRouteForRole(role));
  }

  // 1. Public route → allow
  if (!requiresAuth) {
    return next();
  }

  // 2. Not logged in → login
  if (!auth) {
    return next("/login");
  }

  // 3. Role check
  if (allowedRoles && allowedRoles.length > 0) {
    if (!role || !allowedRoles.includes(role)) {
      return next("/unauthorized");
    }
  }

  return next();
});

export default router;
