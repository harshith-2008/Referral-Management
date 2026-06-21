<script setup lang="ts">
import { computed } from "vue";
import { useRoute } from "vue-router";
import DashboardLayout from "../../components/layout/DashboardLayout.vue";
import PersonalInfoCard from "../../components/profile/PersonalInfoCard.vue";
import {
  adminNavLinks,
  coordinatorNavLinks,
  patientNavLinks,
  specialistNavLinks,
} from "../../config/navigation";
import { mockCoordinatorProfile } from "../../data/mockCoordinatorProfile";
import { mockProfile } from "../../data/mockProfile";
import type { NavLink, SidebarUser } from "../../types/navigation";

const props = defineProps<{
  navLinks?: NavLink[];
  user?: SidebarUser;
}>();

const route = useRoute();

const navLinks = computed(() => {
  if (props.navLinks) return props.navLinks;
  if (route.path.startsWith("/admin")) return adminNavLinks;
  if (route.path.startsWith("/coordinator")) return coordinatorNavLinks;
  if (route.path.startsWith("/patient")) return patientNavLinks;
  return specialistNavLinks;
});

const user = computed<SidebarUser>(() => {
  if (props.user) return props.user;
  if (route.path.startsWith("/coordinator")) {
    return {
      name: "Sarah Mitchell",
      role: "Referral Coordinator",
      initials: "SM",
    };
  }
  return {
    name: "Dr. James Rivera",
    role: "Cardiologist",
    initials: "JR",
  };
});

const profile = computed(() => {
  if (route.path.startsWith("/coordinator")) return mockCoordinatorProfile;
  return mockProfile;
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
    <PersonalInfoCard :profile="profile" />
  </DashboardLayout>
</template>
