<script setup lang="ts">
defineProps<{
  urgency: string;
}>();

const urgencyClass: Record<string, string> = {
  Routine: "bg-slate-100 text-slate-600",
  Urgent: "bg-amber-50 text-amber-700",
  Emergency: "bg-red-50 text-red-600",
};
</script>

<template>
  <span
    class="inline-flex rounded-full px-2.5 py-1 text-xs font-semibold"
    :class="urgencyClass[urgency] ?? 'bg-slate-100 text-slate-600'"
  >
    {{ urgency }}
  </span>
</template>
