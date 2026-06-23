import { createRouter, createWebHistory } from "vue-router";

import Login from "../pages/auth/Login.vue";
import AdminDashboard from "../pages/admin/AdminDashboard.vue";
import PatientDashboard from "../pages/patient/PatientDashboard.vue";
import CoordinatorDashboard from "../pages/referral-coordinator/CoordinatorDashboard.vue";
import ReferralList from "../pages/referral-coordinator/ReferralList.vue";
import SpecialistDashboard from "../pages/specialist/SpecialistDashboard.vue";
import AssignedReferrals from "../pages/specialist/AssignedReferrals.vue";
import CreateReferral from "../pages/specialist/CreateReferral.vue";
import ProfileSettingsPage from "../pages/shared/ProfileSettingsPage.vue";
import Unauthorized from "../pages/auth/Unauthorized.vue";

const router = createRouter({
  history: createWebHistory(),

  routes: [
    { path: "/login", component: Login },
    { path: "/unauthorized", component: Unauthorized },
    {
      path: "/admin",
      component: AdminDashboard,
      meta: {
        requiresAuth: true,
        roles: ["1"],
      },
    },
    {
      path: "/admin/profile",
      component: ProfileSettingsPage,
      meta: {
        requiresAuth: true,
        roles: ["1"],
      },
    },
    {
      path: "/coordinator",
      component: CoordinatorDashboard,
      meta: {
        requiresAuth: true,
        roles: ["2"],
      },
    },
    {
      path: "/coordinator/referrals",
      component: ReferralList,
      meta: {
        requiresAuth: true,
        roles: ["2"],
      },
    },
    {
      path: "/coordinator/profile",
      component: ProfileSettingsPage,
      meta: {
        requiresAuth: true,
        roles: ["2"],
      },
    },
    {
      path: "/patient",
      component: PatientDashboard,
      meta: {
        requiresAuth: true,
        roles: ["3"],
      },
    },
    {
      path: "/patient/profile",
      component: ProfileSettingsPage,
      meta: {
        requiresAuth: true,
        roles: ["3"],
      },
    },
    {
      path: "/specialist",
      component: SpecialistDashboard,
      meta: {
        requiresAuth: true,
        roles: ["4"],
      },
    },
    {
      path: "/specialist/create-referral",
      component: CreateReferral,
      meta: {
        requiresAuth: true,
        roles: ["4"],
      },
    },
    {
      path: "/specialist/referrals",
      component: AssignedReferrals,
      meta: {
        requiresAuth: true,
        roles: ["4"],
      },
    },
    {
      path: "/specialist/profile",
      component: ProfileSettingsPage,
      meta: {
        requiresAuth: true,
        roles: ["4"],
      },
    },
  ],
});

export default router;
