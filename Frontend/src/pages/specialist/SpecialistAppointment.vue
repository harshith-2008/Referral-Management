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

const loadAppointments = async () => {
  try {
    loading.value = true;

    const response = await getSchedule(selectedDate.value);

    appointments.value = response.data.data.appointments;
  } catch (error) {
    alert(getErrorMessage(error));
  } finally {
    loading.value = false;
  }
};

const openDetails = async (appointment: any) => {
  try {
    const response = await getAppointmentDetails(appointment.appointmentId);

    selectedAppointment.value = response.data.data;
  } catch (error) {
    alert(getErrorMessage(error));
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
    :user="user"
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

    <AppointmentsTable :appointments="appointments" @view="openDetails" />

    <AppointmentDetailsModal
      v-if="selectedAppointment"
      :appointment="selectedAppointment"
      @close="closeDetails"
    />
  </DashboardLayout>
</template>
