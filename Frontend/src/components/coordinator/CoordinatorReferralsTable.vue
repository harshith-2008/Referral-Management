<script setup lang="ts">
import { computed, ref } from "vue";

import type { ReferralDTO } from "../../types/referral";

import CoordinatorStatusBadge from "./CoordinatorStatusBadge.vue";
import CoordinatorUrgencyBadge from "./CoordinatorUrgencyBadge.vue";
import type { ReferralUrgency } from "../../types/coordinatorReferral.ts";

const props = defineProps<{
  referrals: ReferralDTO[];
  showFilters?: boolean;
  showSummary?: boolean;
  showActions?: boolean;
  actionLabel?: string;
}>();

const emit = defineEmits<{
  view: [referral: ReferralDTO];
  route: [referral: ReferralDTO];
}>();

const searchQuery = ref("");

const statusFilter = ref<string>("All");
const urgencyFilter = ref<string>("All");

const statusOptions = [
  "All",
  "Submitted",
  "Requested",
  "Accepted",
  "Rejected",
  "Scheduled",
  "Completed",
  "Cancelled",
];

const urgencyOptions = ["All", "Routine", "Urgent", "Emergency"];

const filteredReferrals = computed(() => {
  const query = searchQuery.value.trim().toLowerCase();

  return props.referrals.filter((referral) => {
    const matchesSearch =
      !query ||
      referral.referralId.toString().includes(query) ||
      referral.patientName.toLowerCase().includes(query) ||
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
  requested: props.referrals.filter((r) => r.status === "Requested").length,
  accepted: props.referrals.filter((r) => r.status === "Accepted").length,
  completed: props.referrals.filter((r) => r.status === "Completed").length,
}));

const handleAction = (referral: ReferralDTO) => {
  if (props.actionLabel === "Route") {
    emit("route", referral);
  } else {
    emit("view", referral);
  }
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
          <input
            v-model="searchQuery"
            type="text"
            placeholder="Search by patient, referral ID, or facility..."
            class="w-full rounded-xl border border-slate-200 bg-white py-2.5 px-4 text-sm"
          />
        </div>

        <select
          v-model="statusFilter"
          class="rounded-xl border border-slate-200 px-4 py-2.5 text-sm"
        >
          <option v-for="option in statusOptions" :key="option" :value="option">
            Status: {{ option }}
          </option>
        </select>

        <select
          v-model="urgencyFilter"
          class="rounded-xl border border-slate-200 px-4 py-2.5 text-sm"
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
          class="rounded-full bg-purple-50 px-3 py-1 text-xs font-semibold text-purple-700"
        >
          Requested: {{ statusCounts.requested }}
        </span>

        <span
          class="rounded-full bg-green-50 px-3 py-1 text-xs font-semibold text-green-700"
        >
          Accepted: {{ statusCounts.accepted }}
        </span>

        <span
          class="rounded-full bg-emerald-50 px-3 py-1 text-xs font-semibold text-emerald-700"
        >
          Completed: {{ statusCounts.completed }}
        </span>

        <span class="ml-auto text-sm text-slate-500">
          Showing {{ filteredReferrals.length }} referrals
        </span>
      </div>
    </div>

    <div class="overflow-hidden">
      <table class="w-full">
        <thead>
          <tr class="border-b border-slate-100 bg-slate-50">
            <th class="px-6 py-3 text-left text-xs font-semibold">
              Referral ID
            </th>

            <th class="px-6 py-3 text-left text-xs font-semibold">Patient</th>

            <th class="px-6 py-3 text-left text-xs font-semibold">
              Origin Facility
            </th>

            <th class="px-6 py-3 text-left text-xs font-semibold">
              Destination Facility
            </th>

            <th class="px-6 py-3 text-left text-xs font-semibold">Specialty</th>

            <th class="px-6 py-3 text-left text-xs font-semibold">Urgency</th>

            <th class="px-6 py-3 text-left text-xs font-semibold">Status</th>

            <th class="px-6 py-3 text-left text-xs font-semibold">Created</th>

            <th
              v-if="showActions"
              class="px-6 py-3 text-left text-xs font-semibold"
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
            <td class="px-6 py-4 text-blue-600 font-medium">
              #{{ referral.referralId }}
            </td>

            <td class="px-6 py-4">
              {{ referral.patientName }}
            </td>

            <td class="px-6 py-4">
              {{ referral.originFacility }}
            </td>

            <td class="px-6 py-4">
              {{ referral.destinationFacility }}
            </td>

            <td class="px-6 py-4">
              {{ referral.specialty }}
            </td>

            <td class="px-6 py-4">
              <CoordinatorUrgencyBadge
                :urgency="referral.urgency as ReferralUrgency"
              />
            </td>

            <td class="px-6 py-4">
              <CoordinatorStatusBadge :status="referral.status as any" />
            </td>

            <td class="px-6 py-4 text-sm text-slate-500">
              {{ new Date(referral.createdAt).toLocaleDateString() }}
            </td>

            <td v-if="showActions" class="px-6 py-4">
              <button
                class="rounded-lg border border-blue-200 px-3 py-1.5 text-sm font-medium text-blue-600 hover:bg-blue-50"
                @click="handleAction(referral)"
              >
                {{ actionLabel || "View" }}
              </button>
            </td>
          </tr>

          <tr v-if="filteredReferrals.length === 0">
            <td colspan="9" class="px-6 py-8 text-center text-slate-500">
              No referrals found.
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>
