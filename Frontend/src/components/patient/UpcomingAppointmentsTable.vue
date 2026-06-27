<script setup lang="ts">
import type { AppointmentDTO } from "../../types/patient";
import { formatDate, formatTime } from "../../utils/date";

defineProps<{
  appointments: AppointmentDTO[];
}>();
</script>

<template>
  <div class="rounded-xl border border-slate-100 bg-white shadow-sm">
    <div
      class="flex items-center justify-between border-b border-slate-100 px-6 py-5"
    >
      <h2 class="text-lg font-bold text-slate-900">
        Upcoming Appointments
      </h2>
    </div>

    <table class="w-full">
      <thead>
        <tr class="bg-slate-50">
          <th class="px-6 py-3 text-left text-xs font-semibold uppercase">
            Date
          </th>

          <th class="px-6 py-3 text-left text-xs font-semibold uppercase">
            Time
          </th>

          <th class="px-6 py-3 text-left text-xs font-semibold uppercase">
            Specialist
          </th>

          <th class="px-6 py-3 text-left text-xs font-semibold uppercase">
            Status
          </th>
        </tr>
      </thead>

      <tbody>
        <tr
          v-for="appointment in appointments"
          :key="appointment.appointmentId"
          class="border-t hover:bg-slate-50"
        >
          <td class="px-6 py-4">
            {{ formatDate(appointment.appointmentDate) }}
          </td>

          <td class="px-6 py-4">
            {{ formatTime(appointment.appointmentTime) }}
          </td>

          <td class="px-6 py-4">
            {{ appointment.specialistName }}
          </td>

          <td class="px-6 py-4">
            {{ appointment.appointmentStatus }}
          </td>
        </tr>
      </tbody>
    </table>
  </div>
</template>
