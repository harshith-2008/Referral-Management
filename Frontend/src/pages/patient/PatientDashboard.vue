<script setup lang="ts">
import { onMounted, ref, computed } from "vue";

import DashboardLayout from "../../components/layout/DashboardLayout.vue";
import StatsCards from "../../components/specialist/StatsCards.vue";

import { patientNavLinks } from "../../config/navigation";
import { getDashboard } from "../../api/patient";

import type { PatientDashboardDTO } from "../../types/patient";
import type { StatCardItem } from "../../components/specialist/StatsCards.vue";

/* ---------------- USER STATE ---------------- */
const user = ref({
  name: "",
  welcomeName: "",
  role: "Patient",
  initials: "",
});

/* ---------------- DASHBOARD STATE ---------------- */
const dashboard = ref<PatientDashboardDTO | null>(null);

/* ---------------- LOAD DASHBOARD ---------------- */
const loadDashboard = async () => {
  try {
    const response = await getDashboard();

    const payload = response?.data?.data ?? response?.data ?? response;

    dashboard.value = payload;

    if (dashboard.value?.profile) {
      const profile = dashboard.value.profile;

      user.value.name = `${profile.firstName} ${profile.lastName}`;
      user.value.welcomeName = profile.firstName;
      user.value.initials = `${profile.firstName.charAt(0)}${profile.lastName.charAt(0)}`;
    }
  } catch (error) {
    console.error("Error loading dashboard:", error);
  }
};

/* ---------------- STATS ---------------- */
const stats = computed<StatCardItem[]>(() => [
  {
    label: "Total Referrals",
    value: Number(dashboard.value?.totalReferrals ?? 0),
    icon: "clipboard",
    iconBg: "bg-blue-50",
    iconColor: "text-blue-600",
  },
  {
    label: "Pending Referrals",
    value: Number(dashboard.value?.pendingReferrals ?? 0),
    icon: "clock",
    iconBg: "bg-amber-50",
    iconColor: "text-amber-600",
  },
  {
    label: "Completed Referrals",
    value: Number(dashboard.value?.completedReferrals ?? 0),
    icon: "check",
    iconBg: "bg-green-50",
    iconColor: "text-green-600",
  },
  {
    label: "Upcoming Appointments",
    value: Number(dashboard.value?.upcomingAppointments ?? 0),
    icon: "calendar",
    iconBg: "bg-purple-50",
    iconColor: "text-purple-600",
  },
]);

onMounted(loadDashboard);
</script>

<template>
  <DashboardLayout
    :nav-links="patientNavLinks"
    title="Dashboard"
    :subtitle="`Welcome back, ${user.welcomeName}`"
    :notification-count="1"
  >
    <div class="space-y-6">
      <!-- ✅ STATS -->
      <StatsCards :items="stats" :columns="5" />

      <!-- ✅ UPCOMING APPOINTMENTS (MyReferrals TABLE UI) -->
      <div class="rounded-xl border border-slate-100 bg-white shadow-sm">
        <div class="overflow-hidden">
          <table class="w-full">
            <thead>
              <tr class="border-b border-slate-100 bg-slate-50/50">
                <th
                  class="px-6 py-3 text-left text-sm font-bold uppercase tracking-wide text-slate-700"
                >
                  Appointment Date
                </th>
                <th
                  class="px-6 py-3 text-left text-sm font-bold uppercase tracking-wide text-slate-700"
                >
                  Time
                </th>
                <th
                  class="px-6 py-3 text-left text-sm font-bold uppercase tracking-wide text-slate-700"
                >
                  Specialist
                </th>
                <th
                  class="px-6 py-3 text-left text-sm font-bold uppercase tracking-wide text-slate-700"
                >
                  Status
                </th>
              </tr>
            </thead>

            <tbody>
              <tr
                v-for="appointment in dashboard?.upcomingAppointmentList ?? []"
                :key="appointment.appointmentId"
                class="border-b border-slate-100 last:border-b-0 transition-colors hover:bg-slate-50"
              >
                <td class="px-6 py-4 text-sm font-semibold text-slate-900">
                  {{
                    new Date(appointment.appointmentDate).toLocaleDateString()
                  }}
                </td>

                <td class="px-6 py-4 text-sm text-slate-600">
                  {{ appointment.appointmentTime }}
                </td>

                <td class="px-6 py-4 text-sm text-slate-600">
                  {{ appointment.specialistName ?? "-" }}
                </td>

                <td class="px-6 py-4 text-sm text-slate-600">
                  {{ appointment.appointmentStatus }}
                </td>
              </tr>

              <!-- Empty State -->
              <tr
                v-if="(dashboard?.upcomingAppointmentList?.length ?? 0) === 0"
              >
                <td
                  colspan="4"
                  class="px-6 py-8 text-center text-sm text-slate-500"
                >
                  No upcoming appointments.
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>
  </DashboardLayout>
</template>
