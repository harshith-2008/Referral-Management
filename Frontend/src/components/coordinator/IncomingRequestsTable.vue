<script setup lang="ts">
import { computed, ref } from "vue";

const props = defineProps<{
  referrals: any[];
}>();

const emit = defineEmits<{
  view: [referral: any];
}>();

const searchQuery = ref("");
const urgencyFilter = ref("All");

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

    const matchesUrgency =
      urgencyFilter.value === "All" || referral.urgency === urgencyFilter.value;

    return matchesSearch && matchesUrgency;
  });
});

const summary = computed(() => ({
  total: props.referrals.length,
}));

const handleView = (referral: any) => {
  emit("view", referral);
};
</script>

<template>
  <div class="rounded-xl border border-slate-100 bg-white shadow-sm">
    <!-- Filters -->
    <div class="space-y-4 border-b border-slate-100 px-6 py-4">
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

      <div class="flex flex-wrap items-center gap-3">
        <span class="rounded-full bg-slate-100 px-3 py-1 text-xs font-semibold">
          Total: {{ summary.total }}
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
          <tr class="border-b border-slate-100 bg-slate-50/50">
            <th class="px-6 py-3 text-left text-xs font-semibold uppercase">
              Referral ID
            </th>

            <th class="px-6 py-3 text-left text-xs font-semibold uppercase">
              Patient
            </th>

            <th class="px-6 py-3 text-left text-xs font-semibold uppercase">
              Origin
            </th>

            <th class="px-6 py-3 text-left text-xs font-semibold uppercase">
              Destination
            </th>

            <th class="px-6 py-3 text-left text-xs font-semibold uppercase">
              Specialty
            </th>

            <th class="px-6 py-3 text-left text-xs font-semibold uppercase">
              Urgency
            </th>

            <th class="px-6 py-3 text-left text-xs font-semibold uppercase">
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
              {{ referral.originFacility }}
            </td>

            <td class="px-6 py-4 text-sm">
              {{ referral.destinationFacility }}
            </td>

            <td class="px-6 py-4 text-sm">
              {{ referral.specialty }}
            </td>

            <td class="px-6 py-4 text-sm">
              {{ referral.urgency }}
            </td>

            <td class="px-6 py-4">
              <button
                type="button"
                class="inline-flex items-center rounded-lg border border-blue-200 px-3 py-1.5 text-sm font-medium text-blue-600 hover:bg-blue-50"
                @click="handleView(referral)"
              >
                View
              </button>
            </td>
          </tr>

          <tr v-if="filteredReferrals.length === 0">
            <td colspan="7" class="px-6 py-10 text-center text-slate-500">
              No incoming referrals found.
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>
