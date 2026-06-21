<script setup lang="ts">
import type { Referral } from "../../types/referral";
import UrgencyBadge from "./UrgencyBadge.vue";
import StatusBadge from "./StatusBadge.vue";

defineProps<{
  referral: Referral;
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
      <!-- Header card -->
      <div class="rounded-xl border border-slate-100 bg-white p-6 shadow-lg">
        <div class="flex items-start justify-between gap-4">
          <div>
            <div class="flex flex-wrap items-center gap-2">
              <span class="text-sm font-semibold text-slate-700">{{ referral.id }}</span>
              <StatusBadge :status="referral.status" />
              <UrgencyBadge :urgency="referral.urgency" />
            </div>
            <h2 class="mt-3 text-2xl font-bold text-slate-900">{{ referral.reviewTitle }}</h2>
          </div>

          <button
            type="button"
            class="rounded-lg p-2 text-slate-400 transition-colors hover:bg-slate-100 hover:text-slate-600"
            aria-label="Close"
            @click="$emit('close')"
          >
            <svg viewBox="0 0 24 24" fill="none" class="h-5 w-5" aria-hidden="true">
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

      <!-- Patient summary -->
      <div class="mt-4 rounded-xl border border-slate-100 bg-white p-6 shadow-lg">
        <div class="mb-5 flex items-center gap-2">
          <svg viewBox="0 0 24 24" fill="none" class="h-5 w-5 text-slate-500" aria-hidden="true">
            <circle cx="12" cy="8" r="3.5" stroke="currentColor" stroke-width="1.75" />
            <path
              d="M5 20c0-3.3 2.7-6 6-6s6 2.7 6 6"
              stroke="currentColor"
              stroke-width="1.75"
              stroke-linecap="round"
            />
          </svg>
          <h3 class="text-base font-bold text-slate-900">Patient Summary</h3>
        </div>

        <div class="rounded-xl bg-slate-50 p-5">
          <p class="text-base font-bold text-slate-900">{{ referral.patientName }}</p>
          <p class="mt-1 text-sm text-slate-500">
            DOB: {{ referral.dob }} · MRN: {{ referral.mrn }}
          </p>
          <p class="mt-1 text-sm text-slate-500">{{ referral.insurance }}</p>
        </div>

        <div class="mt-5">
          <p class="text-sm font-semibold text-slate-700">Primary Diagnosis</p>
          <p class="mt-2 text-sm text-slate-600">{{ referral.primaryDiagnosis }}</p>
        </div>

        <div class="mt-5">
          <p class="text-sm font-semibold text-slate-700">Referral Reason</p>
          <p class="mt-2 text-sm leading-relaxed text-slate-600">{{ referral.referralReason }}</p>
        </div>
      </div>
    </div>
  </div>
</template>
