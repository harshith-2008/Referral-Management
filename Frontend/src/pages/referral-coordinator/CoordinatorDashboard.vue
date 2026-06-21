<script setup lang="ts">
import { computed, ref } from "vue";
import DashboardLayout from "../../components/layout/DashboardLayout.vue";
import StatsCards from "../../components/specialist/StatsCards.vue";
import RecentReferralsTable from "../../components/coordinator/RecentReferralsTable.vue";
import { coordinatorNavLinks } from "../../config/navigation";
import { mockCoordinatorReferrals } from "../../data/mockCoordinatorReferrals";
import type { StatCardItem } from "../../components/specialist/StatsCards.vue";

const user = ref({
  name: "Sarah Mitchell",
  welcomeName: "Sarah",
  role: "Referral Coordinator",
  initials: "SM",
});

const referrals = ref([...mockCoordinatorReferrals]);

const stats = computed<StatCardItem[]>(() => [
  {
    label: "Total Referrals",
    value: referrals.value.length,
    subtext: "All active records",
    icon: "clipboard",
    iconBg: "bg-blue-50",
    iconColor: "text-blue-600",
  },
  {
    label: "Submitted",
    value: referrals.value.filter((r) => r.status === "Submitted").length,
    subtext: "Awaiting routing",
    icon: "clock",
    iconBg: "bg-amber-50",
    iconColor: "text-amber-500",
  },
  {
    label: "Requested",
    value: referrals.value.filter((r) => r.status === "Requested").length,
    subtext: "Sent to hospitals",
    icon: "users",
    iconBg: "bg-blue-50",
    iconColor: "text-blue-600",
  },
  {
    label: "Accepted",
    value: referrals.value.filter((r) => r.status === "Accepted").length,
    subtext: "Docket assigned",
    icon: "check",
    iconBg: "bg-green-50",
    iconColor: "text-green-600",
  },
  {
    label: "Rejected",
    value: referrals.value.filter((r) => r.status === "Rejected").length,
    subtext: "Needs review",
    icon: "x",
    iconBg: "bg-red-50",
    iconColor: "text-red-500",
  },
  {
    label: "Closed",
    value: referrals.value.filter((r) => r.status === "Closed").length,
    subtext: "Completed",
    icon: "archive",
    iconBg: "bg-green-50",
    iconColor: "text-green-600",
  },
]);

const recentReferrals = computed(() => referrals.value.slice(0, 5));
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
