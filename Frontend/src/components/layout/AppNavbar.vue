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
const role = ref("");

onMounted(async () => {
  try {
    const data = await getCurrentUser();

    firstName.value = data.firstName ?? "";
    lastName.value = data.lastName ?? "";
    role.value = data.role ?? "";
  } catch (error) {
    console.error("Failed to load user profile", error);
  }
});

const userInitials = computed(
  () => (firstName.value?.[0] ?? "") + (lastName.value?.[0] ?? ""),
);

const formattedRole = computed(
  () => role.value.replace(/([a-z])([A-Z])/g, "$1 $2") || "User",
);
</script>

<template>
  <header
    class="flex h-[72px] shrink-0 items-center justify-between border-b border-slate-100 bg-white px-8"
  >
    <div>
      <h1 class="text-3xl font-semibold text-slate-900 leading-tight">
        {{ title }}
      </h1>
      <!-- <p class="text-[12px] text-slate-400 mt-0.5">{{ subtitle }}</p> -->
    </div>

    <div class="flex items-center gap-4">
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
        class="flex items-center gap-3 rounded-2xl border border-slate-100 bg-slate-50 px-4 py-2 shadow-sm"
      >
        <div class="text-right">
          <!-- <p class="text-[11px] font-semibold uppercase tracking-[0.22em] text-slate-400">
            Current Role
          </p> -->
          <p class="text-2xl font-bold leading-tight text-slate-950">
            {{ formattedRole }}
          </p>
        </div>

        <div
          class="flex h-10 w-10 items-center justify-center rounded-full bg-blue-600 text-[12px] font-bold text-white shadow-sm"
        >
          {{ userInitials }}
        </div>
      </div>
    </div>
  </header>
</template>
