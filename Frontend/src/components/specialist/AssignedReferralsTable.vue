<script setup lang="ts">
import type { SpecialistPatientDTO } from "../../types/referral";
import { formatDate } from "../../utils/date";
import UrgencyBadge from "../referrals/UrgencyBadge.vue";

defineProps<{
  referrals: SpecialistPatientDTO[];
  viewAllLink?: string;
}>();
</script>

<template>
  <div class="rounded-xl border border-slate-100 bg-white shadow-sm">
    <div
      class="flex items-center justify-between border-b border-slate-100 px-6 py-5"
    >
      <h2 class="text-lg font-bold text-slate-900">Assigned Referrals</h2>
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
              Urgency
            </th>
            <th
              class="px-6 py-3 text-left text-xs font-semibold uppercase tracking-wide text-slate-500"
            >
              Specialty
            </th>
            <th
              class="px-6 py-3 text-left text-xs font-semibold uppercase tracking-wide text-slate-500"
            >
              Appointment Date
            </th>
          </tr>
        </thead>
        <tbody v-if="referrals.length > 0">
          <tr
            v-for="referral in referrals"
            :key="referral.referralId"
            class="border-b border-slate-100 last:border-b-0 transition-colors hover:bg-slate-50"
          >
            <td class="px-6 py-4">
              <span class="text-sm font-medium text-blue-600">
                {{ referral.referralId }}
              </span>
            </td>

            <td class="px-6 py-4 text-sm font-semibold text-slate-900">
              {{ referral.patientName }}
            </td>

            <td class="px-6 py-4">
              <UrgencyBadge :urgency="referral.urgency" />
            </td>

            <td class="px-6 py-4 text-sm text-slate-700">
              {{ referral.specialty }}
            </td>

            <td class="px-6 py-4 text-sm text-slate-700">
              {{ referral.appointmentDate ? formatDate(referral.appointmentDate) : "Not scheduled" }}
            </td>
          </tr>
        </tbody>

        <tbody v-else>
          <tr>
            <td colspan="5" class="px-6 py-12 text-center">
              <div class="flex flex-col items-center gap-2">
                <p class="text-base font-medium text-slate-700">
                  No assigned referrals
                </p>
                <p class="text-sm text-slate-500">
                  Referrals assigned to you will appear here.
                </p>
              </div>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>
