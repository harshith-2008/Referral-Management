<script setup lang="ts">
import { computed, ref } from "vue";
import type { SpecialistPatientDTO } from "../../types/referral";
import UrgencyBadge from "./UrgencyBadge.vue";

const props = defineProps<{
  referrals: SpecialistPatientDTO[];
  showActions?: boolean;
}>();

const emit = defineEmits<{
  review: [referral: SpecialistPatientDTO];
}>();

const searchQuery = ref("");

const filteredReferrals = computed(() => {
  const query = searchQuery.value.trim().toLowerCase();

  return props.referrals.filter((referral) => {
    return (
      !query ||
      referral.referralId.toString().includes(query) ||
      referral.patientName.toLowerCase().includes(query) ||
      referral.mrn.toLowerCase().includes(query) ||
      referral.specialty.toLowerCase().includes(query)
    );
  });
});
</script>

<template>
  <div class="rounded-xl border border-slate-100 bg-white shadow-sm">
    <div
      v-if="showActions"
      class="flex items-center gap-4 border-b border-slate-100 px-6 py-4"
    >
      <div class="relative flex-1">
        <svg
          viewBox="0 0 24 24"
          fill="none"
          class="absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-slate-400"
        >
          <circle
            cx="11"
            cy="11"
            r="7"
            stroke="currentColor"
            stroke-width="1.75"
          />
          <path
            d="M20 20l-3-3"
            stroke="currentColor"
            stroke-width="1.75"
            stroke-linecap="round"
          />
        </svg>

        <input
          v-model="searchQuery"
          type="text"
          placeholder="Search referrals..."
          class="w-full rounded-xl border border-slate-200 bg-white py-2.5 pl-10 pr-4 text-sm text-slate-900 outline-none transition-colors focus:border-blue-400"
        />
      </div>
    </div>

    <div class="overflow-hidden">
      <table class="w-full">
        <thead>
          <tr class="border-b border-slate-100 bg-slate-50/50">
            <th
              class="px-6 py-3 text-left text-xs font-semibold uppercase tracking-wide text-slate-500"
            >
              Referral ID
            </th>

            <th
              class="px-6 py-3 text-left text-xs font-semibold uppercase tracking-wide text-slate-500"
            >
              Patient
            </th>

            <th
              class="px-6 py-3 text-left text-xs font-semibold uppercase tracking-wide text-slate-500"
            >
              MRN
            </th>

            <th
              class="px-6 py-3 text-left text-xs font-semibold uppercase tracking-wide text-slate-500"
            >
              Specialty
            </th>

            <th
              class="px-6 py-3 text-left text-xs font-semibold uppercase tracking-wide text-slate-500"
            >
              Urgency
            </th>

            <th
              v-if="showActions"
              class="px-6 py-3 text-left text-xs font-semibold uppercase tracking-wide text-slate-500"
            >
              Actions
            </th>
          </tr>
        </thead>

        <tbody>
          <tr
            v-for="referral in filteredReferrals"
            :key="referral.referralId"
            class="border-b border-slate-100 last:border-b-0 transition-colors hover:bg-slate-50"
          >
            <td class="px-6 py-4">
              <span class="text-sm font-medium text-blue-600">
                #{{ referral.referralId }}
              </span>
            </td>

            <td class="px-6 py-4 text-sm font-semibold text-slate-900">
              {{ referral.patientName }}
            </td>

            <td class="px-6 py-4 text-sm text-slate-600">
              {{ referral.mrn }}
            </td>

            <td class="px-6 py-4 text-sm text-slate-600">
              {{ referral.specialty }}
            </td>

            <td class="px-6 py-4">
              <UrgencyBadge :urgency="referral.urgency" />
            </td>

            <td v-if="showActions" class="px-6 py-4">
              <button
                type="button"
                class="inline-flex items-center gap-2 rounded-lg border border-blue-200 px-3 py-1.5 text-sm font-medium text-blue-600 transition-colors hover:bg-blue-50"
                @click="emit('review', referral)"
              >
                Review
              </button>
            </td>
          </tr>

          <tr v-if="filteredReferrals.length === 0">
            <td
              :colspan="showActions ? 6 : 5"
              class="px-6 py-8 text-center text-sm text-slate-500"
            >
              No referrals found.
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>
