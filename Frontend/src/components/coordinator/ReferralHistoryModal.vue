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
    class="fixed inset-0 z-50 flex items-start justify-center overflow-y-auto bg-slate-900/40 p-8"
    @click.self="$emit('close')"
  >
    <div class="w-full max-w-4xl">
      <!-- Header -->
      <div class="rounded-xl border border-slate-100 bg-white p-6 shadow-lg">
        <div class="flex items-start justify-between gap-4">
          <div>
            <div class="flex flex-wrap items-center gap-2">
              <span class="text-sm font-semibold text-slate-700">
                REF-{{ referral.referralId }}
              </span>

              <CoordinatorStatusBadge :status="referral.status as any" />

              <CoordinatorUrgencyBadge :urgency="referral.urgency as any" />
            </div>

            <h2 class="mt-3 text-2xl font-bold text-slate-900">
              {{ referral.patientName }}
            </h2>

            <p class="mt-1 text-sm text-slate-500">
              {{ referral.specialty }}
            </p>
          </div>

          <button
            type="button"
            class="rounded-lg p-2 text-slate-400 transition-colors hover:bg-slate-100 hover:text-slate-600"
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

      <div class="mt-4 grid grid-cols-5 gap-4">
        <!-- Referral Details -->
        <div
          class="col-span-2 rounded-xl border border-slate-100 bg-white p-6 shadow-lg"
        >
          <h3 class="text-base font-bold text-slate-900">Referral Details</h3>

          <div class="mt-5 space-y-4">
            <div>
              <p
                class="text-xs font-semibold uppercase tracking-wide text-slate-500"
              >
                Patient Name
              </p>
              <p class="mt-1 text-sm text-slate-900">
                {{ referral.patientName }}
              </p>
            </div>

            <div>
              <p
                class="text-xs font-semibold uppercase tracking-wide text-slate-500"
              >
                Origin Facility
              </p>
              <p class="mt-1 text-sm text-slate-700">
                {{ referral.originFacility }}
              </p>
            </div>

            <div>
              <p
                class="text-xs font-semibold uppercase tracking-wide text-slate-500"
              >
                Destination Facility
              </p>
              <p class="mt-1 text-sm text-slate-700">
                {{ referral.destinationFacility }}
              </p>
            </div>

            <div>
              <p
                class="text-xs font-semibold uppercase tracking-wide text-slate-500"
              >
                Specialty
              </p>
              <p class="mt-1 text-sm text-slate-700">
                {{ referral.specialty }}
              </p>
            </div>

            <div>
              <p
                class="text-xs font-semibold uppercase tracking-wide text-slate-500"
              >
                Diagnosis Code
              </p>
              <p class="mt-1 text-sm text-slate-700">
                {{ referral.diagnosisCode || "N/A" }}
              </p>
            </div>

            <div>
              <p
                class="text-xs font-semibold uppercase tracking-wide text-slate-500"
              >
                Status
              </p>

              <div class="mt-1">
                <CoordinatorStatusBadge :status="referral.status as any" />
              </div>
            </div>

            <div>
              <p
                class="text-xs font-semibold uppercase tracking-wide text-slate-500"
              >
                Urgency
              </p>

              <div class="mt-1">
                <CoordinatorUrgencyBadge :urgency="referral.urgency as any" />
              </div>
            </div>

            <div>
              <p
                class="text-xs font-semibold uppercase tracking-wide text-slate-500"
              >
                Created At
              </p>

              <p class="mt-1 text-sm text-slate-700">
                {{ new Date(referral.createdAt).toLocaleString() }}
              </p>
            </div>
          </div>
        </div>

        <!-- Status Section -->
        <div
          class="col-span-3 rounded-xl border border-slate-100 bg-white p-6 shadow-lg"
        >
          <h3 class="text-base font-bold text-slate-900">Referral Status</h3>

          <p class="mt-1 text-sm text-slate-500">
            Current status and workflow information.
          </p>

          <!-- Status Definitions -->
          <div class="mt-5 rounded-xl bg-slate-50 p-4">
            <p
              class="text-xs font-semibold uppercase tracking-wide text-slate-500"
            >
              Status Definitions
            </p>

            <div class="mt-3 grid grid-cols-2 gap-x-4 gap-y-2">
              <div
                v-for="status in referralStatusOrder"
                :key="status"
                class="text-xs text-slate-600"
              >
                <span class="font-semibold text-slate-800">
                  {{ status }} —
                </span>

                {{ referralStatusDefinitions[status].description }}
              </div>
            </div>
          </div>

          <!-- Current State -->
          <div class="mt-6">
            <p
              class="mb-4 text-xs font-semibold uppercase tracking-wide text-slate-500"
            >
              Current State
            </p>

            <div class="rounded-xl border border-slate-200 bg-white p-5">
              <div class="flex items-center gap-3">
                <CoordinatorStatusBadge :status="referral.status as any" />

                <CoordinatorUrgencyBadge :urgency="referral.urgency as any" />
              </div>

              <p class="mt-4 text-sm text-slate-700">
                This referral is currently in
                <span class="font-semibold">
                  {{ referral.status }}
                </span>
                status.
              </p>

              <p class="mt-2 text-sm text-slate-600">
                Specialty Requested:
                <span class="font-medium">
                  {{ referral.specialty }}
                </span>
              </p>

              <p class="mt-2 text-xs text-slate-500">
                Created:
                {{ new Date(referral.createdAt).toLocaleString() }}
              </p>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
