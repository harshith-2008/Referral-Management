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

const formatDate = (date: string) =>
  new Date(`${date}T00:00:00`).toLocaleDateString(undefined, {
    weekday: "short",
    day: "numeric",
    month: "short",
    year: "numeric",
  });

const formatTime = (time: string) => time?.slice(0, 5) || "—";

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
        <div
          class="flex items-center justify-between border-b border-slate-100 px-6 py-4"
        >
          <div>
            <h2 class="text-base font-semibold text-slate-900">
              Upcoming appointments
            </h2>
            <p class="mt-0.5 text-sm text-slate-500">
              Your confirmed care schedule
            </p>
          </div>
          <span
            class="rounded-full bg-blue-50 px-3 py-1 text-xs font-semibold text-blue-700"
          >
            {{ dashboard?.upcomingAppointments ?? 0 }} upcoming
          </span>
        </div>
        <div class="overflow-x-auto">
          <table class="w-full">
            <thead>
              <tr class="border-b border-slate-100 bg-slate-50/50">
                <th
                  class="px-6 py-3 text-left text-sm font-bold uppercase tracking-wide text-slate-700"
                >
                  Date & time
                </th>
                <th
                  class="px-6 py-3 text-left text-sm font-bold uppercase tracking-wide text-slate-700"
                >
                  Referral
                </th>
                <th
                  class="px-6 py-3 text-left text-sm font-bold uppercase tracking-wide text-slate-700"
                >
                  Care team
                </th>
                <th
                  class="px-6 py-3 text-left text-sm font-bold uppercase tracking-wide text-slate-700"
                >
                  Facility
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
                <td class="px-6 py-4">
                  <p class="text-sm font-semibold text-slate-900">
                    {{ formatDate(appointment.appointmentDate) }}
                  </p>
                  <p class="mt-0.5 text-xs text-slate-500">
                    {{ formatTime(appointment.appointmentTime) }}
                  </p>
                </td>

                <td class="px-6 py-4">
                  <p class="text-sm font-medium text-slate-800">
                    {{ appointment.specialty || "Specialist consultation" }}
                  </p>
                  <p class="mt-0.5 text-xs text-slate-500">
                    Referral #{{ appointment.referralId }}
                  </p>
                </td>

                <td class="px-6 py-4">
                  <p class="text-sm font-medium text-slate-800">
                    {{ appointment.specialistName || "Specialist pending" }}
                  </p>
                  <p class="mt-0.5 text-xs text-slate-500">
                    Your attending specialist
                  </p>
                </td>

                <td class="px-6 py-4 text-sm text-slate-600">
                  {{ appointment.facilityName || "Facility pending" }}
                </td>

                <td class="px-6 py-4">
                  <span
                    class="rounded-full bg-emerald-50 px-2.5 py-1 text-xs font-semibold text-emerald-700"
                  >
                    {{ appointment.appointmentStatus }}
                  </span>
                </td>
              </tr>

              <!-- Empty State -->
              <tr
                v-if="(dashboard?.upcomingAppointmentList?.length ?? 0) === 0"
              >
                <td
                  colspan="5"
                  class="px-6 py-8 text-center text-sm text-slate-500"
                >
                  No upcoming appointments.
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>

      <!-- <div class="rounded-xl border border-slate-100 bg-white shadow-sm">
        <div class="border-b border-slate-100 px-6 py-4">
          <h2 class="text-base font-semibold text-slate-900">
            Recent referral
          </h2>
          <p class="mt-0.5 text-sm text-slate-500">
            Your latest referral status and destination
          </p>
        </div>
        <div class="divide-y divide-slate-100">
          <div
            v-for="referral in dashboard?.recentReferrals ?? []"
            :key="referral.referralId"
            class="flex flex-wrap items-center gap-x-6 gap-y-3 px-6 py-4"
          >
            <div class="min-w-48 flex-1">
              <p class="text-sm font-semibold text-slate-900">
                {{ referral.specialty }}
              </p>
              <p class="mt-0.5 text-xs text-slate-500">
                Referral #{{ referral.referralId }} ·
                {{ formatDate(referral.createdAt) }}
              </p>
            </div>
            <div class="min-w-40">
              <p
                class="text-xs font-medium uppercase tracking-wide text-slate-400"
              >
                Destination
              </p>
              <p class="mt-1 text-sm text-slate-700">
                {{ referral.destinationFacility || "Being arranged" }}
              </p>
            </div>
            <span
              class="rounded-full bg-slate-100 px-2.5 py-1 text-xs font-semibold text-slate-700"
            >
              {{ referral.referralStatus }}
            </span>
          </div>
          <p
            v-if="(dashboard?.recentReferrals?.length ?? 0) === 0"
            class="px-6 py-8 text-center text-sm text-slate-500"
          >
            No referral updates yet.
          </p>
        </div>
      </div> -->
    </div>
  </DashboardLayout>
</template>
