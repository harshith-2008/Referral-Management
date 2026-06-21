<script setup lang="ts">
import type { ReferralUrgency } from "../../types/referral";

defineProps<{
  urgency: ReferralUrgency;
}>();

const urgencyClass: Record<ReferralUrgency, string> = {
  Urgent: "bg-amber-50 text-amber-700",
  Routine: "bg-slate-100 text-slate-600",
  Emergency: "bg-red-50 text-red-600",
};
</script>

<template>
  <span
    class="inline-flex rounded-full px-2.5 py-1 text-xs font-semibold"
    :class="urgencyClass[urgency]"
  >
    {{ urgency }}
  </span>
</template>
