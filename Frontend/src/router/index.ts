import { createRouter, createWebHistory } from "vue-router";

import Login from "../pages/auth/Login.vue";
import AdminDashboard from "../pages/admin/AdminDashboard.vue";
import PatientDashboard from "../pages/patient/PatientDashboard.vue";
import CoordinatorDashboard from "../pages/referral-coordinator/CoordinatorDashboard.vue";
import SpecialistDashboard from "../pages/specialist/SpecialistDashboard.vue";
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
      path: "/coordinator",
      component: CoordinatorDashboard,
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
      path: "/specialist",
      component: SpecialistDashboard,
      meta: {
        requiresAuth: true,
        roles: ["4"],
      },
    },
  ],
});

export default router;
