<script setup lang="ts">
import { computed, ref } from "vue";
import type { ReferralDTO } from "../../types/referral";
import CoordinatorUrgencyBadge from "../coordinator/CoordinatorUrgencyBadge.vue";
import type { ReferralUrgency } from "../../types/coordinatorReferral";

const props = defineProps<{
  referrals: ReferralDTO[];
  showActions?: boolean;
}>();

const emit = defineEmits<{
  view: [referral: ReferralDTO];
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

const urgencyOptions = ["All", "Routine", "Urgent", "Emergency"];

const statusClass = (status: string) => {
  const classes: Record<string, string> = {
    Submitted: "bg-slate-100 text-slate-700",
    Requested: "bg-purple-50 text-purple-700",
    Accepted: "bg-green-50 text-green-700",
    Rejected: "bg-red-50 text-red-700",
    Completed: "bg-emerald-50 text-emerald-700",
    Closed: "bg-slate-100 text-slate-600",
  };

  return classes[status] ?? "bg-slate-100 text-slate-700";
};

const filteredReferrals = computed(() => {
  const query = searchQuery.value.trim().toLowerCase();

  return props.referrals.filter((referral) => {
    const matchesSearch =
      !query ||
      referral.referralId.toString().includes(query) ||
      referral.patientName.toLowerCase().includes(query) ||
      referral.specialty.toLowerCase().includes(query) ||
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
  requested: props.referrals.filter((r) => r.status === "Requested").length,
  accepted: props.referrals.filter((r) => r.status === "Accepted").length,
  rejected: props.referrals.filter((r) => r.status === "Rejected").length,
  completed: props.referrals.filter((r) => r.status === "Completed").length,
}));
</script>

<template>
  <div class="rounded-xl border border-slate-100 bg-white shadow-sm">
    <!-- Filters -->
    <div class="space-y-4 border-b border-slate-100 px-6 py-4">
      <div class="flex flex-col gap-3 lg:flex-row lg:items-center">
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
            placeholder="Search by patient, referral ID, specialty, or facility..."
            class="w-full rounded-xl border border-slate-200 bg-white py-2.5 pl-10 pr-4 text-sm text-slate-900 outline-none focus:border-blue-400"
          />
        </div>

        <select
          v-model="statusFilter"
          class="rounded-xl border border-slate-200 bg-white px-4 py-2.5 text-sm text-slate-700"
        >
          <option v-for="option in statusOptions" :key="option" :value="option">
            Status: {{ option }}
          </option>
        </select>

        <select
          v-model="urgencyFilter"
          class="rounded-xl border border-slate-200 bg-white px-4 py-2.5 text-sm text-slate-700"
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

      <div class="flex flex-wrap items-center gap-3">
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
          class="rounded-full bg-red-50 px-3 py-1 text-xs font-semibold text-red-700"
        >
          Rejected: {{ statusCounts.rejected }}
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

    <!-- Table -->
    <div class="overflow-hidden">
      <table class="w-full">
        <thead>
          <tr class="border-b border-slate-100 bg-slate-50">
            <th class="px-6 py-3 text-left text-xs font-semibold">
              Referral ID
            </th>

            <th class="px-6 py-3 text-left text-xs font-semibold">Patient</th>

            <th class="px-6 py-3 text-left text-xs font-semibold">Specialty</th>

            <th class="px-6 py-3 text-left text-xs font-semibold">Urgency</th>

            <th class="px-6 py-3 text-left text-xs font-semibold">
              Origin Facility
            </th>

            <th class="px-6 py-3 text-left text-xs font-semibold">
              Destination Facility
            </th>

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
            <td class="px-6 py-4 font-medium text-blue-600">
              #{{ referral.referralId }}
              <span
                v-if="(referral.destinationCount ?? 1) > 1"
                class="mt-1 inline-flex rounded-full bg-blue-50 px-2 py-1 text-xs font-medium text-blue-700"
              >
                {{ referral.destinationCount }} Facilities
              </span>
            </td>

            <td class="px-6 py-4">
              {{ referral.patientName }}
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
              {{ referral.originFacility }}
            </td>

            <td class="px-6 py-4">
              {{ referral.destinationFacility }}
            </td>

            <td class="px-6 py-4">
              <span
                class="inline-flex rounded-full px-3 py-1 text-xs font-semibold"
                :class="statusClass(referral.status)"
              >
                {{ referral.status }}
              </span>
            </td>

            <td class="px-6 py-4 text-sm text-slate-500">
              {{ new Date(referral.createdAt).toLocaleDateString() }}
            </td>

            <td v-if="showActions" class="px-6 py-4">
              <button
                class="rounded-lg border border-blue-200 px-3 py-1.5 text-sm font-medium text-blue-600 hover:bg-blue-50"
                @click="emit('view', referral)"
              >
                View
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
