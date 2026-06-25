<script setup lang="ts">
import { ref, onMounted } from "vue";

import DashboardLayout from "../../components/layout/DashboardLayout.vue";
import AppointmentDetailsModal from "../../components/specialist/AppointmentDetailsModal.vue";
import AppointmentsTable from "../../components/specialist/AppointmentsTable.vue";

import { specialistNavLinks } from "../../config/navigation";

import { getSchedule, getAppointmentDetails } from "../../api/appointment";

import { getErrorMessage } from "../../utils/errorHandler";

import type {
  AppointmentScheduleDTO,
  AppointmentDetailsDTO,
} from "../../types/appointment";

const user = ref({
  name: "Dr. James Rivera",
  welcomeName: "Dr. Rivera",
  role: "Cardiologist",
  initials: "JR",
});

const selectedDate = ref(new Date().toISOString().split("T")[0]);

const appointments = ref<AppointmentScheduleDTO[]>([]);

const selectedAppointment = ref<AppointmentDetailsDTO | null>(null);

const loading = ref(false);

const errorMessage = ref("");
const infoMessage = ref("");

const loadAppointments = async () => {
  errorMessage.value = "";
  infoMessage.value = "";

  try {
    loading.value = true;

    const response = await getSchedule(selectedDate.value);

    appointments.value = response.data.data?.appointments ?? [];

    if (appointments.value.length === 0) {
      infoMessage.value = "No appointments scheduled for the selected date.";
    }
  } catch (error) {
    appointments.value = [];
    errorMessage.value = getErrorMessage(error);
  } finally {
    loading.value = false;
  }
};

const openDetails = async (appointment: AppointmentScheduleDTO) => {
  errorMessage.value = "";

  try {
    const response = await getAppointmentDetails(appointment.appointmentId);

    selectedAppointment.value = response.data.data;
  } catch (error) {
    errorMessage.value = getErrorMessage(error);
  }
};

const closeDetails = () => {
  selectedAppointment.value = null;
};

onMounted(loadAppointments);
</script>

<template>
  <DashboardLayout
    :nav-links="specialistNavLinks"
    title="My Appointments"
    subtitle="Manage your daily schedule"
    :notification-count="2"
  >
    <div class="mb-6">
      <label class="block text-sm font-medium mb-2"> Select Date </label>

      <input
        v-model="selectedDate"
        type="date"
        class="rounded-lg border px-4 py-2"
        @change="loadAppointments"
      />
    </div>

    <div
      v-if="errorMessage"
      class="mb-4 rounded-lg border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-700"
    >
      {{ errorMessage }}
    </div>

    <div
      v-if="infoMessage"
      class="mb-4 rounded-lg border border-blue-200 bg-blue-50 px-4 py-3 text-sm text-blue-700"
    >
      {{ infoMessage }}
    </div>

    <div
      v-if="loading"
      class="rounded-xl border border-slate-200 bg-white p-8 text-center text-slate-500"
    >
      Loading appointments...
    </div>

    <AppointmentsTable
      v-else
      :appointments="appointments"
      @view="openDetails"
    />

    <AppointmentDetailsModal
      v-if="selectedAppointment"
      :appointment="selectedAppointment"
      @close="closeDetails"
    />
  </DashboardLayout>
</template>
