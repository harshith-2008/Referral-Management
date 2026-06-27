<script setup lang="ts">
import { formatDate } from "../../utils/date";
import UrgencyBadge from "./UrgencyBadge.vue";

interface ReferralReviewModalReferral {
  referralId: number;
  patientName: string;
  age: number;
  gender: string;
  mrn: string;
  specialty: string;
  urgency: string;
  diagnosisCode?: string;
  appointmentDate?: string;
}

defineProps<{
  referral: ReferralReviewModalReferral;
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
    <div class="w-full max-w-2xl space-y-4 pb-10">
      <!-- Header -->
      <div
        class="bg-white rounded-2xl shadow-2xl px-6 py-5 flex items-start justify-between gap-4"
      >
        <div>
          <div class="flex flex-wrap items-center gap-2 mb-3">
            <span
              class="text-xs font-medium tracking-widest text-slate-400 uppercase"
              >Referral #{{ referral.referralId }}</span
            >
            <UrgencyBadge :urgency="referral.urgency" />
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

      <!-- Body -->
      <div class="bg-white rounded-2xl shadow-xl px-6 py-5">
        <p
          class="text-xs font-medium tracking-widest text-slate-400 uppercase mb-4"
        >
          Patient Summary
        </p>

        <!-- Patient identity strip -->
        <div
          class="flex items-center gap-4 rounded-xl bg-slate-50 border border-slate-100 px-5 py-4 mb-5"
        >
          <div
            class="w-10 h-10 rounded-full bg-blue-100 flex items-center justify-center text-blue-600 font-bold text-sm shrink-0"
          >
            {{ referral.patientName?.charAt(0) }}
          </div>
          <div>
            <p class="font-semibold text-slate-900 text-sm leading-tight">
              {{ referral.patientName }}
            </p>
            <p class="text-xs text-slate-400 mt-0.5">MRN {{ referral.mrn }}</p>
          </div>
          <div class="ml-auto flex gap-6 text-right">
            <div>
              <p
                class="text-[10px] font-medium tracking-wider text-slate-400 uppercase mb-0.5"
              >
                Age
              </p>
              <p class="text-sm font-medium text-slate-800">
                {{ referral.age }}
              </p>
            </div>
            <div>
              <p
                class="text-[10px] font-medium tracking-wider text-slate-400 uppercase mb-0.5"
              >
                Gender
              </p>
              <p class="text-sm font-medium text-slate-800">
                {{ referral.gender }}
              </p>
            </div>
          </div>
        </div>

        <!-- Detail fields -->
        <div class="grid grid-cols-2 gap-x-8 gap-y-5">
          <div>
            <p
              class="text-[10px] font-medium tracking-wider text-slate-400 uppercase mb-1"
            >
              Specialty
            </p>
            <p class="text-sm font-medium text-slate-800">
              {{ referral.specialty }}
            </p>
          </div>

          <div>
            <p
              class="text-[10px] font-medium tracking-wider text-slate-400 uppercase mb-1.5"
            >
              Urgency
            </p>
            <UrgencyBadge :urgency="referral.urgency" />
          </div>

          <div v-if="referral.diagnosisCode">
            <p
              class="text-[10px] font-medium tracking-wider text-slate-400 uppercase mb-1"
            >
              Diagnosis Code
            </p>
            <p class="text-sm font-medium text-slate-800">
              {{ referral.diagnosisCode }}
            </p>
          </div>

          <div v-if="referral.appointmentDate">
            <p
              class="text-[10px] font-medium tracking-wider text-slate-400 uppercase mb-1"
            >
              Appointment Date
            </p>
            <p class="text-sm font-medium text-slate-800">
              {{ formatDate(referral.appointmentDate) }}
            </p>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
