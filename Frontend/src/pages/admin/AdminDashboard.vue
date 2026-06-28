<script setup lang="ts">
import { computed, onMounted, ref } from "vue";

import DashboardLayout from "../../components/layout/DashboardLayout.vue";
import { adminNavLinks } from "../../config/navigation";
import {
  getAppointmentAnalytics,
  getDailyReferrals,
  getDashboard,
  getReferralLeakage,
  getReferralStatus,
} from "../../api/admin";

import type {
  AdminDashboardDTO,
  AppointmentAnalyticsDTO,
  DailyReferralDTO,
  ReferralLeakageDTO,
  StatusCountDTO,
} from "../../types/admin";

const dashboard = ref<AdminDashboardDTO | null>(null);
const appointmentAnalytics = ref<AppointmentAnalyticsDTO | null>(null);
const dailyReferrals = ref<DailyReferralDTO[]>([]);
const referralStatus = ref<StatusCountDTO[]>([]);
const leakage = ref<ReferralLeakageDTO | null>(null);

const loading = ref(true);
const errorMessage = ref("");

const formatNumber = (value?: number | null) =>
  Number(value ?? 0).toLocaleString();

const formatDateLabel = (value: string) =>
  new Date(value).toLocaleDateString(undefined, {
    weekday: "short",
    day: "numeric",
  });

const percentage = (value: number, total: number) =>
  total > 0 ? Math.round((value / total) * 100) : 0;

const dailyMax = computed(() =>
  Math.max(...dailyReferrals.value.map((item) => item.count), 1),
);

const totalStatusCount = computed(() =>
  referralStatus.value.reduce((total, status) => total + status.count, 0),
);

const completionRate = computed(() =>
  percentage(
    dashboard.value?.completedReferrals ?? 0,
    dashboard.value?.totalReferrals ?? 0,
  ),
);

const pendingRate = computed(() =>
  percentage(
    dashboard.value?.pendingReferrals ?? 0,
    dashboard.value?.totalReferrals ?? 0,
  ),
);

const leakageRate = computed(() => Math.round(leakage.value?.leakagePercentage ?? 0));

const primaryMetrics = computed(() => [
  {
    label: "Total Referrals",
    value: formatNumber(dashboard.value?.totalReferrals),
    detail: `${completionRate.value}% completed`,
    accent: "from-blue-500 to-indigo-500",
  },
  {
    label: "Pending Workload",
    value: formatNumber(dashboard.value?.pendingReferrals),
    detail: `${pendingRate.value}% of referrals`,
    accent: "from-amber-500 to-orange-500",
  },
  {
    label: "Patients",
    value: formatNumber(dashboard.value?.totalPatients),
    detail: `${formatNumber(dashboard.value?.totalUsers)} total users`,
    accent: "from-violet-500 to-purple-500",
  },
  {
    label: "Specialists",
    value: formatNumber(dashboard.value?.totalSpecialists),
    detail: `${formatNumber(dashboard.value?.appointmentsToday)} appointments today`,
    accent: "from-emerald-500 to-teal-500",
  },
]);

const appointmentCards = computed(() => [
  {
    label: "Upcoming",
    value: appointmentAnalytics.value?.upcoming ?? 0,
    class: "bg-blue-50 text-blue-700",
  },
  {
    label: "Completed",
    value: appointmentAnalytics.value?.completed ?? 0,
    class: "bg-emerald-50 text-emerald-700",
  },
  {
    label: "Missed",
    value: appointmentAnalytics.value?.missed ?? 0,
    class: "bg-red-50 text-red-700",
  },
]);

onMounted(async () => {
  loading.value = true;
  errorMessage.value = "";

  try {
    const [
      dashboardRes,
      dailyRes,
      statusRes,
      appointmentsRes,
      leakageRes,
    ] = await Promise.all([
      getDashboard(),
      getDailyReferrals(),
      getReferralStatus(),
      getAppointmentAnalytics(),
      getReferralLeakage(),
    ]);

    dashboard.value = dashboardRes.data;
    dailyReferrals.value = dailyRes.data;
    referralStatus.value = statusRes.data;
    appointmentAnalytics.value = appointmentsRes.data;
    leakage.value = leakageRes.data;
  } catch (error) {
    console.error("Dashboard error:", error);
    errorMessage.value = "Unable to load admin dashboard data.";
  } finally {
    loading.value = false;
  }
});
</script>

<template>
  <DashboardLayout
    :nav-links="adminNavLinks"
    title="Admin Dashboard"
    subtitle="Operational overview across referrals, patients, and appointments"
  >
    <div class="space-y-6">
      <div
        v-if="errorMessage"
        class="rounded-2xl border border-red-200 bg-red-50 px-5 py-4 text-sm text-red-700"
      >
        {{ errorMessage }}
      </div>

      <div
        v-if="loading"
        class="rounded-2xl border border-slate-200 bg-white p-10 text-center text-sm text-slate-500 shadow-sm"
      >
        Loading admin dashboard...
      </div>

      <template v-else>
        <section class="grid grid-cols-4 gap-4">
          <div
            v-for="metric in primaryMetrics"
            :key="metric.label"
            class="overflow-hidden rounded-2xl border border-slate-100 bg-white shadow-sm"
          >
            <div class="h-1.5 bg-gradient-to-r" :class="metric.accent" />
            <div class="p-5">
              <p class="text-sm font-medium text-slate-500">{{ metric.label }}</p>
              <p class="mt-3 text-3xl font-bold tracking-tight text-slate-950">
                {{ metric.value }}
              </p>
              <p class="mt-1 text-xs font-medium text-slate-400">
                {{ metric.detail }}
              </p>
            </div>
          </div>
        </section>

        <section class="grid grid-cols-12 gap-5">
          <div class="col-span-8 rounded-2xl border border-slate-100 bg-white p-6 shadow-sm">
            <div class="flex items-start justify-between">
              <div>
                <h2 class="text-base font-semibold text-slate-950">
                  Daily referral intake
                </h2>
                <p class="mt-1 text-sm text-slate-500">
                  Recent referral volume by day
                </p>
              </div>
              <span class="rounded-full bg-slate-100 px-3 py-1 text-xs font-semibold text-slate-600">
                {{ dailyReferrals.length }} days
              </span>
            </div>

            <div class="mt-8 flex h-56 items-end gap-3">
              <div
                v-for="item in dailyReferrals"
                :key="item.date"
                class="flex min-w-0 flex-1 flex-col items-center gap-3"
              >
                <div class="flex h-44 w-full items-end rounded-xl bg-slate-50 px-2">
                  <div
                    class="w-full rounded-t-lg bg-gradient-to-t from-blue-600 to-blue-400"
                    :style="{ height: `${Math.max((item.count / dailyMax) * 100, 8)}%` }"
                  />
                </div>
                <div class="text-center">
                  <p class="text-sm font-semibold text-slate-800">
                    {{ item.count }}
                  </p>
                  <p class="text-xs text-slate-400">
                    {{ formatDateLabel(item.date) }}
                  </p>
                </div>
              </div>
            </div>
          </div>

          <div class="col-span-4 space-y-5">
            <div class="rounded-2xl border border-slate-100 bg-slate-950 p-6 text-white shadow-sm">
              <p class="text-sm font-medium text-slate-400">Referral completion</p>
              <div class="mt-4 flex items-end gap-3">
                <p class="text-5xl font-bold">{{ completionRate }}%</p>
                <p class="pb-2 text-sm text-slate-400">completed</p>
              </div>
              <div class="mt-5 h-2 rounded-full bg-white/10">
                <div
                  class="h-2 rounded-full bg-emerald-400"
                  :style="{ width: `${completionRate}%` }"
                />
              </div>
              <p class="mt-3 text-xs text-slate-400">
                {{ formatNumber(dashboard?.completedReferrals) }} of
                {{ formatNumber(dashboard?.totalReferrals) }} referrals completed.
              </p>
            </div>

            <div class="rounded-2xl border border-slate-100 bg-white p-6 shadow-sm">
              <div class="flex items-center justify-between">
                <p class="text-sm font-medium text-slate-500">Leakage risk</p>
                <span
                  class="rounded-full px-2.5 py-1 text-xs font-semibold"
                  :class="leakageRate > 30 ? 'bg-red-50 text-red-700' : 'bg-amber-50 text-amber-700'"
                >
                  {{ leakageRate }}%
                </span>
              </div>
              <p class="mt-3 text-2xl font-bold text-slate-950">
                {{ formatNumber(leakage?.leakageCount) }} referrals
              </p>
              <p class="mt-1 text-sm text-slate-500">
                Not successfully completed yet.
              </p>
            </div>
          </div>
        </section>

        <section class="grid grid-cols-3 gap-5">
          <div class="rounded-2xl border border-slate-100 bg-white p-6 shadow-sm">
            <h2 class="text-base font-semibold text-slate-950">Referral status</h2>
            <div class="mt-5 space-y-4">
              <div v-for="status in referralStatus" :key="status.status">
                <div class="mb-1 flex items-center justify-between text-sm">
                  <span class="font-medium text-slate-600">{{ status.status }}</span>
                  <span class="font-semibold text-slate-900">{{ status.count }}</span>
                </div>
                <div class="h-2 rounded-full bg-slate-100">
                  <div
                    class="h-2 rounded-full bg-blue-500"
                    :style="{ width: `${percentage(status.count, totalStatusCount)}%` }"
                  />
                </div>
              </div>
            </div>
          </div>

          <div class="rounded-2xl border border-slate-100 bg-white p-6 shadow-sm">
            <h2 class="text-base font-semibold text-slate-950">Appointments</h2>
            <p class="mt-1 text-sm text-slate-500">
              {{ formatNumber(appointmentAnalytics?.totalAppointments) }} total appointments
            </p>
            <div class="mt-5 grid gap-3">
              <div
                v-for="item in appointmentCards"
                :key="item.label"
                class="flex items-center justify-between rounded-xl px-4 py-3"
                :class="item.class"
              >
                <span class="text-sm font-semibold">{{ item.label }}</span>
                <span class="text-lg font-bold">{{ formatNumber(item.value) }}</span>
              </div>
            </div>
          </div>

          <div class="rounded-2xl border border-slate-100 bg-white p-6 shadow-sm">
            <h2 class="text-base font-semibold text-slate-950">System footprint</h2>
            <div class="mt-5 space-y-4">
              <div class="flex items-center justify-between">
                <span class="text-sm text-slate-500">Users</span>
                <span class="font-semibold text-slate-950">{{ formatNumber(dashboard?.totalUsers) }}</span>
              </div>
              <div class="flex items-center justify-between">
                <span class="text-sm text-slate-500">Patients</span>
                <span class="font-semibold text-slate-950">{{ formatNumber(dashboard?.totalPatients) }}</span>
              </div>
              <div class="flex items-center justify-between">
                <span class="text-sm text-slate-500">Specialists</span>
                <span class="font-semibold text-slate-950">{{ formatNumber(dashboard?.totalSpecialists) }}</span>
              </div>
              <div class="flex items-center justify-between">
                <span class="text-sm text-slate-500">Today appointments</span>
                <span class="font-semibold text-slate-950">{{ formatNumber(dashboard?.appointmentsToday) }}</span>
              </div>
            </div>
          </div>
        </section>
      </template>
    </div>
  </DashboardLayout>
</template>
