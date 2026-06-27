<script setup lang="ts">
import { computed, ref } from "vue";
import type { RoutingPendingReferral } from "../../types/routingPendingReferral";
import CoordinatorStatusBadge from "./CoordinatorStatusBadge.vue";
import CoordinatorUrgencyBadge from "./CoordinatorUrgencyBadge.vue";

const props = defineProps<{
  referrals: RoutingPendingReferral[];
  showFilters?: boolean;
  showSummary?: boolean;
  showActions?: boolean;
}>();

const emit = defineEmits<{
  route: [referral: RoutingPendingReferral];
}>();

const searchQuery = ref("");
const statusFilter = ref("All");
const urgencyFilter = ref("All");

const statusOptions = [
  "All",
  "Submitted",
  "Requested",
  "Accepted",
  "Rejected",
  "Completed",
  "Closed",
];

const urgencyOptions = ["All", "Urgent", "Emergency", "Routine"];

const filteredReferrals = computed(() => {
  const query = searchQuery.value.trim().toLowerCase();

  return props.referrals.filter((referral) => {
    const matchesSearch =
      !query ||
      referral.referralId.toString().includes(query) ||
      referral.patientName.toLowerCase().includes(query) ||
      referral.originFacility.toLowerCase().includes(query) ||
      referral.destinationFacility.toLowerCase().includes(query);

    const matchesStatus =
      statusFilter.value === "All" || referral.status === statusFilter.value;

    const matchesUrgency =
      urgencyFilter.value === "All" || referral.urgency === urgencyFilter.value;

    return matchesSearch && matchesStatus && matchesUrgency;
  });
});

const statusCounts = computed(() => ({
  total: props.referrals.length,
  submitted: props.referrals.filter((r) => r.status === "Submitted").length,
  rejected: props.referrals.filter((r) => r.status === "Rejected").length,
}));

const handleRoute = (referral: RoutingPendingReferral) => {
  emit("route", referral);
};
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
            placeholder="Search by patient, referral ID, or facility..."
            class="w-full rounded-xl border border-slate-200 bg-white py-2.5 pl-10 pr-4 text-sm text-slate-900 outline-none focus:border-blue-400"
          />
        </div>

        <select
          v-model="statusFilter"
          class="rounded-xl border border-slate-200 bg-white px-4 py-2.5 text-sm"
        >
          <option v-for="option in statusOptions" :key="option" :value="option">
            Status: {{ option }}
          </option>
        </select>

        <select
          v-model="urgencyFilter"
          class="rounded-xl border border-slate-200 bg-white px-4 py-2.5 text-sm"
        >
          <option
            v-for="option in urgencyOptions"
            :key="option"
            :value="option"
          >
            Urgency: {{ option }}
          </option>
        </select>
      </div>

      <div v-if="showSummary" class="flex flex-wrap items-center gap-3">
        <span class="rounded-full bg-slate-100 px-3 py-1 text-xs font-semibold">
          Total: {{ statusCounts.total }}
        </span>

        <span class="rounded-full bg-slate-100 px-3 py-1 text-xs font-semibold">
          Submitted: {{ statusCounts.submitted }}
        </span>

        <span
          class="rounded-full bg-red-50 px-3 py-1 text-xs font-semibold text-red-700"
        >
          Rejected: {{ statusCounts.rejected }}
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
            <th class="px-6 py-3 text-left text-xs font-semibold uppercase">
              Referral ID
            </th>

            <th class="px-6 py-3 text-left text-xs font-semibold uppercase">
              Patient
            </th>

            <th class="px-6 py-3 text-left text-xs font-semibold uppercase">
              Specialty
            </th>

            <th class="px-6 py-3 text-left text-xs font-semibold uppercase">
              Diagnosis
            </th>

            <th class="px-6 py-3 text-left text-xs font-semibold uppercase">
              Urgency
            </th>

            <th class="px-6 py-3 text-left text-xs font-semibold uppercase">
              Status
            </th>

            <th
              v-if="showActions"
              class="px-6 py-3 text-left text-xs font-semibold uppercase"
            >
              Actions
            </th>
          </tr>
        </thead>

        <tbody>
          <tr
            v-for="referral in filteredReferrals"
            :key="referral.referralId"
            class="border-b border-slate-100 hover:bg-slate-50"
          >
            <td class="px-6 py-4">
              <span class="font-medium text-blue-600">
                #{{ referral.referralId }}
              </span>
            </td>

            <td class="px-6 py-4 text-sm font-medium">
              {{ referral.patientName }}
            </td>

            <td class="px-6 py-4 text-sm">
              {{ referral.specialty }}
            </td>

            <td class="px-6 py-4 text-sm">
              {{ referral.diagnosisCode }}
            </td>

            <td class="px-6 py-4">
              <CoordinatorUrgencyBadge :urgency="referral.urgency" />
            </td>

            <td class="px-6 py-4">
              <CoordinatorStatusBadge :status="referral.status" />
            </td>

            <td v-if="showActions" class="px-6 py-4">
              <button
                type="button"
                class="inline-flex items-center rounded-lg border border-blue-200 px-3 py-1.5 text-sm font-medium text-blue-600 hover:bg-blue-50"
                @click="handleRoute(referral)"
              >
                {{ referral.status === "Rejected" ? "Route Again" : "Route" }}
              </button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>
