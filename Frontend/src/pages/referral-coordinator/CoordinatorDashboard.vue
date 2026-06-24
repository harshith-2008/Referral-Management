<script setup lang="ts">
import { computed, onMounted, ref } from "vue";

import DashboardLayout from "../../components/layout/DashboardLayout.vue";
import StatsCards from "../../components/specialist/StatsCards.vue";
import RecentReferralsTable from "../../components/coordinator/RecentReferralsTable.vue";

import { coordinatorNavLinks } from "../../config/navigation";
import { getDashboard } from "../../api/coordinator";

import type { StatCardItem } from "../../components/specialist/StatsCards.vue";
import type { CoordinatorDashboardDTO } from "../../types/coordinator";
import type { ReferralDTO } from "../../types/referral";

const user = ref({
  name: "Sarah Mitchell",
  welcomeName: "Sarah",
  role: "Referral Coordinator",
  initials: "SM",
});

const loading = ref(false);

const dashboard = ref<CoordinatorDashboardDTO>({
  totalReferrals: 0,
  submitted: 0,
  requested: 0,
  accepted: 0,
  rejected: 0,
  closed: 0,
  recentReferrals: [],
});

const loadDashboard = async () => {
  loading.value = true;

  try {
    const response = await getDashboard();

    dashboard.value = response.data.data;
  } catch (error) {
    console.error("Failed to load dashboard", error);
  } finally {
    loading.value = false;
  }
};

const stats = computed<StatCardItem[]>(() => [
  {
    label: "Total Referrals",
    value: dashboard.value.totalReferrals,
    subtext: "All active records",
    icon: "clipboard",
    iconBg: "bg-blue-50",
    iconColor: "text-blue-600",
  },
  {
    label: "Submitted",
    value: dashboard.value.submitted,
    subtext: "Awaiting routing",
    icon: "clock",
    iconBg: "bg-amber-50",
    iconColor: "text-amber-500",
  },
  {
    label: "Requested",
    value: dashboard.value.requested,
    subtext: "Sent to hospitals",
    icon: "users",
    iconBg: "bg-blue-50",
    iconColor: "text-blue-600",
  },
  {
    label: "Accepted",
    value: dashboard.value.accepted,
    subtext: "Docket assigned",
    icon: "check",
    iconBg: "bg-green-50",
    iconColor: "text-green-600",
  },
  {
    label: "Rejected",
    value: dashboard.value.rejected,
    subtext: "Needs review",
    icon: "x",
    iconBg: "bg-red-50",
    iconColor: "text-red-500",
  },
  {
    label: "Closed",
    value: dashboard.value.closed,
    subtext: "Completed",
    icon: "archive",
    iconBg: "bg-green-50",
    iconColor: "text-green-600",
  },
]);

const recentReferrals = computed(() => dashboard.value.recentReferrals);

onMounted(loadDashboard);
</script>

<template>
  <DashboardLayout
    :nav-links="coordinatorNavLinks"
    :user="user"
    title="Dashboard"
    :subtitle="`Welcome back, ${user.welcomeName}`"
    :notification-count="2"
  >
    <div class="space-y-6">
      <StatsCards :items="stats" :columns="6" />

      <RecentReferralsTable
        :referrals="recentReferrals"
        view-all-link="/coordinator/referrals"
      />
    </div>
  </DashboardLayout>
</template>
