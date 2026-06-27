<script setup lang="ts">
import { computed, ref } from "vue";
import { formatTime } from "../../utils/date";

export interface AppointmentScheduleDTO {
  appointmentId: number;
  appointmentTime: string;
  patientName: string;
  mrn: string;
  appointmentStatus: string;
  referralId?: number;
}

const props = defineProps<{
  appointments: AppointmentScheduleDTO[];
  completingAppointmentId?: number | null;
}>();

const emit = defineEmits<{
  view: [appointment: AppointmentScheduleDTO];
  complete: [appointment: AppointmentScheduleDTO];
}>();

const searchQuery = ref("");
const statusFilter = ref("All");

const statusOptions = ["All", "Scheduled", "Completed", "Cancelled"];

const canComplete = (appointment: AppointmentScheduleDTO) =>
  !["Completed", "Cancelled"].includes(appointment.appointmentStatus);

const filteredAppointments = computed(() => {
  const query = searchQuery.value.toLowerCase().trim();

  return props.appointments.filter((appointment) => {
    const matchesSearch =
      !query ||
      appointment.patientName.toLowerCase().includes(query) ||
      appointment.mrn.toLowerCase().includes(query) ||
      appointment.appointmentId.toString().includes(query);

    const matchesStatus =
      statusFilter.value === "All" ||
      appointment.appointmentStatus === statusFilter.value;

    return matchesSearch && matchesStatus;
  });
});
</script>

<template>
  <div class="rounded-xl border border-slate-100 bg-white shadow-sm">
    <div class="border-b border-slate-100 px-6 py-4">
      <div class="flex items-center gap-4">
        <input
          v-model="searchQuery"
          type="text"
          placeholder="Search by patient, MRN or appointment ID..."
          class="flex-1 rounded-xl border border-slate-200 px-4 py-2.5 text-sm"
        />

        <select
          v-model="statusFilter"
          class="rounded-xl border border-slate-200 px-4 py-2.5 text-sm"
        >
          <option v-for="option in statusOptions" :key="option" :value="option">
            Status: {{ option }}
          </option>
        </select>

        <span class="rounded-full bg-slate-100 px-3 py-1 text-xs font-semibold">
          {{ filteredAppointments.length }} appointments
        </span>
      </div>
    </div>

    <div class="overflow-hidden">
      <table class="w-full">
        <thead>
          <tr class="border-b border-slate-100 bg-slate-50">
            <th class="px-6 py-3 text-left text-xs font-semibold">
              Appointment ID
            </th>

            <th class="px-6 py-3 text-left text-xs font-semibold">Time</th>

            <th class="px-6 py-3 text-left text-xs font-semibold">Patient</th>

            <th class="px-6 py-3 text-left text-xs font-semibold">MRN</th>

            <th class="px-6 py-3 text-left text-xs font-semibold">Status</th>

            <th class="px-6 py-3 text-left text-xs font-semibold">Actions</th>
          </tr>
        </thead>

        <tbody>
          <tr
            v-for="appointment in filteredAppointments"
            :key="appointment.appointmentId"
            class="border-b border-slate-100 hover:bg-slate-50"
          >
            <td class="px-6 py-4 font-medium text-blue-600">
              #{{ appointment.appointmentId }}
            </td>

            <td class="px-6 py-4 text-sm">
              {{ formatTime(appointment.appointmentTime) }}
            </td>

            <td class="px-6 py-4 font-medium">
              {{ appointment.patientName }}
            </td>

            <td class="px-6 py-4 text-sm text-slate-600">
              {{ appointment.mrn }}
            </td>

            <td class="px-6 py-4">
              <span
                class="rounded-full px-3 py-1 text-xs font-semibold"
                :class="
                  appointment.appointmentStatus === 'Scheduled'
                    ? 'bg-blue-50 text-blue-700'
                    : appointment.appointmentStatus === 'Completed'
                      ? 'bg-green-50 text-green-700'
                      : 'bg-red-50 text-red-700'
                "
              >
                {{ appointment.appointmentStatus }}
              </span>
            </td>

            <td class="px-6 py-4">
              <div class="flex items-center gap-2">
                <button
                  class="rounded-lg border border-blue-200 px-3 py-1.5 text-sm font-medium text-blue-600 hover:bg-blue-50"
                  @click="emit('view', appointment)"
                >
                  View
                </button>

                <button
                  v-if="canComplete(appointment)"
                  :disabled="props.completingAppointmentId === appointment.appointmentId"
                  class="rounded-lg border border-emerald-200 px-3 py-1.5 text-sm font-medium text-emerald-700 hover:bg-emerald-50"
                  :class="
                    props.completingAppointmentId === appointment.appointmentId
                      ? 'cursor-not-allowed opacity-60'
                      : ''
                  "
                  @click="emit('complete', appointment)"
                >
                  {{
                    props.completingAppointmentId === appointment.appointmentId
                      ? "Completing..."
                      : "Mark Completed"
                  }}
                </button>
              </div>
            </td>
          </tr>

          <tr v-if="filteredAppointments.length === 0">
            <td colspan="6" class="px-6 py-10 text-center text-slate-500">
              No appointments found.
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>
