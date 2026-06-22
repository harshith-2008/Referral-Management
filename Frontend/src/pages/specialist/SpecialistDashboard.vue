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

const user = ref({
  name: "Dr. James Rivera",
  welcomeName: "Dr. Rivera",
  role: "Cardiologist",
  initials: "JR",
});

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

    scheduleItems.value = scheduleResponse.data.data.appointments ?? [];
  } catch (error) {
    console.error(error);

    referrals.value = [
      {
        referralId: 101,
        patientId: 1,
        patientName: "John Smith",
        age: 45,
        gender: "Male",
        mrn: "MRN001",
        specialty: "Cardiology",
        urgency: "Urgent",
        diagnosisCode: "I20",
        appointmentDate: "2026-06-22",
      },
      {
        referralId: 102,
        patientId: 2,
        patientName: "Sarah Johnson",
        age: 60,
        gender: "Female",
        mrn: "MRN002",
        specialty: "Neurology",
        urgency: "Routine",
        diagnosisCode: "G40",
      },
      {
        referralId: 103,
        patientId: 3,
        patientName: "Michael Brown",
        age: 38,
        gender: "Male",
        mrn: "MRN003",
        specialty: "Orthopedics",
        urgency: "Emergency",
        diagnosisCode: "M17",
      },
    ];

    scheduleItems.value = [
      {
        appointmentId: 1,
        appointmentTime: "09:00",
        patientName: "John Smith",
        mrn: "MRN001",
        appointmentStatus: "Scheduled",
      },
      {
        appointmentId: 2,
        appointmentTime: "10:30",
        patientName: "Sarah Johnson",
        mrn: "MRN002",
        appointmentStatus: "Scheduled",
      },
      {
        appointmentId: 3,
        appointmentTime: "14:00",
        patientName: "Michael Brown",
        mrn: "MRN003",
        appointmentStatus: "Scheduled",
      },
    ];
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
      (x) => x.urgency === "Urgent" || x.urgency === "Emergency"
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
