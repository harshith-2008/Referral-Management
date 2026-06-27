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
const errorMessage = ref("");
const referrals = ref<ReferralDTO[]>([]);
const selectedReferral = ref<ReferralDTO | null>(null);

const loading = ref(false);

const loadReferrals = async () => {
  loading.value = true;
  errorMessage.value = "";

  try {
    const response = await getOriginFacilityReferrals();

    referrals.value = response.data.data ?? [];
  } catch (error) {
    console.error("Failed to load referrals:", error);

    errorMessage.value = getErrorMessage(error);

    referrals.value = [];
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
    title="Referral List"
    subtitle="Manage all referrals"
    :notification-count="2"
  >
    <div
      v-if="loading"
      class="rounded-xl bg-white p-8 text-center text-slate-500"
    >
      Loading referrals...
    </div>

    <div
      v-else-if="errorMessage"
      class="rounded-xl border border-red-200 bg-red-50 p-6 text-center"
    >
      <p class="font-medium text-red-700">
        {{ errorMessage }}
      </p>

      <button
        @click="loadReferrals"
        class="mt-4 rounded-lg bg-red-600 px-4 py-2 text-sm font-medium text-white hover:bg-red-700"
      >
        Try Again
      </button>
    </div>

    <template v-else>
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
    </template>
  </DashboardLayout>
</template>
