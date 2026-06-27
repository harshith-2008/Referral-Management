<script setup lang="ts">
import { computed, onMounted, ref } from "vue";
import { getCurrentUser } from "../../utils/currentUser";

defineProps<{
  title: string;
  subtitle: string;
  notificationCount?: number;
}>();

const firstName = ref("");
const lastName = ref("");

onMounted(async () => {
  try {
    const data = await getCurrentUser();

    firstName.value = data.firstName ?? "";
    lastName.value = data.lastName ?? "";
  } catch (error) {
    console.error("Failed to load user profile", error);
  }
});

const userInitials = computed(
  () => (firstName.value?.[0] ?? "") + (lastName.value?.[0] ?? ""),
);
</script>

<template>
  <header
    class="flex h-[72px] shrink-0 items-center justify-between border-b border-slate-100 bg-white px-8"
  >
    <div>
      <h1 class="text-[18px] font-semibold text-slate-900 leading-tight">
        {{ title }}
      </h1>
      <p class="text-[12px] text-slate-400 mt-0.5">{{ subtitle }}</p>
    </div>

    <div class="flex items-center gap-3">
      <!-- Notifications -->
      <!--
      <button
        type="button"
        class="relative flex h-8 w-8 items-center justify-center rounded-lg text-slate-400 transition-colors hover:bg-slate-50 hover:text-slate-600"
        aria-label="Notifications"
      >
        ...
      </button>
      -->

      <div
        class="flex h-8 w-8 items-center justify-center rounded-full bg-blue-600 text-[11px] font-semibold text-white"
      >
        {{ userInitials }}
      </div>
    </div>
  </header>
</template>
