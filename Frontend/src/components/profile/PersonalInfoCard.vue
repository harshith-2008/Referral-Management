<script setup lang="ts">
import type { UserProfile } from "../../types/profile";

defineProps<{
  profile: UserProfile;
}>();

const fields = [
  { label: "First Name", key: "firstName" },
  { label: "Last Name", key: "lastName" },
  { label: "Email", key: "email" },
  { label: "Phone", key: "phone" },
  { label: "Department", key: "department" },
  { label: "Employee ID", key: "employeeId" },
] as const;
</script>

<template>
  <div class="rounded-xl border border-slate-100 bg-white shadow-sm">
    <div class="border-b border-slate-100 px-6 py-5">
      <h2 class="text-lg font-bold text-slate-900">Personal Information</h2>
    </div>

    <div class="px-6 py-6">
      <div class="mb-6 border-b border-slate-100 pb-6">
        <p class="text-lg font-bold text-slate-900">{{ profile.displayName }}</p>
        <p class="mt-1 text-sm text-slate-500">{{ profile.roleLabel }}</p>
      </div>

      <div class="grid grid-cols-2 gap-6">
        <div v-for="field in fields" :key="field.key">
          <p class="text-sm font-medium text-slate-500">{{ field.label }}</p>
          <div class="mt-2 rounded-xl border border-slate-200 bg-slate-50 px-4 py-3 text-sm text-slate-900">
            {{ profile[field.key] }}
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
