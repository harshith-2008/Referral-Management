<script setup lang="ts">
import { ref, computed } from "vue";
import DashboardLayout from "../../components/layout/DashboardLayout.vue";
import StatsCards from "../../components/specialist/StatsCards.vue";
import { adminNavLinks } from "../../config/navigation";
import type { StatCardItem } from "../../components/specialist/StatsCards.vue";
import { getReferralStatus, getMonthlyReferral, getTopSpecialists, getReferralAging, getScheduledDelays } from "../../api/admin";

import { onMounted } from "vue";
import {
  getDashboard,
  getReferralLeakage,
  getFacilityLeakage,
  getSpecialtyLoad,
} from "../../api/admin";


// ✅ User
const user = ref({
  name: "Admin User",
  welcomeName: "Admin",
  role: "Administrator",
  initials: "AD",
});

// ✅ Overview data (analytics-level)

const overview = ref<any>(null);
const leakage = ref<any>(null);
const specialtyLoad = ref<any[]>([]);
const facilityLeakage = ref<any[]>([]);
const loading = ref(true);
const referralStatus = ref<any[]>([]);

const monthlyReferral = ref<any[]>([]);
const topSpecialists = ref<any[]>([]);


const referralAging = ref<any>(null);
const scheduledDelays = ref<any>(null);





onMounted(async () => {
  try {
    const [dashboardRes, leakageRes, specRes, facRes, statusRes, monRefRes, topSpecRes, agingRes, delayRes] =
      await Promise.all([
        getDashboard(),
        getReferralLeakage(),
        getSpecialtyLoad(),
        getFacilityLeakage(),
        getReferralStatus(), 
        getMonthlyReferral(),
        getTopSpecialists(),
        getReferralAging(),      // ✅ NEW
        getScheduledDelays()     // ✅ NEW

      ]);

    overview.value = dashboardRes.data;
    leakage.value = leakageRes.data;
    specialtyLoad.value = specRes.data;
    facilityLeakage.value = facRes.data;

    referralStatus.value = statusRes.data; // ✅ NEW
    
monthlyReferral.value = monRefRes.data;       // ✅ NEW
topSpecialists.value = topSpecRes.data;      // ✅ NEW

referralAging.value = agingRes.data;          // ✅ NEW
scheduledDelays.value = delayRes.data;        // ✅ NE



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
      value: facilityLeakage.value.length, // ✅ workaround
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
    subtitle="Overall system insights"
  >
    <div class="space-y-6">

      <!-- ✅ Top KPI Cards -->
      <StatsCards :items="stats" :columns="4" />

      <!-- ✅ Leakage (MAIN KPI) -->
      <div class="bg-white p-5 rounded-xl shadow-sm border">
        <h3 class="text-lg font-semibold mb-4">Referral Leakage</h3>

        <p class="text-4xl font-bold text-blue-600">
          {{ leakage?.leakagePercentage }}%
        </p>
        <p class="text-sm text-gray-500 mt-2">
          Percentage of referrals not successfully completed
        </p>
      </div>

      <!-- ✅ Analytics Section -->
      <div class="grid grid-cols-1 md:grid-cols-2 gap-4">

        <!-- ✅ Specialty Load -->
        <div class="bg-white p-5 rounded-xl shadow-sm border">
          <h3 class="text-lg font-semibold mb-4">Specialty Load</h3>

          <div class="space-y-3 text-sm">
            <div
              v-for="s in specialtyLoad"
              :key="s.specialty"
              class="flex justify-between"
            >
              <span class="text-slate-600">{{ s.specialty }}</span>
              <span class="font-semibold">{{ s.referralCount }}</span>
            </div>
          </div>
        </div>

        <!-- ✅ Facility Leakage -->
        <div class="bg-white p-5 rounded-xl shadow-sm border">
          <h3 class="text-lg font-semibold mb-4">Facility Leakage</h3>

          <div class="space-y-3 text-sm">
            <div
              v-for="f in facilityLeakage"
              :key="f.facilityName"
              class="flex justify-between"
            >
              <span class="text-slate-600">{{ f.facilityName }}</span>
              <span class="text-red-500 font-semibold">
                {{ f.leakageCount }}
              </span>
            </div>
          </div>
        </div>

    

      </div>
      <!-- ✅ referral trends(monthly) Section -->
      <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
        
 <!-- ✅ Referral Trends -->
  <div class="bg-white p-5 rounded-xl shadow-sm border">
    <h3 class="text-lg font-semibold mb-4">Referral Trends (Monthly)</h3>

    <div class="space-y-3 text-sm">
      <div
        v-for="t in monthlyReferral"
        :key="t.year + '-' + t.month"
        class="flex justify-between"
      >
        <span class="text-slate-600">
          {{ new Date(t.year, t.month - 1).toLocaleString("default", { month: "short", year: "numeric" }) }}
        </span>
        <span class="font-semibold text-slate-900">
          {{ t.count }}
        </span>
      </div>
    </div>
  </div>

  <!-- ✅ Top Specialists -->
  <div class="bg-white p-5 rounded-xl shadow-sm border">
    <h3 class="text-lg font-semibold mb-4">Top Specialists</h3>

    <div class="space-y-3 text-sm">
      <div
        v-for="s in topSpecialists"
        :key="s.specialist"
        class="flex justify-between"
      >
        <span class="text-slate-600">{{ s.specialist }}</span>
        <span class="font-semibold text-slate-900">{{ s.referrals }}</span>
      </div>
    </div>
  </div>

      </div>
      <!-- ✅ Aging + Scheduled Delays -->
<div class="grid grid-cols-1 md:grid-cols-2 gap-4">

  <!-- ✅ Referral Aging -->
  <div class="bg-white p-5 rounded-xl shadow-sm border">
    <h3 class="text-lg font-semibold mb-4">Referral Aging</h3>

    <div class="space-y-3 text-sm">
      <div class="flex justify-between">
        <span class="text-green-600">&lt; 3 days</span>
        <span class="font-semibold">{{ referralAging?.lessThan3 }}</span>
      </div>

      <div class="flex justify-between">
        <span class="text-yellow-600">3 – 7 days</span>
        <span class="font-semibold">{{ referralAging?.between3And7 }}</span>
      </div>

      <div class="flex justify-between">
        <span class="text-red-600">&gt; 7 days</span>
        <span class="font-semibold">{{ referralAging?.moreThan7 }}</span>
      </div>
    </div>
  </div>

  <!-- ✅ Scheduled Delays -->
  <div class="bg-white p-5 rounded-xl shadow-sm border">
    <h3 class="text-lg font-semibold mb-4">Scheduled Delays</h3>

    <div class="space-y-3 text-sm">
      <div class="flex justify-between">
        <span>Total Scheduled</span>
        <span class="font-semibold">{{ scheduledDelays?.totalScheduled }}</span>
      </div>

      <div class="flex justify-between">
        <span class="text-red-600">Delayed (&gt; 7 days)</span>
        <span class="font-semibold">{{ scheduledDelays?.delayed }}</span>
      </div>

      <div class="flex justify-between">
        <span class="text-green-600">Healthy (≤ 7 days)</span>
        <span class="font-semibold">{{ scheduledDelays?.healthy }}</span>
      </div>
    </div>
  </div>

</div>

      <!-- ✅ Referral Status Breakdown -->
<div class="bg-white p-5 rounded-xl shadow-sm border">
  <h3 class="text-lg font-semibold mb-4">Referral Status Breakdown</h3>

  <div class="space-y-3 text-sm">
    <div
      v-for="s in referralStatus"
      :key="s.status"
      class="flex justify-between"
    >
      <span class="text-slate-600">{{ s.status }}</span>
      <span class="font-semibold text-slate-900">{{ s.count }}</span>
    </div>
  </div>
</div>


      <!-- ✅ Top Specialties -->
    

    </div>
  </DashboardLayout>
</template>
