<script setup lang="ts">
import { formatDate, formatTime } from "../../utils/date";

export interface AppointmentDetailsDTO {
  appointmentId: number;
  referralId: number;
  appointmentDate: string;
  appointmentTime: string;
  appointmentStatus: string;
  patientName: string;
  mrn: string;
  specialistName: string;
  referralReason: string;
}

defineProps<{
  appointment: AppointmentDetailsDTO;
}>();

defineEmits<{
  close: [];
}>();
</script>

<template>
  <div
    class="fixed inset-0 z-50 flex items-start justify-center overflow-y-auto bg-black/50 backdrop-blur-sm p-6 sm:p-10"
    @click.self="$emit('close')"
  >
    <div class="w-full max-w-3xl space-y-4 pb-10">
      <!-- Header -->
      <div
        class="bg-white rounded-2xl shadow-2xl px-6 py-5 flex items-start justify-between gap-4"
      >
        <div>
          <div class="flex items-center gap-2 mb-3">
            <span
              class="text-xs font-medium tracking-widest text-slate-400 uppercase"
              >Appointment #{{ appointment.appointmentId }}</span
            >
            <span class="text-slate-200">·</span>
            <span class="text-xs font-medium text-slate-400"
              >Referral #{{ appointment.referralId }}</span
            >
          </div>
          <h2 class="text-2xl font-bold text-slate-900 leading-tight">
            {{ appointment.patientName }}
          </h2>
          <p class="mt-1 text-sm text-slate-400">
            {{ appointment.specialistName }}
          </p>
        </div>

        <div class="flex items-center gap-3 shrink-0 mt-1">
          <!-- Status pill -->
          <span
            class="inline-flex items-center px-3 py-1 rounded-full text-xs font-semibold bg-blue-50 text-blue-600 border border-blue-100"
          >
            {{ appointment.appointmentStatus }}
          </span>

          <button
            type="button"
            class="w-8 h-8 flex items-center justify-center rounded-full text-slate-400 hover:text-slate-700 hover:bg-slate-100 transition-colors"
            @click="$emit('close')"
          >
            <svg viewBox="0 0 24 24" fill="none" class="h-4 w-4">
              <path
                d="M18 6L6 18M6 6l12 12"
                stroke="currentColor"
                stroke-width="2"
                stroke-linecap="round"
              />
            </svg>
          </button>
        </div>
      </div>

      <!-- Body -->
      <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
        <!-- Patient Information -->
        <div class="bg-white rounded-2xl shadow-xl px-6 py-5">
          <p
            class="text-xs font-medium tracking-widest text-slate-400 uppercase mb-4"
          >
            Patient Information
          </p>

          <!-- Avatar strip -->
          <div
            class="flex items-center gap-3 rounded-xl bg-slate-50 border border-slate-100 px-4 py-3 mb-5"
          >
            <div
              class="w-9 h-9 rounded-full bg-blue-100 flex items-center justify-center text-blue-600 font-bold text-sm shrink-0"
            >
              {{ appointment.patientName?.charAt(0) }}
            </div>
            <div>
              <p class="text-sm font-semibold text-slate-900 leading-tight">
                {{ appointment.patientName }}
              </p>
              <p class="text-xs text-slate-400 mt-0.5">
                MRN {{ appointment.mrn }}
              </p>
            </div>
          </div>

          <div>
            <p
              class="text-[10px] font-medium tracking-wider text-slate-400 uppercase mb-1"
            >
              Specialist
            </p>
            <div class="flex items-center gap-2">
              <div
                class="w-6 h-6 rounded-full bg-slate-100 flex items-center justify-center text-slate-500 text-xs font-bold shrink-0"
              >
                {{ appointment.specialistName?.charAt(0) }}
              </div>
              <p class="text-sm font-medium text-slate-800">
                {{ appointment.specialistName }}
              </p>
            </div>
          </div>
        </div>

        <!-- Appointment Details -->
        <div class="bg-white rounded-2xl shadow-xl px-6 py-5">
          <p
            class="text-xs font-medium tracking-widest text-slate-400 uppercase mb-4"
          >
            Appointment Details
          </p>

          <div class="space-y-4">
            <!-- Date + Time side by side -->
            <div
              class="grid grid-cols-2 gap-4 rounded-xl bg-slate-50 border border-slate-100 px-4 py-3"
            >
              <div>
                <p
                  class="text-[10px] font-medium tracking-wider text-slate-400 uppercase mb-1"
                >
                  Date
                </p>
                <p class="text-sm font-semibold text-slate-900">
                  {{ formatDate(appointment.appointmentDate) }}
                </p>
              </div>
              <div>
                <p
                  class="text-[10px] font-medium tracking-wider text-slate-400 uppercase mb-1"
                >
                  Time
                </p>
                <p class="text-sm font-semibold text-slate-900">
                  {{ formatTime(appointment.appointmentTime) }}
                </p>
              </div>
            </div>

            <div>
              <p
                class="text-[10px] font-medium tracking-wider text-slate-400 uppercase mb-1"
              >
                Status
              </p>
              <span
                class="inline-flex items-center px-2.5 py-1 rounded-full text-xs font-semibold bg-blue-50 text-blue-600 border border-blue-100"
              >
                {{ appointment.appointmentStatus }}
              </span>
            </div>

            <div>
              <p
                class="text-[10px] font-medium tracking-wider text-slate-400 uppercase mb-1"
              >
                Referral Reason
              </p>
              <p class="text-sm text-slate-700 leading-relaxed">
                {{ appointment.referralReason }}
              </p>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
