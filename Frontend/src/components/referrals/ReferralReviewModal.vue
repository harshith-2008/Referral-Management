<script setup lang="ts">
import type { SpecialistPatientDTO } from "../../types/referral";
import UrgencyBadge from "./UrgencyBadge.vue";

defineProps<{
  referral: SpecialistPatientDTO;
}>();

defineEmits<{
  close: [];
}>();
</script>

<template>
  <div
    class="fixed inset-0 z-50 flex items-start justify-center overflow-y-auto bg-slate-900/40 p-8"
    @click.self="$emit('close')"
  >
    <div class="w-full max-w-3xl">
      <!-- Header -->

      <div class="rounded-xl border border-slate-100 bg-white p-6 shadow-lg">
        <div class="flex items-start justify-between gap-4">
          <div>
            <div class="flex items-center gap-3">
              <span class="text-sm font-semibold text-slate-700">
                Referral #{{ referral.referralId }}
              </span>

              <UrgencyBadge :urgency="referral.urgency" />
            </div>

            <h2 class="mt-3 text-2xl font-bold text-slate-900">
              {{ referral.patientName }}
            </h2>
          </div>

          <button
            type="button"
            class="rounded-lg p-2 text-slate-400 hover:bg-slate-100 hover:text-slate-600"
            @click="$emit('close')"
          >
            <svg viewBox="0 0 24 24" fill="none" class="h-5 w-5">
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

      <!-- Patient Summary -->

      <div
        class="mt-4 rounded-xl border border-slate-100 bg-white p-6 shadow-lg"
      >
        <h3 class="mb-5 text-base font-bold text-slate-900">Patient Summary</h3>

        <div class="rounded-xl bg-slate-50 p-5">
          <p class="text-base font-bold text-slate-900">
            {{ referral.patientName }}
          </p>

          <p class="mt-1 text-sm text-slate-500">MRN: {{ referral.mrn }}</p>

          <p class="mt-1 text-sm text-slate-500">Age: {{ referral.age }}</p>

          <p class="mt-1 text-sm text-slate-500">
            Gender: {{ referral.gender }}
          </p>
        </div>

        <div class="mt-5">
          <p class="text-sm font-semibold text-slate-700">Specialty</p>

          <p class="mt-2 text-sm text-slate-600">
            {{ referral.specialty }}
          </p>
        </div>

        <div class="mt-5">
          <p class="text-sm font-semibold text-slate-700">Urgency</p>

          <div class="mt-2">
            <UrgencyBadge :urgency="referral.urgency" />
          </div>
        </div>

        <div v-if="referral.diagnosisCode" class="mt-5">
          <p class="text-sm font-semibold text-slate-700">Diagnosis Code</p>

          <p class="mt-2 text-sm text-slate-600">
            {{ referral.diagnosisCode }}
          </p>
        </div>

        <div v-if="referral.appointmentDate" class="mt-5">
          <p class="text-sm font-semibold text-slate-700">Appointment Date</p>

          <p class="mt-2 text-sm text-slate-600">
            {{ referral.appointmentDate }}
          </p>
        </div>
      </div>
    </div>
  </div>
</template>
