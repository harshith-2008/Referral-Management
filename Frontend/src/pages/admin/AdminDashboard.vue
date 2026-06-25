<script setup lang="ts">
import { ref, computed, onMounted } from "vue";
import DashboardLayout from "../../components/layout/DashboardLayout.vue";
import StatsCards from "../../components/specialist/StatsCards.vue";
import { adminNavLinks } from "../../config/navigation";
import type { StatCardItem } from "../../components/specialist/StatsCards.vue";
import { getDashboard } from "../../api/admin";

import { getDailyReferrals } from "../../api/admin";


// ✅ User
const user = ref({
  name: "Admin User",
  welcomeName: "Admin",
  role: "Administrator",
  initials: "AD",
});

// ✅ STATE
const dashboard = ref<any>(null);
const loading = ref(true);

// ✅ API CALL
onMounted(async () => {
  try {
    const res = await getDashboard();
    dashboard.value = res.data; // ✅ your API returns direct DTO
  } catch (err) {
    console.error("Dashboard error:", err);
  } finally {
    loading.value = false;
  }
});

// ✅ Daily Referrals (keep mock OR later connect API)

const dailyReferrals = ref<any[]>([]);

onMounted(async () => {
  try {
    const res = await getDailyReferrals();

    // convert backend data → UI format
    dailyReferrals.value = res.data.map((item: any) => {
      const date = new Date(item.date);

      return {
        date: date.toLocaleDateString("en-US", { weekday: "short" }),
        count: item.count,
      };
    });

  } catch (err) {
    console.error("Daily referrals error:", err);
  }
});


// ✅ Stats (SAFE with null check)
const stats = computed<StatCardItem[]>(() => {
  if (!dashboard.value) return [];

  return [
    {
      label: "Referrals",
      value: dashboard.value.totalReferrals,
      icon: "clipboard",
      iconBg: "bg-purple-50",
      iconColor: "text-purple-600",
    },
    {
      label: "Pending",
      value: dashboard.value.pendingReferrals,
      icon: "clock",
      iconBg: "bg-yellow-50",
      iconColor: "text-yellow-600",
    },
    {
      label: "Completed",
      value: dashboard.value.completedReferrals,
      icon: "check",
      iconBg: "bg-green-50",
      iconColor: "text-green-600",
    },
    {
      label: "Cancelled",
      value: dashboard.value.cancelledReferrals,
      icon: "x",
      iconBg: "bg-red-50",
      iconColor: "text-red-500",
    },
    {
      label: "Today Appointments",
      value: dashboard.value.appointmentsToday,
      icon: "calendar",
      iconBg: "bg-blue-50",
      iconColor: "text-blue-600",
    },
  ];
});
</script>


<template>
  <DashboardLayout
    :nav-links="adminNavLinks"
    :user="user"
    title="Admin Dashboard"
    :subtitle="`Welcome back, ${user.welcomeName}`"
  >
    <div class="space-y-6">

      <!-- ✅ Loading State -->
      <div v-if="loading" class="text-center text-gray-500 py-10">
        Loading dashboard...
      </div>

      <template v-else>

        <!-- ✅ TOP CARDS -->
        <StatsCards :items="stats" :columns="5" />

        <!-- ✅ LOWER SECTION -->
        <div class="grid grid-cols-1 gap-4">

          <!-- ✅ Daily Referrals -->
          <div class="bg-white p-5 rounded-xl shadow-sm border">
            <h3 class="text-lg font-semibold mb-4">Daily Referrals</h3>
            <p class="text-xs text-gray-400 mb-3">Last 5 days</p>

            <div class="space-y-3 text-sm">
              <div
                v-for="item in dailyReferrals"
                :key="item.date"
                class="flex justify-between"
              >
                <span>{{ item.date }}</span>
                <span>{{ item.count }}</span>
              </div>
            </div>
          </div>

        </div>

      </template>

    </div>
  </DashboardLayout>
</template>
