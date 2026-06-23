<script setup lang="ts">
import { ref } from "vue";
import DashboardLayout from "../../components/layout/DashboardLayout.vue";
import CoordinatorReferralsTable from "../../components/coordinator/CoordinatorReferralsTable.vue";
import ReferralHistoryModal from "../../components/coordinator/ReferralHistoryModal.vue";
import { coordinatorNavLinks } from "../../config/navigation";
import { mockCoordinatorReferrals } from "../../data/mockCoordinatorReferrals";
import type { CoordinatorReferral } from "../../types/coordinatorReferral";

const user = ref({
  name: "Sarah Mitchell",
  welcomeName: "Sarah",
  role: "Referral Coordinator",
  initials: "SM",
});

const referrals = ref<CoordinatorReferral[]>([...mockCoordinatorReferrals]);
const selectedReferral = ref<CoordinatorReferral | null>(null);

const openView = (referral: CoordinatorReferral) => {
  selectedReferral.value = referral;
};

const closeView = () => {
  selectedReferral.value = null;
};
</script>

<template>
  <DashboardLayout
    :nav-links="coordinatorNavLinks"
    :user="user"
    title="Referral List"
    subtitle="Manage all referrals"
    :notification-count="2"
  >
    <CoordinatorReferralsTable
      :referrals="referrals"
      show-filters
      show-summary
      show-actions
      action-label="View"
      @view="openView"
    />

    <ReferralHistoryModal
      v-if="selectedReferral"
      :referral="selectedReferral"
      @close="closeView"
    />
  </DashboardLayout>
</template>
