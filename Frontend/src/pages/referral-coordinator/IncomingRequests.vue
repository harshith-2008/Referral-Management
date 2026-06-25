<script setup lang="ts">
import { onMounted, ref } from "vue";

const user = ref({
  name: "Sarah Mitchell",
  welcomeName: "Sarah",
  role: "Referral Coordinator",
  initials: "SM",
});

import type {
  ReferralDTO,
  ReferralDetailDTO,
  SpecialistMatchDTO,
} from "../../types/referral";

import {
  getRequestedReferrals,
  getReferralDetails,
  getMatchingSpecialists,
} from "../../api/referral";
import { getErrorMessage } from "../../utils/errorHandler";

import DashboardLayout from "../../components/layout/DashboardLayout.vue";
import IncomingRequestsTable from "../../components/coordinator/IncomingRequestsTable.vue";
import AssignSpecialistModal from "../../components/coordinator/AssignSpecialistModal.vue";

import { coordinatorNavLinks } from "../../config/navigation";

const referrals = ref<ReferralDTO[]>([]);
const showModal = ref(false);

const selectedReferral = ref<ReferralDTO | null>(null);

const referralDetails = ref<ReferralDetailDTO | null>(null);

const specialists = ref<SpecialistMatchDTO[]>([]);

const loading = ref(false);

const loadRequests = async () => {
  loading.value = true;

  try {
    const response = await getRequestedReferrals();

    referrals.value = response.data.data;
  } catch (error) {
    alert(getErrorMessage(error));
  } finally {
    loading.value = false;
  }
};

const openModal = async (referral: ReferralDTO) => {
  try {
    loading.value = true;

    const [detailsResponse, specialistsResponse] = await Promise.all([
      getReferralDetails(referral.referralId),
      getMatchingSpecialists(referral.referralId),
    ]);

    referralDetails.value = detailsResponse.data.data;

    specialists.value = specialistsResponse.data.data;

    selectedReferral.value = referral;

    showModal.value = true;
  } catch (error) {
    alert(getErrorMessage(error));
  } finally {
    loading.value = false;
  }
};

const closeModal = () => {
  showModal.value = false;

  selectedReferral.value = null;
  referralDetails.value = null;
  specialists.value = [];
};

onMounted(loadRequests);
</script>

<template>
  <DashboardLayout
    :nav-links="coordinatorNavLinks"
    title="Incoming Requests"
    subtitle="Assign specialists and schedule appointments"
  >
    <IncomingRequestsTable :referrals="referrals" @view="openModal" />

    <AssignSpecialistModal
      v-if="showModal && selectedReferral && referralDetails"
      :referral="selectedReferral"
      :details="referralDetails"
      :specialists="specialists"
      @close="closeModal"
    />
  </DashboardLayout>
</template>
