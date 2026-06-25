<script setup lang="ts">
import { computed, ref } from "vue";
import type { ReferralDTO } from "../../types/referral";

const props = defineProps<{
  referrals: ReferralDTO[];
  showActions?: boolean;
}>();

const emit = defineEmits<{
  view: [referral: ReferralDTO];
}>();

const searchQuery = ref("");

const filteredReferrals = computed(() => {
  const query = searchQuery.value.trim().toLowerCase();

  return props.referrals.filter((referral) => {
    return (
      !query ||
      referral.referralId.toString().includes(query) ||
      referral.patientName.toLowerCase().includes(query) ||
      referral.specialty.toLowerCase().includes(query) ||
      referral.originFacility.toLowerCase().includes(query) ||
      referral.destinationFacility.toLowerCase().includes(query)
    );
  });
});
</script>

<template>
  <div class="rounded-xl border border-slate-100 bg-white shadow-sm">
    <!-- Search -->
    <div class="border-b border-slate-100 px-6 py-4">
      <input
        v-model="searchQuery"
        type="text"
        placeholder="Search referrals..."
        class="w-full rounded-xl border border-slate-200 px-4 py-2.5 text-sm"
      />
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
            </td>

            <td class="px-6 py-4">
              {{ referral.patientName }}
            </td>

            <td class="px-6 py-4">
              {{ referral.specialty }}
            </td>

            <td class="px-6 py-4">
              {{ referral.originFacility }}
            </td>

            <td class="px-6 py-4">
              {{ referral.destinationFacility }}
            </td>

            <td class="px-6 py-4">
              <span
                class="rounded-full bg-slate-100 px-3 py-1 text-xs font-medium"
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
            <td colspan="8" class="px-6 py-8 text-center text-slate-500">
              No referrals found.
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>
