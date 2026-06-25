<script setup lang="ts">
import { computed, onMounted, ref } from "vue";

import DashboardLayout from "../../components/layout/DashboardLayout.vue";
import StatsCards from "../../components/specialist/StatsCards.vue";
import TodaySchedule from "../../components/specialist/TodaySchedule.vue";
import AssignedReferralsTable from "../../components/specialist/AssignedReferralsTable.vue";

import { specialistNavLinks } from "../../config/navigation";

import { getAssignedPatients } from "../../api/specialist";
import { getSchedule } from "../../api/appointment";

import type { StatCardItem } from "../../components/specialist/StatsCards.vue";
import type { SpecialistPatientDTO } from "../../types/referral";
import type { AppointmentScheduleDTO } from "../../types/appointment";

const loading = ref(false);

const scheduleDate = ref(new Date().toLocaleDateString());

const referrals = ref<SpecialistPatientDTO[]>([]);
const scheduleItems = ref<AppointmentScheduleDTO[]>([]);

const loadDashboard = async () => {
  loading.value = true;

  try {
    const today = new Date().toISOString().split("T")[0];

    const [referralsResponse, scheduleResponse] = await Promise.all([
      getAssignedPatients(),
      getSchedule(today),
    ]);

    referrals.value = referralsResponse.data.data ?? [];

    scheduleItems.value = scheduleResponse.data.data?.appointments ?? [];
  } catch (error) {
    console.error("Dashboard load failed:", error);

    referrals.value = [];
    scheduleItems.value = [];
  } finally {
    loading.value = false;
  }
};

const stats = computed<StatCardItem[]>(() => [
  {
    label: "Assigned Referrals",
    value: referrals.value.length,
    icon: "clipboard",
    iconBg: "bg-blue-50",
    iconColor: "text-blue-600",
  },
  {
    label: "Urgent Cases",
    value: referrals.value.filter(
      (x) => x.urgency === "Urgent" || x.urgency === "Emergency",
    ).length,
    icon: "clock",
    iconBg: "bg-amber-50",
    iconColor: "text-amber-500",
  },
  {
    label: "Appointments Today",
    value: scheduleItems.value.length,
    icon: "calendar",
    iconBg: "bg-blue-50",
    iconColor: "text-blue-600",
  },
  {
    label: "Scheduled Referrals",
    value: referrals.value.filter((x) => x.appointmentDate).length,
    icon: "check",
    iconBg: "bg-green-50",
    iconColor: "text-green-600",
  },
  {
    label: "Total Patients",
    value: new Set(referrals.value.map((x) => x.patientId)).size,
    icon: "users",
    iconBg: "bg-purple-50",
    iconColor: "text-purple-600",
  },
]);

onMounted(loadDashboard);
</script>

<template>
  <DashboardLayout
    v-if="!loading"
    :nav-links="specialistNavLinks"
    title="Dashboard"
    subtitle="Manage your assigned referrals and appointments"
    :notification-count="2"
  >
    <div class="space-y-6">
      <StatsCards :items="stats" />

      <TodaySchedule :date-label="scheduleDate" :items="scheduleItems" />

      <AssignedReferralsTable
        :referrals="referrals"
        view-all-link="/specialist/appointments"
      />
    </div>
  </DashboardLayout>

  <div v-else class="p-8 text-center text-slate-500">Loading dashboard...</div>
</template>
