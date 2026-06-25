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
  if (path === route.path) return true;
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
    class="flex w-[260px] shrink-0 flex-col border-r border-slate-200 bg-white"
  >
    <!-- Logo -->
    <div class="border-b border-slate-100 px-6 py-5">
      <div class="flex items-center gap-3">
        <div
          class="flex h-10 w-10 items-center justify-center rounded-xl bg-blue-600 text-white"
        >
          <NavIcon name="logo" />
        </div>

        <div>
          <p class="text-base font-bold text-slate-900">ReferralHub</p>
          <p class="text-xs text-slate-500">Care Coordination</p>
        </div>
      </div>
    </div>

    <!-- User card -->
    <div class="px-4 py-5">
      <div class="rounded-xl bg-blue-50 px-4 py-4">
        <div class="flex items-center gap-3">
          <div
            class="flex h-10 w-10 shrink-0 items-center justify-center rounded-full bg-blue-600 text-sm font-semibold text-white"
          >
            {{ userInitials }}
          </div>

          <div class="min-w-0">
            <p class="truncate text-sm font-semibold text-slate-900">
              {{ user.name }}
            </p>

            <p class="truncate text-xs text-slate-500">
              {{ user.role }}
            </p>
          </div>
        </div>
      </div>
    </div>

    <!-- Navigation -->
    <nav class="flex-1 space-y-1 px-4">
      <RouterLink
        v-for="link in navLinks"
        :key="link.path"
        :to="link.path"
        class="flex items-center gap-3 rounded-xl px-4 py-3 text-sm font-medium transition-colors hover:bg-slate-50"
        :class="
          isActive(link.path)
            ? 'bg-blue-600 text-white hover:bg-blue-600'
            : 'text-slate-600'
        "
      >
        <NavIcon :name="link.icon" />
        <span class="flex-1">{{ link.label }}</span>
        <NavIcon v-if="isActive(link.path)" name="chevron" />
      </RouterLink>
    </nav>

    <!-- Logout -->
    <div class="border-t border-slate-100 px-4 py-5">
      <button
        type="button"
        @click="handleLogout"
        class="flex w-full items-center gap-3 rounded-xl px-4 py-3 text-sm font-medium text-red-600 transition-colors hover:bg-red-50"
      >
        <NavIcon name="switch-role" />
        Logout
      </button>
    </div>
  </aside>
</template>
