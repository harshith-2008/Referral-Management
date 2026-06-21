<script setup lang="ts">
import { ref } from "vue";
import DashboardLayout from "../../components/layout/DashboardLayout.vue";
import { specialistNavLinks } from "../../config/navigation";

const user = ref({
  name: "Dr. James Rivera",
  welcomeName: "Dr. Rivera",
  role: "Cardiologist",
  initials: "JR",
});
</script>

<template>
  <DashboardLayout
    :nav-links="specialistNavLinks"
    :user="user"
    title="Create Referral"
    subtitle="Submit a new referral request for coordinator routing"
    :notification-count="2"
  >
    <div class="rounded-xl border border-slate-100 bg-white p-8 shadow-sm">
      <p class="text-sm text-slate-500">
        Referral creation form will go here. Connect this page to your backend when ready.
      </p>
    </div>
  </DashboardLayout>
</template>
