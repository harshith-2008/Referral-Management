<script setup lang="ts">
import type { ReferralStatus } from "../../types/referral";

defineProps<{
  status: ReferralStatus;
}>();

const statusDotClass: Record<ReferralStatus, string> = {
  "Under Review": "bg-purple-500",
  Scheduled: "bg-cyan-500",
  Pending: "bg-orange-500",
  Accepted: "bg-green-500",
  Completed: "bg-green-600",
  "Treatment Ongoing": "bg-blue-500",
  Rejected: "bg-red-500",
  Assigned: "bg-sky-500",
};

const statusTextClass: Record<ReferralStatus, string> = {
  "Under Review": "text-purple-600",
  Scheduled: "text-cyan-600",
  Pending: "text-orange-600",
  Accepted: "text-green-600",
  Completed: "text-green-700",
  "Treatment Ongoing": "text-blue-600",
  Rejected: "text-red-600",
  Assigned: "text-sky-600",
};
</script>

<template>
  <div class="flex items-center gap-2">
    <span class="h-2 w-2 shrink-0 rounded-full" :class="statusDotClass[status]" />
    <span class="text-sm font-medium" :class="statusTextClass[status]">
      {{ status }}
    </span>
  </div>
</template>
