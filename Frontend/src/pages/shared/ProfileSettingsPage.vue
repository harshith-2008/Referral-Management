<script setup lang="ts">
import { computed, ref, onMounted } from "vue";
import { useRoute } from "vue-router";

import DashboardLayout from "../../components/layout/DashboardLayout.vue";
import PersonalInfoCard from "../../components/profile/PersonalInfoCard.vue";

import {
  adminNavLinks,
  coordinatorNavLinks,
  patientNavLinks,
  specialistNavLinks,
} from "../../config/navigation";

import type { SidebarUser } from "../../types/navigation";
import type { UserProfile } from "../../types/profile";
import { getMe } from "../../api/authApi";

const route = useRoute();

const navLinks = computed(() => {
  if (route.path.startsWith("/admin")) return adminNavLinks;
  if (route.path.startsWith("/coordinator")) return coordinatorNavLinks;
  if (route.path.startsWith("/patient")) return patientNavLinks;
  return specialistNavLinks;
});

const user = ref<SidebarUser>({
  name: "",
  role: "",
  initials: "",
});
const profile = ref<UserProfile | null>(null);

onMounted(async () => {
  const res = await getMe();
  const data = res.data.data;

  profile.value = {
    userId: data.userId,
    firstName: data.firstName,
    lastName: data.lastName,
    email: data.email,
    phone: data.phone,
    role: data.role,
    facilityId: data.facilityId,
    facilityName: data.facilityName,
    patientId: data.patientId,
    specialistId: data.specialistId,
    referralCoordinatorId: data.referralCoordinatorId,
    adminId: data.adminId,
  };

  user.value = {
    name: `${data.firstName} ${data.lastName}`,
    role: data.role,
    initials: (data.firstName?.[0] ?? "") + (data.lastName?.[0] ?? ""),
  };
});
</script>

<template>
  <DashboardLayout
    :nav-links="navLinks"
    :user="user"
    title="Profile"
    subtitle="Manage your account"
    :notification-count="2"
  >
    <PersonalInfoCard v-if="profile" :profile="profile" />
  </DashboardLayout>
</template>
