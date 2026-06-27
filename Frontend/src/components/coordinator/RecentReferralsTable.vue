<script setup lang="ts">
import type { ReferralDTO } from "../../types/referral.ts";
import CoordinatorStatusBadge from "./CoordinatorStatusBadge.vue";
import CoordinatorUrgencyBadge from "./CoordinatorUrgencyBadge.vue";

defineProps<{
  referrals: ReferralDTO[];
  viewAllLink?: string;
}>();
</script>

<template>
  <div class="rounded-xl border border-slate-100 bg-white shadow-sm">
    <div
      class="flex items-center justify-between border-b border-slate-100 px-6 py-5"
    >
      <h2 class="text-lg font-bold text-slate-900">Recent Referrals</h2>
      <RouterLink
        v-if="viewAllLink"
        :to="viewAllLink"
        class="text-sm font-medium text-blue-600 transition-colors hover:text-blue-700"
      >
        View all &gt;
      </RouterLink>
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
              Status
            </th>
          </tr>
        </thead>
        <tbody>
          <tr
            v-for="referral in referrals"
            :key="referral.referralId"
            class="border-b border-slate-100 last:border-b-0 transition-colors hover:bg-slate-50"
          >
            <td class="px-6 py-4">
              <span class="text-sm font-medium text-blue-600">{{
                referral.referralId
              }}</span>
            </td>
            <td class="px-6 py-4 text-sm font-semibold text-slate-900">
              {{ referral.patientName }}
            </td>
            <!-- <td class="px-6 py-4 text-sm text-slate-600">
              {{ referral.destinationFacility }}
            </td>
            <td class="px-6 py-4">
              <CoordinatorUrgencyBadge :urgency="referral.urgency as any" />
            </td> -->
            <td class="px-6 py-4">
              <CoordinatorStatusBadge :status="referral.status as any" />
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>
