<script setup lang="ts">
import type { NavIcon } from "../../types/navigation";

defineProps<{
  name: NavIcon | "logo" | "switch-role" | "chevron";
}>();
</script>

<template>
  <!-- Logo -->
  <svg
    v-if="name === 'logo'"
    viewBox="0 0 24 24"
    fill="none"
    class="h-5 w-5"
    aria-hidden="true"
  >
    <path
      d="M12 4v16M8 8c0-2.2 1.8-4 4-4s4 1.8 4 4M8 16c0 2.2 1.8 4 4 4s4-1.8 4-4"
      stroke="currentColor"
      stroke-width="2"
      stroke-linecap="round"
    />
  </svg>

  <!-- Dashboard -->
  <svg
    v-else-if="name === 'dashboard'"
    viewBox="0 0 24 24"
    fill="none"
    class="h-5 w-5"
    aria-hidden="true"
  >
    <rect x="3" y="3" width="7" height="7" rx="1.5" stroke="currentColor" stroke-width="1.75" />
    <rect x="14" y="3" width="7" height="7" rx="1.5" stroke="currentColor" stroke-width="1.75" />
    <rect x="3" y="14" width="7" height="7" rx="1.5" stroke="currentColor" stroke-width="1.75" />
    <rect x="14" y="14" width="7" height="7" rx="1.5" stroke="currentColor" stroke-width="1.75" />
  </svg>

  <!-- Referrals -->
  <svg
    v-else-if="name === 'referrals'"
    viewBox="0 0 24 24"
    fill="none"
    class="h-5 w-5"
    aria-hidden="true"
  >
    <path
      d="M9 5H7a2 2 0 0 0-2 2v12a2 2 0 0 0 2 2h10a2 2 0 0 0 2-2V7a2 2 0 0 0-2-2h-2"
      stroke="currentColor"
      stroke-width="1.75"
      stroke-linecap="round"
    />
    <rect x="9" y="3" width="6" height="4" rx="1" stroke="currentColor" stroke-width="1.75" />
    <path d="M9 12h6M9 16h4" stroke="currentColor" stroke-width="1.75" stroke-linecap="round" />
  </svg>

  <!-- Settings -->
  <svg
    v-else-if="name === 'settings'"
    viewBox="0 0 24 24"
    fill="none"
    class="h-5 w-5"
    aria-hidden="true"
  >
    <circle cx="12" cy="12" r="3" stroke="currentColor" stroke-width="1.75" />
    <path
      d="M12 2v2M12 20v2M4.93 4.93l1.41 1.41M17.66 17.66l1.41 1.41M2 12h2M20 12h2M4.93 19.07l1.41-1.41M17.66 6.34l1.41-1.41"
      stroke="currentColor"
      stroke-width="1.75"
      stroke-linecap="round"
    />
  </svg>

  <!-- Users -->
  <svg
    v-else-if="name === 'users'"
    viewBox="0 0 24 24"
    fill="none"
    class="h-5 w-5"
    aria-hidden="true"
  >
    <circle cx="9" cy="8" r="3" stroke="currentColor" stroke-width="1.75" />
    <path
      d="M3 20c0-3.3 2.7-6 6-6s6 2.7 6 6"
      stroke="currentColor"
      stroke-width="1.75"
      stroke-linecap="round"
    />
    <path
      d="M16 7c1.7 0 3 1.3 3 3M19 20c0-2.5-1.5-4.5-4-5.5"
      stroke="currentColor"
      stroke-width="1.75"
      stroke-linecap="round"
    />
  </svg>

  <!-- Reports -->
  <svg
    v-else-if="name === 'reports'"
    viewBox="0 0 24 24"
    fill="none"
    class="h-5 w-5"
    aria-hidden="true"
  >
    <path d="M4 20V10M10 20V4M16 20v-8M22 20H2" stroke="currentColor" stroke-width="1.75" stroke-linecap="round" />
  </svg>

  <!-- Profile -->
  <svg
    v-else-if="name === 'profile'"
    viewBox="0 0 24 24"
    fill="none"
    class="h-5 w-5"
    aria-hidden="true"
  >
    <circle cx="12" cy="8" r="3.5" stroke="currentColor" stroke-width="1.75" />
    <path
      d="M5 20c0-3.3 2.7-6 6-6s6 2.7 6 6"
      stroke="currentColor"
      stroke-width="1.75"
      stroke-linecap="round"
    />
  </svg>

  <!-- Create -->
  <svg
    v-else-if="name === 'create'"
    viewBox="0 0 24 24"
    fill="none"
    class="h-5 w-5"
    aria-hidden="true"
  >
    <path d="M12 5v14M5 12h14" stroke="currentColor" stroke-width="1.75" stroke-linecap="round" />
  </svg>

  <!-- Switch Role -->
  <svg
    v-else-if="name === 'switch-role'"
    viewBox="0 0 24 24"
    fill="none"
    class="h-5 w-5"
    aria-hidden="true"
  >
    <path
      d="M9 21H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h4M16 17l5-5-5-5M21 12H9"
      stroke="currentColor"
      stroke-width="1.75"
      stroke-linecap="round"
      stroke-linejoin="round"
    />
  </svg>

  <!-- Chevron -->
  <svg
    v-else-if="name === 'chevron'"
    viewBox="0 0 24 24"
    fill="none"
    class="h-4 w-4"
    aria-hidden="true"
  >
    <path d="M9 6l6 6-6 6" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" />
  </svg>
</template>
