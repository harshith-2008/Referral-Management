<script setup lang="ts">
import type { ReferralDTO } from "../../types/patient";

defineProps<{
  referrals: ReferralDTO[];
}>();
</script>

<template>
  <div class="rounded-xl border border-slate-100 bg-white shadow-sm">
    <div
      class="flex items-center justify-between border-b border-slate-100 px-6 py-5"
    >
      <h2 class="text-lg font-bold text-slate-900">
        Recent Referrals
      </h2>
    </div>

    <table class="w-full">
      <thead>
        <tr class="bg-slate-50">
          <th class="px-6 py-3 text-left text-xs font-semibold uppercase">
            Referral ID
          </th>

          <th class="px-6 py-3 text-left text-xs font-semibold uppercase">
            Specialty
          </th>

          <th class="px-6 py-3 text-left text-xs font-semibold uppercase">
            Status
          </th>

          <th class="px-6 py-3 text-left text-xs font-semibold uppercase">
            Urgency
          </th>

          <th class="px-6 py-3 text-left text-xs font-semibold uppercase">
            Created
          </th>
        </tr>
      </thead>

      <tbody>
        <tr
          v-for="referral in referrals"
          :key="referral.referralId"
          class="border-t hover:bg-slate-50"
        >
          <td class="px-6 py-4">
            #{{ referral.referralId }}
          </td>

          <td class="px-6 py-4">
            {{ referral.specialty }}
          </td>

          <td class="px-6 py-4">
            {{ referral.referralStatus }}
          </td>

          <td class="px-6 py-4">
            {{ referral.urgency }}
          </td>

          <td class="px-6 py-4">
            {{ referral.createdAt }}
          </td>
        </tr>
      </tbody>
    </table>
  </div>
</template>