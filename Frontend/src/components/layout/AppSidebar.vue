<script setup lang="ts">
import { computed, onMounted, ref } from "vue";
import { useRoute, useRouter } from "vue-router";

import NavIcon from "../icons/NavIcon.vue";

import type { NavLink, SidebarUser } from "../../types/navigation";

import { logout, getMe } from "../../api/authApi";

const props = defineProps<{
  navLinks: NavLink[];
}>();

const route = useRoute();
const router = useRouter();

const user = ref<SidebarUser>({
  name: "",
  role: "",
  initials: "",
});

const isActive = (path: string) => {
  // Exact match
  if (route.path === path) {
    return true;
  }

  // Dashboard pages should only match exactly
  const dashboardRoutes = ["/admin", "/patient", "/coordinator", "/specialist"];

  if (dashboardRoutes.includes(path)) {
    return false;
  }

  // Other menu items can match nested routes
  return route.path.startsWith(`${path}/`);
};

const userInitials = computed(() => user.value.initials);

onMounted(async () => {
  try {
    const res = await getMe();
    const data = res.data.data;

    user.value = {
      name: `${data.firstName} ${data.lastName}`,
      role: data.role,
      initials: (data.firstName?.[0] ?? "") + (data.lastName?.[0] ?? ""),
    };
  } catch (error) {
    console.error("Failed to load user profile", error);
  }
});

const handleLogout = async () => {
  try {
    await logout();
  } catch {
    // ignore API failure
  }

  localStorage.removeItem("token");

  router.push("/login");
};
</script>
<template>
  <aside
    class="flex w-[260px] shrink-0 flex-col bg-[#0f1117] border-r border-white/5"
  >
    <!-- Logo -->
    <div class="px-5 py-5">
      <div class="flex items-center gap-3">
        <div
          class="flex h-9 w-9 items-center justify-center rounded-xl bg-blue-600 shrink-0"
        >
          <NavIcon name="logo" />
        </div>
        <div>
          <p class="text-[15px] font-semibold text-white leading-tight">
            ReferralHub
          </p>
          <p class="text-[11px] text-slate-500 mt-0.5">Care Coordination</p>
        </div>
      </div>
    </div>

    <!-- User card -->
    <div class="px-4 pb-4">
      <div class="flex items-center gap-3 rounded-xl bg-white/5 px-3.5 py-3">
        <div
          class="flex h-8 w-8 shrink-0 items-center justify-center rounded-full bg-blue-600 text-[12px] font-semibold text-white"
        >
          {{ userInitials }}
        </div>
        <div class="min-w-0 flex-1">
          <p class="truncate text-[13px] font-medium text-white">
            {{ user.name }}
          </p>
          <p class="truncate text-[11px] text-slate-500 mt-0.5">
            {{ user.role }}
          </p>
        </div>
        <div class="w-1.5 h-1.5 rounded-full bg-emerald-400 shrink-0" />
      </div>
    </div>

    <!-- Divider -->
    <div class="mx-4 h-px bg-white/5 mb-3" />

    <!-- Navigation -->
    <nav class="flex-1 px-3 space-y-0.5 overflow-y-auto">
      <RouterLink
        v-for="link in navLinks"
        :key="link.path"
        :to="link.path"
        class="group flex items-center gap-3 rounded-lg px-3 py-2.5 transition-all duration-150 text-[13px] font-medium"
        :class="
          isActive(link.path)
            ? 'bg-blue-600 text-white'
            : 'text-slate-400 hover:bg-white/5 hover:text-white'
        "
      >
        <span
          class="flex h-[18px] w-[18px] shrink-0 items-center justify-center transition-colors"
          :class="
            isActive(link.path)
              ? 'text-white'
              : 'text-slate-600 group-hover:text-slate-300'
          "
        >
          <NavIcon :name="link.icon" />
        </span>

        <span class="flex-1 truncate">{{ link.label }}</span>

        <span
          v-if="isActive(link.path)"
          class="flex items-center justify-center text-blue-200 opacity-70"
        >
          <NavIcon name="chevron" />
        </span>
      </RouterLink>
    </nav>

    <!-- Divider -->
    <div class="mx-4 h-px bg-white/5 mt-3" />

    <!-- Logout -->
    <div class="px-3 py-4">
      <button
        type="button"
        @click="handleLogout"
        class="group flex w-full items-center gap-3 rounded-lg px-3 py-2.5 text-[13px] font-medium text-slate-500 transition-all duration-150 hover:bg-white/5 hover:text-red-400"
      >
        <span
          class="flex h-[18px] w-[18px] shrink-0 items-center justify-center text-slate-600 transition-colors group-hover:text-red-400"
        >
          <NavIcon name="switch-role" />
        </span>
        Logout
      </button>
    </div>
  </aside>
</template>
