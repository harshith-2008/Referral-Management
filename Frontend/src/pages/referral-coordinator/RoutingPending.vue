<script setup lang="ts">
import { onMounted, ref } from "vue";

import DashboardLayout from "../../components/layout/DashboardLayout.vue";
import RouteReferralModal from "../../components/coordinator/RouteReferralModal.vue";

import { coordinatorNavLinks } from "../../config/navigation";
import { getErrorMessage } from "../../utils/errorHandler";
import { getSubmittedPendingReferrals } from "../../api/referral";
import type { RoutingPendingReferral } from "../../types/routingPendingReferral";
import { mockRoutingPendingReferrals } from "../../data/mockRoutingPendingReferrals.ts";
import RoutingPendingTable from "../../components/coordinator/RoutingPendingTable.vue";

const referrals = ref<RoutingPendingReferral[]>([]);
const loading = ref(false);
const errorMessage = ref("");
const successMessage = ref("");
const selectedReferral = ref<any | null>(null);
const showRouteModal = ref(false);

const loadReferrals = async () => {
  loading.value = true;
  errorMessage.value = "";

  try {
    const response = await getSubmittedPendingReferrals();

    referrals.value = (response.data.data ?? []).map((referral) => ({
      ...referral,
      status: referral.status as RoutingPendingReferral["status"],
      urgency: referral.urgency as RoutingPendingReferral["urgency"],
      diagnosisCode: referral.diagnosisCode ?? "",
    }));
  } catch (error) {
    console.error("Failed to load referrals:", error);

    referrals.value = [...mockRoutingPendingReferrals];

    errorMessage.value = `Backend unavailable. Showing demo data. (${getErrorMessage(error)})`;
  } finally {
    loading.value = false;
  }
};

const openRouteModal = async (referral: RoutingPendingReferral) => {
  try {
    successMessage.value = "";

    selectedReferral.value = {
      ...referral,
    };

    showRouteModal.value = true;
  } catch (error) {
    console.error("Failed to open route modal:", error);

    errorMessage.value = getErrorMessage(error);
  }
};

const closeRouteModal = () => {
  showRouteModal.value = false;
  selectedReferral.value = null;
};

const handleSuccess = async (facilityCount: number) => {
  try {
    const referralId = selectedReferral.value?.referralId;

    closeRouteModal();
    await loadReferrals();

    successMessage.value = `Referral #${referralId} was routed successfully to ${facilityCount} facilit${
      facilityCount === 1 ? "y" : "ies"
    }.`;
  } catch (error) {
    console.error("Failed to refresh referrals:", error);

    errorMessage.value = getErrorMessage(error);
  }
};

onMounted(loadReferrals);
</script>

<template>
  <DashboardLayout
    :nav-links="coordinatorNavLinks"
    title="Routing Pending"
    subtitle="Route submitted referrals to other facilities"
    :notification-count="2"
  >
    <div
      v-if="errorMessage"
      class="mb-4 rounded-lg border border-amber-200 bg-amber-50 p-4 text-sm text-amber-700"
    >
      {{ errorMessage }}
    </div>

    <div
      v-if="successMessage"
      class="mb-4 rounded-lg border border-emerald-200 bg-emerald-50 p-4 text-sm text-emerald-700"
    >
      {{ successMessage }}
    </div>

    <RoutingPendingTable
      :referrals="referrals"
      show-filters
      show-summary
      show-actions
      action-label="Route"
      @route="openRouteModal"
    />

    <RouteReferralModal
      v-if="showRouteModal && selectedReferral"
      :referral="selectedReferral"
      @close="closeRouteModal"
      @success="handleSuccess"
    />
  </DashboardLayout>
</template>
