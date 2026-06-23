<script setup lang="ts">
import { computed } from "vue";

export interface StatCardItem {
  label: string;
  value: number | string;
  subtext?: string;
  iconBg: string;
  iconColor: string;
  icon: "clipboard" | "clock" | "check" | "calendar" | "archive" | "users" | "x";
}

const props = withDefaults(
  defineProps<{
    items: StatCardItem[];
    columns?: 5 | 6;
  }>(),
  { columns: 5 },
);

const gridClass = computed(() => (props.columns === 6 ? "grid-cols-6" : "grid-cols-5"));
</script>

<template>
  <div class="grid gap-4" :class="gridClass">
    <div
      v-for="item in items"
      :key="item.label"
      class="rounded-xl border border-slate-100 bg-white p-5 shadow-sm"
    >
      <div class="flex items-start justify-between">
        <p class="text-sm font-medium text-slate-500">{{ item.label }}</p>
        <div
          class="flex h-9 w-9 items-center justify-center rounded-lg"
          :class="[item.iconBg, item.iconColor]"
        >
          <svg
            v-if="item.icon === 'clipboard'"
            viewBox="0 0 24 24"
            fill="none"
            class="h-5 w-5"
            aria-hidden="true"
          >
            <path
              d="M9 5H7a2 2 0 0 0-2 2v12a2 2 0 0 0 2 2h10a2 2 0 0 0 2-2V7a2 2 0 0 0-2-2h-2"
              stroke="currentColor"
              stroke-width="1.75"
            />
            <rect x="9" y="3" width="6" height="4" rx="1" stroke="currentColor" stroke-width="1.75" />
          </svg>

          <svg
            v-else-if="item.icon === 'clock'"
            viewBox="0 0 24 24"
            fill="none"
            class="h-5 w-5"
            aria-hidden="true"
          >
            <circle cx="12" cy="12" r="8" stroke="currentColor" stroke-width="1.75" />
            <path d="M12 8v4l3 2" stroke="currentColor" stroke-width="1.75" stroke-linecap="round" />
          </svg>

          <svg
            v-else-if="item.icon === 'check'"
            viewBox="0 0 24 24"
            fill="none"
            class="h-5 w-5"
            aria-hidden="true"
          >
            <circle cx="12" cy="12" r="8" stroke="currentColor" stroke-width="1.75" />
            <path d="M9 12l2 2 4-4" stroke="currentColor" stroke-width="1.75" stroke-linecap="round" />
          </svg>

          <svg
            v-else-if="item.icon === 'calendar'"
            viewBox="0 0 24 24"
            fill="none"
            class="h-5 w-5"
            aria-hidden="true"
          >
            <rect x="3" y="5" width="18" height="16" rx="2" stroke="currentColor" stroke-width="1.75" />
            <path d="M3 10h18M8 3v4M16 3v4" stroke="currentColor" stroke-width="1.75" stroke-linecap="round" />
          </svg>

          <svg
            v-else-if="item.icon === 'archive'"
            viewBox="0 0 24 24"
            fill="none"
            class="h-5 w-5"
            aria-hidden="true"
          >
            <rect x="3" y="4" width="18" height="4" rx="1" stroke="currentColor" stroke-width="1.75" />
            <path
              d="M5 8v10a2 2 0 0 0 2 2h10a2 2 0 0 0 2-2V8M10 12h4"
              stroke="currentColor"
              stroke-width="1.75"
              stroke-linecap="round"
            />
          </svg>

          <svg
            v-else-if="item.icon === 'users'"
            viewBox="0 0 24 24"
            fill="none"
            class="h-5 w-5"
            aria-hidden="true"
          >
            <circle cx="9" cy="8" r="3" stroke="currentColor" stroke-width="1.75" />
            <path d="M3 20c0-3.3 2.7-6 6-6s6 2.7 6 6" stroke="currentColor" stroke-width="1.75" stroke-linecap="round" />
            <path d="M16 7c1.7 0 3 1.3 3 3M19 20c0-2.5-1.5-4.5-4-5.5" stroke="currentColor" stroke-width="1.75" stroke-linecap="round" />
          </svg>

          <svg
            v-else-if="item.icon === 'x'"
            viewBox="0 0 24 24"
            fill="none"
            class="h-5 w-5"
            aria-hidden="true"
          >
            <circle cx="12" cy="12" r="8" stroke="currentColor" stroke-width="1.75" />
            <path d="M15 9l-6 6M9 9l6 6" stroke="currentColor" stroke-width="1.75" stroke-linecap="round" />
          </svg>
        </div>
      </div>

      <p class="mt-3 text-3xl font-bold text-slate-900">{{ item.value }}</p>
      <p v-if="item.subtext" class="mt-1 text-xs text-slate-500">{{ item.subtext }}</p>
    </div>
  </div>
</template>
