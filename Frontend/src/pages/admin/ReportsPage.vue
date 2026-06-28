<script setup lang="ts">
import { computed, onMounted, ref } from "vue";

import DashboardLayout from "../../components/layout/DashboardLayout.vue";
import { adminNavLinks } from "../../config/navigation";

import {
  getDashboard,
  getFacilityLeakage,
  getMonthlyReferral,
  getReferralAging,
  getReferralLeakage,
  getReferralStatus,
  getScheduledDelays,
  getSpecialtyLoad,
  getTopSpecialists,
} from "../../api/admin";

import type {
  AdminDashboardDTO,
  FacilityLeakageDTO,
  MonthlyReferralDTO,
  ReferralAgingDTO,
  ReferralLeakageDTO,
  ScheduledDelayDTO,
  SpecialtyLoadDTO,
  StatusCountDTO,
  TopSpecialistDTO,
} from "../../types/admin";

const overview = ref<AdminDashboardDTO | null>(null);
const leakage = ref<ReferralLeakageDTO | null>(null);
const specialtyLoad = ref<SpecialtyLoadDTO[]>([]);
const facilityLeakage = ref<FacilityLeakageDTO[]>([]);
const referralStatus = ref<StatusCountDTO[]>([]);
const monthlyReferral = ref<MonthlyReferralDTO[]>([]);
const topSpecialists = ref<TopSpecialistDTO[]>([]);
const referralAging = ref<ReferralAgingDTO | null>(null);
const scheduledDelays = ref<ScheduledDelayDTO | null>(null);

const loading = ref(true);
const errorMessage = ref("");

const formatNumber = (value?: number | null) =>
  Number(value ?? 0).toLocaleString();

const percentage = (value: number, total: number) =>
  total > 0 ? Math.round((value / total) * 100) : 0;

const monthLabel = (year: number, month: number) =>
  new Date(year, month - 1).toLocaleString(undefined, {
    month: "short",
    year: "numeric",
  });

const maxSpecialtyLoad = computed(() =>
  Math.max(...specialtyLoad.value.map((item) => item.referralCount), 1),
);

const maxFacilityLeakage = computed(() =>
  Math.max(...facilityLeakage.value.map((item) => item.leakageCount), 1),
);

const maxMonthlyCount = computed(() =>
  Math.max(...monthlyReferral.value.map((item) => item.count), 1),
);

const maxSpecialistReferrals = computed(() =>
  Math.max(...topSpecialists.value.map((item) => item.referrals), 1),
);

const totalStatusCount = computed(() =>
  referralStatus.value.reduce((sum, item) => sum + item.count, 0),
);

const totalAging = computed(() => {
  if (!referralAging.value) return 0;

  return (
    referralAging.value.lessThan3 +
    referralAging.value.between3And7 +
    referralAging.value.moreThan7
  );
});

const delayedAcceptedRate = computed(() =>
  percentage(
    scheduledDelays.value?.delayed ?? 0,
    scheduledDelays.value?.totalScheduled ?? 0,
  ),
);

const healthSummary = computed(() => [
  {
    label: "Leakage",
    value: `${Math.round(leakage.value?.leakagePercentage ?? 0)}%`,
    detail: `${formatNumber(leakage.value?.leakageCount)} of ${formatNumber(
      leakage.value?.totalReferrals,
    )} referrals`,
    tone: "bg-red-50 text-red-700 ring-red-100",
  },
  {
    label: "Completed",
    value: formatNumber(overview.value?.completedReferrals),
    detail: `${formatNumber(overview.value?.pendingReferrals)} still pending`,
    tone: "bg-emerald-50 text-emerald-700 ring-emerald-100",
  },
  {
    label: "Accepted delays",
    value: `${delayedAcceptedRate.value}%`,
    detail: `${formatNumber(scheduledDelays.value?.delayed)} delayed accepted referrals`,
    tone: "bg-amber-50 text-amber-700 ring-amber-100",
  },
  {
    label: "Rejected / Closed",
    value: formatNumber(overview.value?.cancelledReferrals),
    detail: "Rejected or closed referrals",
    tone: "bg-slate-100 text-slate-700 ring-slate-200",
  },
]);

const leakageBreakdown = computed(() => [
  {
    label: "No appointment",
    value: leakage.value?.noAppointment ?? 0,
    color: "bg-slate-500",
  },
  {
    label: "Delayed appointments",
    value: leakage.value?.delayedAppointments ?? 0,
    color: "bg-amber-500",
  },
  {
    label: "Never completed",
    value: leakage.value?.neverCompleted ?? 0,
    color: "bg-red-500",
  },
]);

const agingItems = computed(() => [
  {
    label: "< 3 days",
    value: referralAging.value?.lessThan3 ?? 0,
    color: "bg-emerald-500",
    text: "text-emerald-700",
  },
  {
    label: "3-7 days",
    value: referralAging.value?.between3And7 ?? 0,
    color: "bg-amber-500",
    text: "text-amber-700",
  },
  {
    label: "> 7 days",
    value: referralAging.value?.moreThan7 ?? 0,
    color: "bg-red-500",
    text: "text-red-700",
  },
]);

onMounted(async () => {
  loading.value = true;
  errorMessage.value = "";

  try {
    const [
      dashboardRes,
      leakageRes,
      specRes,
      facRes,
      statusRes,
      monRefRes,
      topSpecRes,
      agingRes,
      delayRes,
    ] = await Promise.all([
      getDashboard(),
      getReferralLeakage(),
      getSpecialtyLoad(),
      getFacilityLeakage(),
      getReferralStatus(),
      getMonthlyReferral(),
      getTopSpecialists(),
      getReferralAging(),
      getScheduledDelays(),
    ]);

    overview.value = dashboardRes.data;
    leakage.value = leakageRes.data;
    specialtyLoad.value = specRes.data;
    facilityLeakage.value = facRes.data;
    referralStatus.value = statusRes.data;
    monthlyReferral.value = monRefRes.data;
    topSpecialists.value = topSpecRes.data;
    referralAging.value = agingRes.data;
    scheduledDelays.value = delayRes.data;
  } catch (error) {
    console.error("Reports error:", error);
    errorMessage.value = "Unable to load reports and analytics.";
  } finally {
    loading.value = false;
  }
});
</script>

<template>
  <DashboardLayout
    :nav-links="adminNavLinks"
    title="Reports & Analytics"
    subtitle="Referral performance, leakage, capacity, and aging insights"
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
        Loading analytics...
      </div>

      <template v-else>
        <section class="grid grid-cols-4 gap-4">
          <div
            v-for="item in healthSummary"
            :key="item.label"
            class="rounded-2xl border border-slate-100 bg-white p-5 shadow-sm"
          >
            <div class="flex items-center justify-between">
              <p class="text-sm font-medium text-slate-500">{{ item.label }}</p>
              <span
                class="rounded-full px-2.5 py-1 text-xs font-semibold ring-1"
                :class="item.tone"
              >
                Live
              </span>
            </div>
            <p class="mt-4 text-3xl font-bold tracking-tight text-slate-950">
              {{ item.value }}
            </p>
            <p class="mt-1 text-xs text-slate-500">{{ item.detail }}</p>
          </div>
        </section>

        <section class="grid grid-cols-12 gap-5">
          <div
            class="col-span-8 rounded-2xl border border-slate-100 bg-white p-6 shadow-sm"
          >
            <div class="flex items-start justify-between">
              <div>
                <h2 class="text-base font-semibold text-slate-950">
                  Monthly referral trend
                </h2>
                <p class="mt-1 text-sm text-slate-500">
                  Referral count over reported months
                </p>
              </div>
              <span
                class="rounded-full bg-blue-50 px-3 py-1 text-xs font-semibold text-blue-700"
              >
                {{ monthlyReferral.length }} periods
              </span>
            </div>

            <div class="mt-8 flex h-64 items-end gap-3">
              <div
                v-for="item in monthlyReferral"
                :key="`${item.year}-${item.month}`"
                class="flex min-w-0 flex-1 flex-col items-center gap-3"
              >
                <div
                  class="flex h-48 w-full items-end rounded-xl bg-slate-50 px-2"
                >
                  <div
                    class="w-full rounded-t-lg bg-gradient-to-t from-indigo-600 to-sky-400"
                    :style="{
                      height: `${Math.max((item.count / maxMonthlyCount) * 100, 8)}%`,
                    }"
                  />
                </div>
                <div class="text-center">
                  <p class="text-sm font-bold text-slate-900">
                    {{ item.count }}
                  </p>
                  <p class="text-xs text-slate-400">
                    {{ monthLabel(item.year, item.month) }}
                  </p>
                </div>
              </div>
            </div>
          </div>

          <div
            class="col-span-4 rounded-2xl border border-slate-100 bg-slate-950 p-6 text-white shadow-sm"
          >
            <p class="text-sm font-medium text-slate-400">Referral leakage</p>
            <p class="mt-3 text-5xl font-bold">
              {{ Math.round(leakage?.leakagePercentage ?? 0) }}%
            </p>
            <p class="mt-2 text-sm text-slate-400">
              {{ formatNumber(leakage?.leakageCount) }} referrals need
              attention.
            </p>

            <div class="mt-6 space-y-4">
              <div v-for="item in leakageBreakdown" :key="item.label">
                <div class="mb-1 flex items-center justify-between text-sm">
                  <span class="text-slate-300">{{ item.label }}</span>
                  <span class="font-semibold text-white">{{
                    formatNumber(item.value)
                  }}</span>
                </div>
                <div class="h-2 rounded-full bg-white/10">
                  <div
                    class="h-2 rounded-full"
                    :class="item.color"
                    :style="{
                      width: `${percentage(item.value, leakage?.leakageCount ?? 0)}%`,
                    }"
                  />
                </div>
              </div>
            </div>
          </div>
        </section>

        <section class="grid grid-cols-3 gap-5">
          <div
            class="rounded-2xl border border-slate-100 bg-white p-6 shadow-sm"
          >
            <h2 class="text-base font-semibold text-slate-950">
              Specialty demand
            </h2>
            <p class="mt-1 text-sm text-slate-500">
              Highest referral load by specialty
            </p>

            <div class="mt-5 space-y-4">
              <div v-for="item in specialtyLoad" :key="item.specialty">
                <div class="mb-1 flex items-center justify-between text-sm">
                  <span class="font-medium text-slate-700">{{
                    item.specialty
                  }}</span>
                  <span class="font-semibold text-slate-950">{{
                    item.referralCount
                  }}</span>
                </div>
                <div class="h-2 rounded-full bg-slate-100">
                  <div
                    class="h-2 rounded-full bg-indigo-500"
                    :style="{
                      width: `${Math.max((item.referralCount / maxSpecialtyLoad) * 100, 6)}%`,
                    }"
                  />
                </div>
              </div>
            </div>
          </div>

          <div
            class="rounded-2xl border border-slate-100 bg-white p-6 shadow-sm"
          >
            <h2 class="text-base font-semibold text-slate-950">
              Facility leakage
            </h2>
            <p class="mt-1 text-sm text-slate-500">
              Facilities with incomplete outcomes
            </p>

            <div class="mt-5 space-y-4">
              <div v-for="item in facilityLeakage" :key="item.facilityName">
                <div class="mb-1 flex items-center justify-between text-sm">
                  <span class="truncate pr-3 font-medium text-slate-700">{{
                    item.facilityName
                  }}</span>
                  <span class="font-semibold text-red-600">{{
                    item.leakageCount
                  }}</span>
                </div>
                <div class="h-2 rounded-full bg-slate-100">
                  <div
                    class="h-2 rounded-full bg-red-500"
                    :style="{
                      width: `${Math.max((item.leakageCount / maxFacilityLeakage) * 100, 6)}%`,
                    }"
                  />
                </div>
              </div>
            </div>
          </div>

          <div
            class="rounded-2xl border border-slate-100 bg-white p-6 shadow-sm"
          >
            <h2 class="text-base font-semibold text-slate-950">
              Referral status mix
            </h2>
            <p class="mt-1 text-sm text-slate-500">
              Current distribution by status
            </p>

            <div class="mt-5 space-y-4">
              <div
                v-for="item in referralStatus"
                :key="item.status"
                class="rounded-xl bg-slate-50 px-4 py-3"
              >
                <div class="flex items-center justify-between">
                  <span class="text-sm font-semibold text-slate-700">{{
                    item.status
                  }}</span>
                  <span class="text-sm font-bold text-slate-950">{{
                    item.count
                  }}</span>
                </div>
                <div class="mt-2 h-1.5 rounded-full bg-white">
                  <div
                    class="h-1.5 rounded-full bg-blue-500"
                    :style="{
                      width: `${percentage(item.count, totalStatusCount)}%`,
                    }"
                  />
                </div>
              </div>
            </div>
          </div>
        </section>

        <section class="grid grid-cols-12 gap-5">
          <div
            class="col-span-5 rounded-2xl border border-slate-100 bg-white p-6 shadow-sm"
          >
            <h2 class="text-base font-semibold text-slate-950">
              Top specialists
            </h2>
            <p class="mt-1 text-sm text-slate-500">
              Referral volume handled by specialist
            </p>

            <div class="mt-5 space-y-4">
              <div
                v-for="(item, index) in topSpecialists"
                :key="item.specialist"
              >
                <div
                  class="mb-1 flex items-center justify-between gap-3 text-sm"
                >
                  <span class="truncate font-medium text-slate-700">
                    {{ index + 1 }}. {{ item.specialist }}
                  </span>
                  <span class="font-bold text-slate-950">{{
                    item.referrals
                  }}</span>
                </div>
                <div class="h-2 rounded-full bg-slate-100">
                  <div
                    class="h-2 rounded-full bg-emerald-500"
                    :style="{
                      width: `${Math.max((item.referrals / maxSpecialistReferrals) * 100, 6)}%`,
                    }"
                  />
                </div>
              </div>
            </div>
          </div>

          <div
            class="col-span-4 rounded-2xl border border-slate-100 bg-white p-6 shadow-sm"
          >
            <h2 class="text-base font-semibold text-slate-950">
              Referral aging
            </h2>
            <p class="mt-1 text-sm text-slate-500">Open referral age bands</p>

            <div class="mt-6 space-y-5">
              <div v-for="item in agingItems" :key="item.label">
                <div class="mb-2 flex items-center justify-between">
                  <span class="text-sm font-semibold" :class="item.text">{{
                    item.label
                  }}</span>
                  <span class="text-sm font-bold text-slate-950">{{
                    formatNumber(item.value)
                  }}</span>
                </div>
                <div class="h-3 rounded-full bg-slate-100">
                  <div
                    class="h-3 rounded-full"
                    :class="item.color"
                    :style="{
                      width: `${Math.max(percentage(item.value, totalAging), item.value ? 6 : 0)}%`,
                    }"
                  />
                </div>
              </div>
            </div>
          </div>

          <div
            class="col-span-3 rounded-2xl border border-slate-100 bg-white p-6 shadow-sm"
          >
            <h2 class="text-base font-semibold text-slate-950">
              Accepted delay health
            </h2>
            <p class="mt-1 text-sm text-slate-500">
              Accepted referrals by delay status
            </p>

            <div class="mt-6 rounded-2xl bg-amber-50 p-5 text-amber-800">
              <p class="text-sm font-semibold">Delayed accepted</p>
              <p class="mt-2 text-4xl font-bold">{{ delayedAcceptedRate }}%</p>
              <p class="mt-1 text-xs">
                {{ formatNumber(scheduledDelays?.delayed) }} of
                {{ formatNumber(scheduledDelays?.totalScheduled) }}
              </p>
            </div>

            <div class="mt-4 grid grid-cols-2 gap-3">
              <div class="rounded-xl bg-emerald-50 p-4 text-emerald-700">
                <p class="text-xs font-semibold">Healthy</p>
                <p class="mt-1 text-2xl font-bold">
                  {{ formatNumber(scheduledDelays?.healthy) }}
                </p>
              </div>
              <div class="rounded-xl bg-slate-100 p-4 text-slate-700">
                <p class="text-xs font-semibold">Total</p>
                <p class="mt-1 text-2xl font-bold">
                  {{ formatNumber(scheduledDelays?.totalScheduled) }}
                </p>
              </div>
            </div>
          </div>
        </section>
      </template>
    </div>
  </DashboardLayout>
</template>
