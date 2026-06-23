<script setup lang="ts">
import { computed, ref } from "vue";
import type { CoordinatorReferral, CoordinatorReferralStatus, ReferralUrgency } from "../../types/coordinatorReferral";
import CoordinatorStatusBadge from "./CoordinatorStatusBadge.vue";
import CoordinatorUrgencyBadge from "./CoordinatorUrgencyBadge.vue";

const props = defineProps<{
  referrals: CoordinatorReferral[];
  showFilters?: boolean;
  showSummary?: boolean;
  showActions?: boolean;
}>();

const emit = defineEmits<{
  view: [referral: CoordinatorReferral];
}>();

const searchQuery = ref("");
const statusFilter = ref<"All" | CoordinatorReferralStatus>("All");
const urgencyFilter = ref<"All" | ReferralUrgency>("All");

const statusOptions: Array<"All" | CoordinatorReferralStatus> = [
  "All",
  "Submitted",
  "Requested",
  "Accepted",
  "Rejected",
  "Scheduled",
  "Closed",
  "Cancelled",
];

const urgencyOptions: Array<"All" | ReferralUrgency> = ["All", "Urgent", "Emergency", "Routine"];

const filteredReferrals = computed(() => {
  const query = searchQuery.value.trim().toLowerCase();

  return props.referrals.filter((referral) => {
    const matchesSearch =
      !query ||
      referral.id.toLowerCase().includes(query) ||
      referral.patientName.toLowerCase().includes(query) ||
      referral.hospitalBranch.toLowerCase().includes(query);

    const matchesStatus = statusFilter.value === "All" || referral.status === statusFilter.value;
    const matchesUrgency = urgencyFilter.value === "All" || referral.urgency === urgencyFilter.value;

    return matchesSearch && matchesStatus && matchesUrgency;
  });
});

const statusCounts = computed(() => ({
  total: props.referrals.length,
  submitted: props.referrals.filter((r) => r.status === "Submitted").length,
  requested: props.referrals.filter((r) => r.status === "Requested").length,
  accepted: props.referrals.filter((r) => r.status === "Accepted").length,
  closed: props.referrals.filter((r) => r.status === "Closed").length,
}));
</script>

<template>
  <div class="rounded-xl border border-slate-100 bg-white shadow-sm">
    <div
      v-if="showFilters"
      class="space-y-4 border-b border-slate-100 px-6 py-4"
    >
      <div class="flex items-center gap-4">
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
            placeholder="Search by patient, ID, or hospital branch..."
            class="w-full rounded-xl border border-slate-200 bg-white py-2.5 pl-10 pr-4 text-sm text-slate-900 outline-none transition-colors focus:border-blue-400"
          />
        </div>

        <select
          v-model="statusFilter"
          class="rounded-xl border border-slate-200 bg-white px-4 py-2.5 text-sm text-slate-700 outline-none transition-colors focus:border-blue-400"
        >
          <option v-for="option in statusOptions" :key="option" :value="option">
            Status: {{ option }}
          </option>
        </select>

        <select
          v-model="urgencyFilter"
          class="rounded-xl border border-slate-200 bg-white px-4 py-2.5 text-sm text-slate-700 outline-none transition-colors focus:border-blue-400"
        >
          <option v-for="option in urgencyOptions" :key="option" :value="option">
            Urgency: {{ option }}
          </option>
        </select>
      </div>

      <div v-if="showSummary" class="flex flex-wrap items-center gap-3">
        <span class="rounded-full bg-slate-100 px-3 py-1 text-xs font-semibold text-slate-700">
          Total: {{ statusCounts.total }}
        </span>
        <span class="rounded-full bg-slate-100 px-3 py-1 text-xs font-semibold text-slate-600">
          Submitted: {{ statusCounts.submitted }}
        </span>
        <span class="rounded-full bg-purple-50 px-3 py-1 text-xs font-semibold text-purple-700">
          Requested: {{ statusCounts.requested }}
        </span>
        <span class="rounded-full bg-green-50 px-3 py-1 text-xs font-semibold text-green-700">
          Accepted: {{ statusCounts.accepted }}
        </span>
        <span class="rounded-full bg-emerald-50 px-3 py-1 text-xs font-semibold text-emerald-700">
          Closed: {{ statusCounts.closed }}
        </span>
        <span class="ml-auto text-sm text-slate-500">
          Showing {{ filteredReferrals.length }} referrals
        </span>
      </div>
    </div>

    <div class="overflow-hidden">
      <table class="w-full">
        <thead>
          <tr class="border-b border-slate-100 bg-slate-50/50">
            <th class="px-6 py-3 text-left text-xs font-semibold uppercase tracking-wide text-slate-500">
              Referral ID
            </th>
            <th class="px-6 py-3 text-left text-xs font-semibold uppercase tracking-wide text-slate-500">
              Patient Name
            </th>
            <th class="px-6 py-3 text-left text-xs font-semibold uppercase tracking-wide text-slate-500">
              Hospital Branch
            </th>
            <th class="px-6 py-3 text-left text-xs font-semibold uppercase tracking-wide text-slate-500">
              Urgency
            </th>
            <th class="px-6 py-3 text-left text-xs font-semibold uppercase tracking-wide text-slate-500">
              Assigned Specialist
            </th>
            <th class="px-6 py-3 text-left text-xs font-semibold uppercase tracking-wide text-slate-500">
              Status
            </th>
            <th class="px-6 py-3 text-left text-xs font-semibold uppercase tracking-wide text-slate-500">
              Date
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
            <td class="px-6 py-4 text-sm text-slate-600">
              {{ referral.hospitalBranch }}
            </td>
            <td class="px-6 py-4">
              <CoordinatorUrgencyBadge :urgency="referral.urgency" />
            </td>
            <td class="px-6 py-4 text-sm text-slate-600">
              {{ referral.assignedSpecialist }}
            </td>
            <td class="px-6 py-4">
              <CoordinatorStatusBadge :status="referral.status" />
            </td>
            <td class="px-6 py-4 text-sm text-slate-500">
              {{ referral.date }}
            </td>
            <td v-if="showActions" class="px-6 py-4">
              <button
                type="button"
                class="inline-flex items-center gap-2 rounded-lg border border-blue-200 px-3 py-1.5 text-sm font-medium text-blue-600 transition-colors hover:bg-blue-50"
                @click="emit('view', referral)"
              >
                <svg viewBox="0 0 24 24" fill="none" class="h-4 w-4" aria-hidden="true">
                  <path
                    d="M2 12s3.5-7 10-7 10 7 10 7-3.5 7-10 7-10-7-10-7Z"
                    stroke="currentColor"
                    stroke-width="1.75"
                  />
                  <circle cx="12" cy="12" r="2.5" stroke="currentColor" stroke-width="1.75" />
                </svg>
                View
              </button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>
