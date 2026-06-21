<script setup lang="ts">
import { computed, ref } from "vue";
import type { Referral, ReferralStatus } from "../../types/referral";
import UrgencyBadge from "./UrgencyBadge.vue";
import StatusBadge from "./StatusBadge.vue";

const props = defineProps<{
  referrals: Referral[];
  showActions?: boolean;
}>();

const emit = defineEmits<{
  review: [referral: Referral];
}>();

const searchQuery = ref("");
const statusFilter = ref<"All" | ReferralStatus>("All");

const statusOptions: Array<"All" | ReferralStatus> = [
  "All",
  "Under Review",
  "Scheduled",
  "Pending",
  "Accepted",
  "Completed",
  "Treatment Ongoing",
  "Rejected",
  "Assigned",
];

const filteredReferrals = computed(() => {
  const query = searchQuery.value.trim().toLowerCase();

  return props.referrals.filter((referral) => {
    const matchesSearch =
      !query ||
      referral.id.toLowerCase().includes(query) ||
      referral.patientName.toLowerCase().includes(query);

    const matchesStatus =
      statusFilter.value === "All" || referral.status === statusFilter.value;

    return matchesSearch && matchesStatus;
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
          aria-hidden="true"
        >
          <circle cx="11" cy="11" r="7" stroke="currentColor" stroke-width="1.75" />
          <path d="M20 20l-3-3" stroke="currentColor" stroke-width="1.75" stroke-linecap="round" />
        </svg>
        <input
          v-model="searchQuery"
          type="text"
          placeholder="Search referrals..."
          class="w-full rounded-xl border border-slate-200 bg-white py-2.5 pl-10 pr-4 text-sm text-slate-900 outline-none transition-colors focus:border-blue-400"
        />
      </div>

      <select
        v-model="statusFilter"
        class="rounded-xl border border-slate-200 bg-white px-4 py-2.5 text-sm text-slate-700 outline-none transition-colors focus:border-blue-400"
      >
        <option v-for="option in statusOptions" :key="option" :value="option">
          {{ option }}
        </option>
      </select>
    </div>

    <div class="overflow-hidden">
      <table class="w-full">
        <thead>
          <tr class="border-b border-slate-100 bg-slate-50/50">
            <th class="px-6 py-3 text-left text-xs font-semibold uppercase tracking-wide text-slate-500">
              Referral ID
            </th>
            <th class="px-6 py-3 text-left text-xs font-semibold uppercase tracking-wide text-slate-500">
              Patient
            </th>
            <th class="px-6 py-3 text-left text-xs font-semibold uppercase tracking-wide text-slate-500">
              Urgency
            </th>
            <th class="px-6 py-3 text-left text-xs font-semibold uppercase tracking-wide text-slate-500">
              Status
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
            :key="referral.id"
            class="border-b border-slate-100 last:border-b-0 transition-colors hover:bg-slate-50"
          >
            <td class="px-6 py-4">
              <span class="text-sm font-medium text-blue-600">{{ referral.id }}</span>
            </td>
            <td class="px-6 py-4 text-sm font-semibold text-slate-900">
              {{ referral.patientName }}
            </td>
            <td class="px-6 py-4">
              <UrgencyBadge :urgency="referral.urgency" />
            </td>
            <td class="px-6 py-4">
              <StatusBadge :status="referral.status" />
            </td>
            <td v-if="showActions" class="px-6 py-4">
              <button
                type="button"
                class="inline-flex items-center gap-2 rounded-lg border border-blue-200 px-3 py-1.5 text-sm font-medium text-blue-600 transition-colors hover:bg-blue-50"
                @click="emit('review', referral)"
              >
                <svg viewBox="0 0 24 24" fill="none" class="h-4 w-4" aria-hidden="true">
                  <path
                    d="M2 12s3.5-7 10-7 10 7 10 7-3.5 7-10 7-10-7-10-7Z"
                    stroke="currentColor"
                    stroke-width="1.75"
                  />
                  <circle cx="12" cy="12" r="2.5" stroke="currentColor" stroke-width="1.75" />
                </svg>
                Review
              </button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>
