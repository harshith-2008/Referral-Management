<script setup lang="ts">
import {
  referralStatusDefinitions,
  referralStatusOrder,
} from "../../types/coordinatorReferral";

import type { ReferralDTO } from "../../types/referral";

import CoordinatorStatusBadge from "./CoordinatorStatusBadge.vue";
import CoordinatorUrgencyBadge from "./CoordinatorUrgencyBadge.vue";

defineProps<{
  referral: ReferralDTO;
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
    <div class="w-full max-w-4xl space-y-4 pb-10">
      <!-- Header Card -->
      <div
        class="bg-white rounded-2xl shadow-2xl px-6 py-5 flex items-start justify-between gap-4"
      >
        <div>
          <div class="flex flex-wrap items-center gap-2 mb-3">
            <span
              class="text-xs font-medium tracking-widest text-slate-400 uppercase"
              >REF-{{ referral.referralId }}</span
            >
            <CoordinatorStatusBadge :status="referral.status as any" />
            <CoordinatorUrgencyBadge :urgency="referral.urgency as any" />
          </div>
          <h2 class="text-2xl font-bold text-slate-900 leading-tight">
            {{ referral.patientName }}
          </h2>
          <p class="mt-1 text-sm text-slate-400">{{ referral.specialty }}</p>
        </div>

        <button
          type="button"
          class="w-8 h-8 flex items-center justify-center rounded-full text-slate-400 hover:text-slate-700 hover:bg-slate-100 transition-colors shrink-0 mt-1"
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

      <!-- Body: 2 col -->
      <div class="grid grid-cols-1 sm:grid-cols-5 gap-4">
        <!-- Left: Referral Details -->
        <div class="sm:col-span-2 bg-white rounded-2xl shadow-xl px-6 py-5">
          <p
            class="text-xs font-medium tracking-widest text-slate-400 uppercase mb-4"
          >
            Referral Details
          </p>

          <div class="space-y-4">
            <div
              v-for="item in [
                { label: 'Patient Name', value: referral.patientName },
                { label: 'Origin Facility', value: referral.originFacility },
                {
                  label: 'Destination Facility',
                  value: referral.destinationFacility,
                },
                { label: 'Specialty', value: referral.specialty },
                {
                  label: 'Diagnosis Code',
                  value: referral.diagnosisCode || 'N/A',
                },
                {
                  label: 'Created At',
                  value: new Date(referral.createdAt).toLocaleString(),
                },
              ]"
              :key="item.label"
            >
              <p
                class="text-[10px] font-medium tracking-wider text-slate-400 uppercase mb-0.5"
              >
                {{ item.label }}
              </p>
              <p class="text-sm text-slate-800 font-medium">{{ item.value }}</p>
            </div>

            <div>
              <p
                class="text-[10px] font-medium tracking-wider text-slate-400 uppercase mb-1.5"
              >
                Status
              </p>
              <CoordinatorStatusBadge :status="referral.status as any" />
            </div>

            <div>
              <p
                class="text-[10px] font-medium tracking-wider text-slate-400 uppercase mb-1.5"
              >
                Urgency
              </p>
              <CoordinatorUrgencyBadge :urgency="referral.urgency as any" />
            </div>
          </div>
        </div>

        <!-- Right: Status Panel -->
        <div
          class="sm:col-span-3 bg-white rounded-2xl shadow-xl px-6 py-5 flex flex-col gap-6"
        >
          <!-- Current State -->
          <div>
            <p
              class="text-xs font-medium tracking-widest text-slate-400 uppercase mb-3"
            >
              Current State
            </p>
            <div
              class="rounded-xl border border-slate-100 bg-slate-50 px-5 py-4 space-y-3"
            >
              <div class="flex flex-wrap items-center gap-2">
                <CoordinatorStatusBadge :status="referral.status as any" />
                <CoordinatorUrgencyBadge :urgency="referral.urgency as any" />
              </div>

              <p class="text-sm text-slate-700">
                This referral is currently in
                <span class="font-semibold text-slate-900">{{
                  referral.status
                }}</span>
                status.
              </p>

              <div
                class="flex items-center justify-between pt-1 border-t border-slate-200"
              >
                <div>
                  <p
                    class="text-[10px] font-medium tracking-wider text-slate-400 uppercase mb-0.5"
                  >
                    Specialty Requested
                  </p>
                  <p class="text-sm font-medium text-slate-800">
                    {{ referral.specialty }}
                  </p>
                </div>
                <div class="text-right">
                  <p
                    class="text-[10px] font-medium tracking-wider text-slate-400 uppercase mb-0.5"
                  >
                    Created
                  </p>
                  <p class="text-xs text-slate-500">
                    {{ new Date(referral.createdAt).toLocaleString() }}
                  </p>
                </div>
              </div>
            </div>
          </div>

          <!-- Status Definitions -->
          <div>
            <p
              class="text-xs font-medium tracking-widest text-slate-400 uppercase mb-3"
            >
              Status Definitions
            </p>
            <div
              class="rounded-xl border border-slate-100 bg-slate-50 px-5 py-4"
            >
              <div class="grid grid-cols-1 sm:grid-cols-2 gap-x-6 gap-y-3">
                <div
                  v-for="status in referralStatusOrder"
                  :key="status"
                  class="flex flex-col"
                  :class="
                    referral.status === status ? 'opacity-100' : 'opacity-60'
                  "
                >
                  <span
                    class="text-[10px] font-semibold tracking-wider uppercase mb-0.5"
                    :class="
                      referral.status === status
                        ? 'text-blue-600'
                        : 'text-slate-500'
                    "
                  >
                    {{ status }}
                  </span>
                  <span class="text-xs text-slate-600 leading-snug">
                    {{ referralStatusDefinitions[status].description }}
                  </span>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
