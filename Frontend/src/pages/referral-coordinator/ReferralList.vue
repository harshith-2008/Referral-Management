<script setup lang="ts">
import { onMounted, ref } from "vue";

import DashboardLayout from "../../components/layout/DashboardLayout.vue";
import CoordinatorReferralsTable from "../../components/coordinator/CoordinatorReferralsTable.vue";
import ReferralHistoryModal from "../../components/coordinator/ReferralHistoryModal.vue";

import { coordinatorNavLinks } from "../../config/navigation";

import { getOriginFacilityReferrals } from "../../api/referral";
import { getErrorMessage } from "../../utils/errorHandler";

import type { ReferralDTO } from "../../types/referral";

const user = ref({
  name: "Sarah Mitchell",
  welcomeName: "Sarah",
  role: "Referral Coordinator",
  initials: "SM",
});

const referrals = ref<ReferralDTO[]>([]);
const selectedReferral = ref<ReferralDTO | null>(null);

const loading = ref(false);

const loadReferrals = async () => {
  try {
    loading.value = true;

    const response = await getOriginFacilityReferrals();

    referrals.value = response.data.data;
  } catch (error) {
    alert(getErrorMessage(error));
  } finally {
    loading.value = false;
  }
};

const openView = (referral: ReferralDTO) => {
  selectedReferral.value = referral;
};

const closeView = () => {
  selectedReferral.value = null;
};

onMounted(loadReferrals);
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
