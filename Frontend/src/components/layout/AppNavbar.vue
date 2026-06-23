<script setup lang="ts">
defineProps<{
  title: string;
  subtitle: string;
  userInitials: string;
  notificationCount?: number;
}>();
</script>

<template>
  <header
    class="flex h-[88px] shrink-0 items-center justify-between border-b border-slate-200 bg-white px-8"
  >
    <div>
      <h1 class="text-2xl font-bold text-slate-900">{{ title }}</h1>
      <p class="mt-0.5 text-sm text-slate-500">{{ subtitle }}</p>
    </div>

    <div class="flex items-center gap-4">
      <button
        type="button"
        class="relative rounded-xl p-2 text-slate-500 transition-colors hover:bg-slate-50 hover:text-slate-700"
        aria-label="Notifications"
      >
        <svg viewBox="0 0 24 24" fill="none" class="h-6 w-6" aria-hidden="true">
          <path
            d="M15 17H9M18 8a6 6 0 1 0-12 0c0 7-3 9-3 9h18s-3-2-3-9"
            stroke="currentColor"
            stroke-width="1.75"
            stroke-linecap="round"
            stroke-linejoin="round"
          />
        </svg>
        <span
          v-if="notificationCount && notificationCount > 0"
          class="absolute -right-0.5 -top-0.5 flex h-5 min-w-5 items-center justify-center rounded-full bg-red-500 px-1 text-[10px] font-bold text-white"
        >
          {{ notificationCount }}
        </span>
      </button>

      <div
        class="flex h-10 w-10 items-center justify-center rounded-full bg-blue-600 text-sm font-semibold text-white"
      >
        {{ userInitials }}
      </div>
    </div>
  </header>
</template>
