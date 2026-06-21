<script setup lang="ts">
import type { CoordinatorReferral } from "../../types/coordinatorReferral";
import { referralStatusDefinitions, referralStatusOrder } from "../../types/coordinatorReferral";
import CoordinatorStatusBadge from "./CoordinatorStatusBadge.vue";
import CoordinatorUrgencyBadge from "./CoordinatorUrgencyBadge.vue";

defineProps<{
  referral: CoordinatorReferral;
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
    <div class="w-full max-w-4xl">
      <!-- Header -->
      <div class="rounded-xl border border-slate-100 bg-white p-6 shadow-lg">
        <div class="flex items-start justify-between gap-4">
          <div>
            <div class="flex flex-wrap items-center gap-2">
              <span class="text-sm font-semibold text-slate-700">{{ referral.id }}</span>
              <CoordinatorStatusBadge :status="referral.status" />
              <CoordinatorUrgencyBadge :urgency="referral.urgency" />
            </div>
            <h2 class="mt-3 text-2xl font-bold text-slate-900">
              {{ referral.patientName }}
            </h2>
            <p class="mt-1 text-sm text-slate-500">{{ referral.hospitalBranch }}</p>
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

      <div class="mt-4 grid grid-cols-5 gap-4">
        <!-- Patient info -->
        <div class="col-span-2 rounded-xl border border-slate-100 bg-white p-6 shadow-lg">
          <h3 class="text-base font-bold text-slate-900">Patient Information</h3>

          <div class="mt-5 space-y-4">
            <div>
              <p class="text-xs font-semibold uppercase tracking-wide text-slate-500">Patient Name</p>
              <p class="mt-1 text-sm font-medium text-slate-900">{{ referral.patientName }}</p>
            </div>
            <div>
              <p class="text-xs font-semibold uppercase tracking-wide text-slate-500">Date of Birth</p>
              <p class="mt-1 text-sm text-slate-700">{{ referral.dob }}</p>
            </div>
            <div>
              <p class="text-xs font-semibold uppercase tracking-wide text-slate-500">MRN</p>
              <p class="mt-1 text-sm text-slate-700">{{ referral.mrn }}</p>
            </div>
            <div>
              <p class="text-xs font-semibold uppercase tracking-wide text-slate-500">Insurance</p>
              <p class="mt-1 text-sm text-slate-700">{{ referral.insurance }}</p>
            </div>
            <div>
              <p class="text-xs font-semibold uppercase tracking-wide text-slate-500">Origin Specialist</p>
              <p class="mt-1 text-sm text-slate-700">{{ referral.originSpecialist }}</p>
            </div>
            <div>
              <p class="text-xs font-semibold uppercase tracking-wide text-slate-500">Destination Hospital</p>
              <p class="mt-1 text-sm text-slate-700">{{ referral.destinationHospital }}</p>
            </div>
            <div>
              <p class="text-xs font-semibold uppercase tracking-wide text-slate-500">Primary Diagnosis</p>
              <p class="mt-1 text-sm text-slate-700">{{ referral.primaryDiagnosis }}</p>
            </div>
            <div>
              <p class="text-xs font-semibold uppercase tracking-wide text-slate-500">Referral Reason</p>
              <p class="mt-1 text-sm leading-relaxed text-slate-600">{{ referral.referralReason }}</p>
            </div>
          </div>
        </div>

        <!-- Referral history timeline -->
        <div class="col-span-3 rounded-xl border border-slate-100 bg-white p-6 shadow-lg">
          <h3 class="text-base font-bold text-slate-900">Referral History</h3>
          <p class="mt-1 text-sm text-slate-500">
            Chronological record of this referral from creation to its current state.
          </p>

          <!-- Status definitions -->
          <div class="mt-5 rounded-xl bg-slate-50 p-4">
            <p class="text-xs font-semibold uppercase tracking-wide text-slate-500">Status definitions</p>
            <div class="mt-3 grid grid-cols-2 gap-x-4 gap-y-2">
              <div
                v-for="status in referralStatusOrder"
                :key="status"
                class="text-xs text-slate-600"
              >
                <span class="font-semibold text-slate-800">{{ status }} —</span>
                {{ referralStatusDefinitions[status].description }}
              </div>
            </div>
          </div>

          <!-- Event timeline -->
          <div class="mt-6">
            <p class="mb-4 text-xs font-semibold uppercase tracking-wide text-slate-500">
              Timeline
            </p>

            <div class="space-y-0">
              <div
                v-for="(event, index) in referral.history"
                :key="`${event.status}-${event.timestamp}`"
                class="relative flex gap-4 pb-8 last:pb-0"
              >
                <div
                  v-if="index < referral.history.length - 1"
                  class="absolute left-[11px] top-6 h-full w-0.5 bg-blue-200"
                />

                <div
                  class="relative z-10 flex h-6 w-6 shrink-0 items-center justify-center rounded-full border-2"
                  :class="
                    index === referral.history.length - 1
                      ? 'border-blue-600 bg-blue-600'
                      : 'border-green-500 bg-green-500'
                  "
                >
                  <svg
                    v-if="index < referral.history.length - 1"
                    viewBox="0 0 24 24"
                    fill="none"
                    class="h-3 w-3 text-white"
                    aria-hidden="true"
                  >
                    <path d="M6 12l4 4 8-8" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" />
                  </svg>
                  <span v-else class="h-2 w-2 rounded-full bg-white" />
                </div>

                <div class="min-w-0 flex-1">
                  <div class="flex flex-wrap items-center gap-2">
                    <CoordinatorStatusBadge :status="event.status" />
                    <span
                      v-if="index === referral.history.length - 1"
                      class="rounded-full bg-blue-50 px-2 py-0.5 text-[10px] font-semibold uppercase text-blue-600"
                    >
                      Latest
                    </span>
                  </div>
                  <p class="mt-2 text-sm font-semibold text-slate-900">{{ event.title }}</p>
                  <p class="mt-1 text-sm text-slate-600">{{ event.description }}</p>
                  <p class="mt-2 text-xs text-slate-400">{{ event.timestamp }}</p>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
