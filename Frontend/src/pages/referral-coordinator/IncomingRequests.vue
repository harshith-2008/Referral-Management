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
const errorMessage = ref("");
const modalError = ref("");

const loading = ref(false);

const loadRequests = async () => {
  loading.value = true;
  errorMessage.value = "";

  try {
    const response = await getRequestedReferrals();

    referrals.value = response.data.data ?? [];
  } catch (error) {
    console.error("Failed to load requests:", error);

    referrals.value = [];
    errorMessage.value = getErrorMessage(error);
  } finally {
    loading.value = false;
  }
};

const openModal = async (referral: ReferralDTO) => {
  modalError.value = "";

  try {
    loading.value = true;

    const [detailsResponse, specialistsResponse] = await Promise.all([
      getReferralDetails(referral.referralId),
      getMatchingSpecialists(referral.referralId),
    ]);

    referralDetails.value = detailsResponse.data.data;

    specialists.value = specialistsResponse.data.data ?? [];

    selectedReferral.value = referral;

    showModal.value = true;

    if (!specialists.value.length) {
      modalError.value =
        "No matching specialists are currently available for this referral.";
    }
  } catch (error) {
    console.error("Failed to load referral details:", error);

    modalError.value = getErrorMessage(error);
  } finally {
    loading.value = false;
  }
};

const closeModal = () => {
  showModal.value = false;

  selectedReferral.value = null;
  referralDetails.value = null;
  specialists.value = [];
  modalError.value = "";
};

onMounted(loadRequests);
</script>

<template>
  <DashboardLayout
    :nav-links="coordinatorNavLinks"
    title="Incoming Requests"
    subtitle="Assign specialists and schedule appointments"
  >
    <div
      v-if="errorMessage"
      class="mb-4 rounded-lg border border-red-200 bg-red-50 p-4 text-sm text-red-700"
    >
      {{ errorMessage }}
    </div>
    <IncomingRequestsTable :referrals="referrals" @view="openModal" />
    <div
      v-if="modalError"
      class="mb-4 rounded-lg border border-amber-200 bg-amber-50 p-4 text-sm text-amber-700"
    >
      {{ modalError }}
    </div>
    <AssignSpecialistModal
      v-if="showModal && selectedReferral && referralDetails"
      :referral="selectedReferral"
      :details="referralDetails"
      :specialists="specialists"
      @close="closeModal"
    />
  </DashboardLayout>
</template>
