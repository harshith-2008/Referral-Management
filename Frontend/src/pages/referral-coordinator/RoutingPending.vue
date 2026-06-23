<script setup lang="ts">
import { onMounted, ref } from "vue";

import DashboardLayout from "../../components/layout/DashboardLayout.vue";
import CoordinatorReferralsTable from "../../components/coordinator/CoordinatorReferralsTable.vue";
import RouteReferralModal from "../../components/coordinator/RouteReferralModal.vue";

import { coordinatorNavLinks } from "../../config/navigation";
import { getErrorMessage } from "../../utils/errorHandler";
// Placeholder API
import { getSubmittedPendingReferrals } from "../../api/referral";
import type { RoutingPendingReferral } from "../../types/routingPendingReferral";
import { mockRoutingPendingReferrals } from "../../data/mockRoutingPendingReferrals.ts";
import RoutingPendingTable from "../../components/coordinator/RoutingPendingTable.vue";

const user = ref({
  name: "Sarah Mitchell",
  welcomeName: "Sarah",
  role: "Referral Coordinator",
  initials: "SM",
});

const mockFacilities = [
  {
    facilityId: 1,
    facilityName: "City Medical Center - Cardiology",
    availableSpecialists: 4,
  },
  {
    facilityId: 2,
    facilityName: "Regional Hospital - Cardiology Unit",
    availableSpecialists: 2,
  },
  {
    facilityId: 3,
    facilityName: "Metro Health - Cardiac Sciences",
    availableSpecialists: 6,
  },
];

const referrals = ref<RoutingPendingReferral[]>([]);
const loading = ref(false);

const selectedReferral = ref<any | null>(null);
const showRouteModal = ref(false);

const loadReferrals = async () => {
  loading.value = true;

  try {
    const coordinatorId = 1;

    const response = await getSubmittedPendingReferrals(coordinatorId);

    referrals.value = response.data.data;
  } catch (error) {
    console.error("Failed to load referrals:", error);

    // fallback demo data
    referrals.value = [...mockRoutingPendingReferrals];

    alert(
      `Backend unavailable. Showing demo data.\n\n${getErrorMessage(error)}`,
    );
  } finally {
    loading.value = false;
  }
};

const openRouteModal = async (referral: RoutingPendingReferral) => {
  selectedReferral.value = {
    ...referral,
    facilities: mockFacilities,
  };

  showRouteModal.value = true;
};
const closeRouteModal = () => {
  showRouteModal.value = false;
  selectedReferral.value = null;
};

const handleSuccess = async () => {
  closeRouteModal();
  await loadReferrals();
};

onMounted(loadReferrals);
</script>

<template>
  <DashboardLayout
    :nav-links="coordinatorNavLinks"
    :user="user"
    title="Routing Pending"
    subtitle="Route submitted referrals to other facilities"
    :notification-count="2"
  >
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
