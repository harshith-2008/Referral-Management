<script setup lang="ts">
import { computed, onMounted, ref } from "vue";

import DashboardLayout from "../../components/layout/DashboardLayout.vue";
import StatsCards from "../../components/specialist/StatsCards.vue";
import TodaySchedule from "../../components/specialist/TodaySchedule.vue";
import AssignedReferralsTable from "../../components/specialist/AssignedReferralsTable.vue";

import { specialistNavLinks } from "../../config/navigation";

import { getAssignedPatients } from "../../api/specialist";
import { getSchedule } from "../../api/appointment";

import { getErrorMessage } from "../../utils/errorHandler";

import type { StatCardItem } from "../../components/specialist/StatsCards.vue";
import type { SpecialistPatientDTO } from "../../types/referral";
import type { AppointmentScheduleDTO } from "../../types/appointment";

const loading = ref(false);

const errorMessage = ref("");
const infoMessage = ref("");

const scheduleDate = ref(new Date().toLocaleDateString());

const referrals = ref<SpecialistPatientDTO[]>([]);
const scheduleItems = ref<AppointmentScheduleDTO[]>([]);

const loadDashboard = async () => {
  loading.value = true;

  errorMessage.value = "";
  infoMessage.value = "";

  try {
    const today = new Date().toISOString().split("T")[0];

    const results = await Promise.allSettled([
      getAssignedPatients(),
      getSchedule(today),
    ]);

    const referralsResult = results[0];
    const scheduleResult = results[1];

    // Referrals
    if (referralsResult.status === "fulfilled") {
      referrals.value = referralsResult.value.data.data ?? [];
    } else {
      referrals.value = [];

      errorMessage.value = getErrorMessage(referralsResult.reason);
    }

    // Schedule
    if (scheduleResult.status === "fulfilled") {
      scheduleItems.value = scheduleResult.value.data.data?.appointments ?? [];
    } else {
      scheduleItems.value = [];

      errorMessage.value =
        errorMessage.value || getErrorMessage(scheduleResult.reason);
    }

    if (
      referrals.value.length === 0 &&
      scheduleItems.value.length === 0 &&
      !errorMessage.value
    ) {
      infoMessage.value =
        "No referrals or appointments are available at the moment.";
    }
  } catch (error) {
    referrals.value = [];
    scheduleItems.value = [];

    errorMessage.value = getErrorMessage(error);
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
    :nav-links="specialistNavLinks"
    title="Dashboard"
    subtitle="Manage your assigned referrals and appointments"
    :notification-count="2"
  >
    <div class="space-y-6">
      <div
        v-if="errorMessage"
        class="rounded-lg border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-700"
      >
        {{ errorMessage }}
      </div>

      <div
        v-if="infoMessage"
        class="rounded-lg border border-blue-200 bg-blue-50 px-4 py-3 text-sm text-blue-700"
      >
        {{ infoMessage }}
      </div>

      <div
        v-if="loading"
        class="rounded-xl border border-slate-200 bg-white p-8 text-center text-slate-500"
      >
        Loading dashboard...
      </div>

      <template v-else>
        <StatsCards :items="stats" />

        <TodaySchedule :date-label="scheduleDate" :items="scheduleItems" />

        <AssignedReferralsTable
          :referrals="referrals"
          view-all-link="/specialist/appointments"
        />
      </template>
    </div>
  </DashboardLayout>
</template>
