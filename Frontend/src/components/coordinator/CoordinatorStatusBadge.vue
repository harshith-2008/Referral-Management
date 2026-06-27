<script setup lang="ts">
import type { CoordinatorReferralStatus } from "../../types/coordinatorReferral";

defineProps<{
  status: CoordinatorReferralStatus;
}>();

const statusDotClass: Record<CoordinatorReferralStatus, string> = {
  Submitted: "bg-slate-500",
  Requested: "bg-purple-500",
  Accepted: "bg-green-500",
  Rejected: "bg-red-500",
  Completed: "bg-emerald-500",
  Closed: "bg-emerald-600",
};

const statusTextClass: Record<CoordinatorReferralStatus, string> = {
  Submitted: "text-slate-600",
  Requested: "text-purple-600",
  Accepted: "text-green-600",
  Rejected: "text-red-600",
  Completed: "text-emerald-600",
  Closed: "text-emerald-700",
};

const statusPillClass: Record<CoordinatorReferralStatus, string> = {
  Submitted: "bg-slate-100 text-slate-700",
  Requested: "bg-purple-50 text-purple-700",
  Accepted: "bg-green-50 text-green-700",
  Rejected: "bg-red-50 text-red-700",
  Completed: "bg-emerald-50 text-emerald-700",
  Closed: "bg-emerald-50 text-emerald-700",
};
</script>

<template>
  <span
    class="inline-flex items-center gap-2 rounded-full px-2.5 py-1 text-xs font-semibold"
    :class="statusPillClass[status]"
  >
    <span class="h-1.5 w-1.5 shrink-0 rounded-full" :class="statusDotClass[status]" />
    <span :class="statusTextClass[status]">{{ status }}</span>
  </span>
</template>
