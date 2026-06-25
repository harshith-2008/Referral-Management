<script setup lang="ts">
import { computed, onMounted, ref } from "vue";
import { getMe } from "../../api/authApi";

defineProps<{
  title: string;
  subtitle: string;
  notificationCount?: number;
}>();

const firstName = ref("");
const lastName = ref("");

onMounted(async () => {
  try {
    const res = await getMe();
    const data = res.data.data;

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
    class="flex h-[88px] shrink-0 items-center justify-between border-b border-slate-200 bg-white px-8"
  >
    <div>
      <h1 class="text-2xl font-bold text-slate-900">
        {{ title }}
      </h1>

      <p class="mt-0.5 text-sm text-slate-500">
        {{ subtitle }}
      </p>
    </div>

    <div class="flex items-center gap-4">
      <!-- Notifications -->

      <!--
      <button
        type="button"
        class="relative rounded-xl p-2 text-slate-500 transition-colors hover:bg-slate-50 hover:text-slate-700"
        aria-label="Notifications"
      >
        ...
      </button>
      -->

      <div
        class="flex h-10 w-10 items-center justify-center rounded-full bg-blue-600 text-sm font-semibold text-white"
      >
        {{ userInitials }}
      </div>
    </div>
  </header>
</template>
