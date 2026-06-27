<script setup lang="ts">
import AppSidebar from "./AppSidebar.vue";
import AppNavbar from "./AppNavbar.vue";
import type { NavLink } from "../../types/navigation";

defineProps<{
  navLinks: NavLink[];
  title: string;
  subtitle: string;
  notificationCount?: number;
}>();
</script>

<template>
  <div class="flex min-h-screen min-w-[1280px] bg-slate-50">
    <AppSidebar :nav-links="navLinks" />

    <div class="flex min-w-0 flex-1 flex-col">
      <AppNavbar
        :title="title"
        :subtitle="subtitle"
        :notification-count="notificationCount"
      />

      <main class="flex-1 overflow-auto p-8">
        <slot />
      </main>
    </div>
  </div>
</template>
