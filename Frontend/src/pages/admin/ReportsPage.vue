<script setup lang="ts">
import { ref, computed, onMounted } from "vue";
import DashboardLayout from "../../components/layout/DashboardLayout.vue";
import StatsCards from "../../components/specialist/StatsCards.vue";
import { adminNavLinks } from "../../config/navigation";
import type { StatCardItem } from "../../components/specialist/StatsCards.vue";

import {
  getDashboard,
  getReferralLeakage,
  getFacilityLeakage,
  getSpecialtyLoad,
  getReferralStatus,
  getMonthlyReferral,
  getTopSpecialists,
  getReferralAging,
  getScheduledDelays,
} from "../../api/admin";

// ✅ User
const user = ref({
  name: "Admin User",
  welcomeName: "Admin",
  role: "Administrator",
  initials: "AD",
});

// ✅ Data
const overview = ref<any>(null);
const leakage = ref<any>(null);
const specialtyLoad = ref<any[]>([]);
const facilityLeakage = ref<any[]>([]);
const referralStatus = ref<any[]>([]);
const monthlyReferral = ref<any[]>([]);
const topSpecialists = ref<any[]>([]);
const referralAging = ref<any>(null);
const scheduledDelays = ref<any>(null);
const loading = ref(true);

// ✅ Fetch
onMounted(async () => {
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
  } catch (err) {
    console.error("Reports error:", err);
  } finally {
    loading.value = false;
  }
});

// ✅ KPI Cards
const stats = computed<StatCardItem[]>(() => {
  if (!overview.value) return [];

  return [
    {
      label: "Total Referrals",
      value: overview.value.totalReferrals,
      icon: "clipboard",
      iconBg: "bg-purple-50",
      iconColor: "text-purple-600",
    },
    {
      label: "Facilities",
      value: facilityLeakage.value.length,
      icon: "users",
      iconBg: "bg-blue-50",
      iconColor: "text-blue-600",
    },
    {
      label: "Patients",
      value: overview.value.totalPatients,
      icon: "users",
      iconBg: "bg-indigo-50",
      iconColor: "text-indigo-600",
    },
    {
      label: "Specialists",
      value: overview.value.totalSpecialists,
      icon: "users",
      iconBg: "bg-green-50",
      iconColor: "text-green-600",
    },
  ];
});
</script>

<template>
  <DashboardLayout
    :nav-links="adminNavLinks"
    :user="user"
    title="Reports & Analytics"
    subtitle="Insights & performance overview"
  >
    <div class="space-y-8">

      <!-- ✅ KPI Cards -->
      <StatsCards :items="stats" :columns="4" />

      <!-- ✅ Leakage Highlight -->
      <div class="bg-gradient-to-r from-blue-50 to-blue-100 p-6 rounded-2xl shadow-sm">
        <p class="text-sm text-gray-500">Referral Leakage</p>
        <p class="text-5xl font-bold text-blue-600 mt-2">
          {{ leakage?.leakagePercentage }}%
        </p>
        <p class="text-sm text-gray-500 mt-1">
          Referrals not successfully completed
        </p>
      </div>

      <!-- ✅ GRID (3 CARDS) -->
      <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-5">

        <!-- Specialty Load -->
        <div class="bg-white p-5 rounded-2xl shadow-sm border hover:shadow-md transition">
          <h3 class="text-lg font-semibold mb-4 text-gray-800">Specialty Load</h3>

          <div v-for="s in specialtyLoad" :key="s.specialty" class="mb-3">
            <div class="flex justify-between text-sm mb-1">
              <span>{{ s.specialty }}</span>
              <span class="font-semibold">{{ s.referralCount }}</span>
            </div>
            <div class="w-full bg-gray-100 h-2 rounded-full">
              <div
                class="bg-indigo-500 h-2 rounded-full"
                :style="{ width: Math.min(s.referralCount * 2, 100) + '%' }"
              ></div>
            </div>
          </div>
        </div>

        <!-- Facility Leakage -->
        <div class="bg-white p-5 rounded-2xl shadow-sm border hover:shadow-md transition">
          <h3 class="text-lg font-semibold mb-4 text-gray-800">Facility Leakage</h3>

          <div
            v-for="f in facilityLeakage"
            :key="f.facilityName"
            class="flex justify-between text-sm py-1"
          >
            <span class="text-gray-600">{{ f.facilityName }}</span>
            <span class="text-red-500 font-semibold">{{ f.leakageCount }}</span>
          </div>
        </div>

        <!-- Referral Status -->
        <div class="bg-white p-5 rounded-2xl shadow-sm border hover:shadow-md transition">
          <h3 class="text-lg font-semibold mb-4 text-gray-800">Referral Status</h3>

          <div
            v-for="s in referralStatus"
            :key="s.status"
            class="flex justify-between text-sm py-1"
          >
            <span class="text-gray-600">{{ s.status }}</span>
            <span class="font-semibold">{{ s.count }}</span>
          </div>
        </div>
      </div>

      <!-- ✅ SECOND ROW -->
      <div class="grid grid-cols-1 md:grid-cols-2 gap-5">

        <!-- Trends -->
        <div class="bg-white p-5 rounded-2xl shadow-sm border hover:shadow-md transition">
          <h3 class="text-lg font-semibold mb-4 text-gray-800">Monthly Trends</h3>

          <div
            v-for="t in monthlyReferral"
            :key="t.year + '-' + t.month"
            class="flex justify-between text-sm py-1"
          >
            <span>
              {{
                new Date(t.year, t.month - 1).toLocaleString("default", {
                  month: "short",
                  year: "numeric",
                })
              }}
            </span>
            <span class="font-semibold">{{ t.count }}</span>
          </div>
        </div>

        <!-- Top Specialists -->
        <div class="bg-white p-5 rounded-2xl shadow-sm border hover:shadow-md transition">
          <h3 class="text-lg font-semibold mb-4 text-gray-800">Top Specialists</h3>

          <div
            v-for="s in topSpecialists"
            :key="s.specialist"
            class="flex justify-between text-sm py-1"
          >
            <span>{{ s.specialist }}</span>
            <span class="font-semibold">{{ s.referrals }}</span>
          </div>
        </div>
      </div>

      <!-- ✅ FINAL ROW -->
      <div class="grid grid-cols-1 md:grid-cols-2 gap-5">

        <!-- Aging -->
        <div class="bg-white p-5 rounded-2xl shadow-sm border hover:shadow-md transition">
          <h3 class="text-lg font-semibold mb-4 text-gray-800">Referral Waiting Time</h3>

          <div class="space-y-2 text-sm">
            <div class="flex justify-between text-black">
              <span>✅ Recently Added</span>
              <span>{{ referralAging?.lessThan3 }}</span>
            </div>
            <div class="flex justify-between text-black">
              <span>⚠️ Waiting Moderately</span>
              <span>{{ referralAging?.between3And7 }}</span>
            </div>
            <div class="flex justify-between text-black">
              <span>🚨 Waiting Too Long</span>
              <span>{{ referralAging?.moreThan7 }}</span>
            </div>
          </div>
        </div>

        <!-- Delays -->
        <div class="bg-white p-5 rounded-2xl shadow-sm border hover:shadow-md transition">
          <h3 class="text-lg font-semibold mb-4 text-gray-800">Scheduled Delays</h3>

          <div class="space-y-2 text-sm">
            <div class="flex justify-between">
              <span>Total</span>
              <span>{{ scheduledDelays?.totalScheduled }}</span>
            </div>
            <div class="flex justify-between text-red-600">
              <span>Delayed</span>
              <span>{{ scheduledDelays?.delayed }}</span>
            </div>
            <div class="flex justify-between text-green-600">
              <span>Healthy</span>
              <span>{{ scheduledDelays?.healthy }}</span>
            </div>
          </div>
        </div>
      </div>

    </div>
  </DashboardLayout>
</template>
