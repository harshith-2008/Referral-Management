<script setup lang="ts">
import { ref } from "vue";
import DashboardLayout from "../../components/layout/DashboardLayout.vue";
import StatsCards from "../../components/specialist/StatsCards.vue";
import TodaySchedule from "../../components/specialist/TodaySchedule.vue";
import AssignedReferralsTable from "../../components/specialist/AssignedReferralsTable.vue";
import { specialistNavLinks } from "../../config/navigation";
import { mockReferrals } from "../../data/mockReferrals";
import type { StatCardItem } from "../../components/specialist/StatsCards.vue";
import type { ScheduleItem } from "../../components/specialist/TodaySchedule.vue";

const user = ref({
  name: "Dr. James Rivera",
  welcomeName: "Dr. Rivera",
  role: "Cardiologist",
  initials: "JR",
});

const stats = ref<StatCardItem[]>([
  {
    label: "New Referrals",
    value: 2,
    icon: "clipboard",
    iconBg: "bg-blue-50",
    iconColor: "text-blue-600",
  },
  {
    label: "Pending Review",
    value: 1,
    subtext: "Action needed",
    icon: "clock",
    iconBg: "bg-amber-50",
    iconColor: "text-amber-500",
  },
  {
    label: "Accepted",
    value: 3,
    subtext: "Active cases",
    icon: "check",
    iconBg: "bg-green-50",
    iconColor: "text-green-600",
  },
  {
    label: "Scheduled",
    value: 5,
    subtext: "This week",
    icon: "calendar",
    iconBg: "bg-blue-50",
    iconColor: "text-blue-600",
  },
  {
    label: "Completed",
    value: 18,
    subtext: "This month",
    icon: "archive",
    iconBg: "bg-green-50",
    iconColor: "text-green-600",
  },
]);

const scheduleDate = ref("Jun 16");

const scheduleItems = ref<ScheduleItem[]>([
  {
    time: "9:00 AM",
    duration: "45 min",
    patientName: "Thomas Anderson",
    appointmentType: "Consultation",
  },
  {
    time: "10:30 AM",
    duration: "30 min",
    patientName: "Margaret Thompson",
    appointmentType: "Follow-up",
  },
  {
    time: "2:00 PM",
    duration: "60 min",
    patientName: "Robert Chen",
    appointmentType: "Initial Assessment",
  },
  {
    time: "3:30 PM",
    duration: "30 min",
    patientName: "Sarah Williams",
    appointmentType: "Consultation",
  },
]);

const referrals = ref(mockReferrals.slice(0, 4));
</script>

<template>
  <DashboardLayout
    :nav-links="specialistNavLinks"
    :user="user"
    title="Dashboard"
    :subtitle="`Welcome back, ${user.welcomeName}`"
    :notification-count="2"
  >
    <div class="space-y-6">
      <StatsCards :items="stats" />

      <TodaySchedule :date-label="scheduleDate" :items="scheduleItems" />

      <AssignedReferralsTable
        :referrals="referrals"
        view-all-link="/specialist/referrals"
      />
    </div>
  </DashboardLayout>
</template>
