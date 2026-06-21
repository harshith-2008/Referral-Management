<script setup lang="ts">
export interface ScheduleItem {
  time: string;
  duration: string;
  patientName: string;
  appointmentType: string;
}

defineProps<{
  dateLabel: string;
  items: ScheduleItem[];
}>();
</script>

<template>
  <div class="rounded-xl border border-slate-100 bg-white p-6 shadow-sm">
    <div class="mb-5 flex items-center justify-between">
      <h2 class="text-lg font-bold text-slate-900">Today's Schedule</h2>
      <span class="rounded-lg bg-blue-50 px-3 py-1 text-sm font-medium text-blue-600">
        {{ dateLabel }}
      </span>
    </div>

    <div class="space-y-4">
      <div
        v-for="item in items"
        :key="`${item.time}-${item.patientName}`"
        class="flex gap-4 rounded-xl border border-slate-100 p-4 transition-colors hover:bg-slate-50"
      >
        <div class="w-[72px] shrink-0 border-r border-blue-100 pr-4">
          <p class="text-sm font-semibold text-blue-600">{{ item.time }}</p>
          <p class="mt-0.5 text-xs text-slate-400">{{ item.duration }}</p>
        </div>
        <div>
          <p class="font-semibold text-slate-900">{{ item.patientName }}</p>
          <p class="text-sm text-slate-500">{{ item.appointmentType }}</p>
        </div>
      </div>
    </div>
  </div>
</template>
