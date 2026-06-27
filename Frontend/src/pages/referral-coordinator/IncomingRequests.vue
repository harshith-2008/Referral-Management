<script setup lang="ts">
import { onMounted, ref } from "vue";

import type {
  ReferralDTO,
  ReferralDetailDTO,
  SpecialistMatchDTO,
} from "../../types/referral";

import {
  getRequestedReferrals,
  getReferralDetails,
  getMatchingSpecialists,
  rejectReferral,
} from "../../api/referral";
import { getErrorMessage } from "../../utils/errorHandler";
import { formatDate, formatTime } from "../../utils/date";

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
const successMessage = ref("");
const referralPendingReject = ref<ReferralDTO | null>(null);
const rejecting = ref(false);

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
  successMessage.value = "";

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

const handleAppointmentSuccess = async (appointment: {
  specialistName: string;
  appointmentDate: string;
  appointmentTime: string;
}) => {
  const referralId = selectedReferral.value?.referralId;

  closeModal();
  await loadRequests();

  successMessage.value = `Appointment scheduled successfully for Referral #${referralId} with ${
    appointment.specialistName
  } on ${formatDate(appointment.appointmentDate)} at ${formatTime(
    appointment.appointmentTime,
  )}.`;
};

const handleRejectReferral = (referral: ReferralDTO) => {
  referralPendingReject.value = referral;
};

const closeRejectConfirm = () => {
  if (rejecting.value) return;
  referralPendingReject.value = null;
};

const confirmRejectReferral = async () => {
  if (!referralPendingReject.value) return;

  rejecting.value = true;
  errorMessage.value = "";
  successMessage.value = "";

  try {
    const referral = referralPendingReject.value;

    await rejectReferral(referral.referralId);
    successMessage.value = `Referral #${referral.referralId} was rejected and returned for rerouting.`;
    referralPendingReject.value = null;
    await loadRequests();
  } catch (error) {
    console.error("Failed to reject referral:", error);
    errorMessage.value = getErrorMessage(error);
  } finally {
    rejecting.value = false;
  }
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
    <div
      v-if="successMessage"
      class="mb-4 rounded-lg border border-green-200 bg-green-50 p-4 text-sm text-green-700"
    >
      {{ successMessage }}
    </div>
    <IncomingRequestsTable
      :referrals="referrals"
      @view="openModal"
      @reject="handleRejectReferral"
    />
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
      @success="handleAppointmentSuccess"
    />

    <div
      v-if="referralPendingReject"
      class="fixed inset-0 z-50 flex items-center justify-center bg-black/50 px-4 backdrop-blur-sm"
      @click.self="closeRejectConfirm"
    >
      <div class="w-full max-w-md rounded-2xl bg-white p-6 shadow-2xl">
        <div class="flex items-start gap-4">
          <div
            class="flex h-10 w-10 shrink-0 items-center justify-center rounded-full bg-red-50 text-red-600"
          >
            <svg viewBox="0 0 24 24" fill="none" class="h-5 w-5">
              <path
                d="M12 9v4m0 4h.01M10.29 3.86 1.82 18a2 2 0 0 0 1.71 3h16.94a2 2 0 0 0 1.71-3L13.71 3.86a2 2 0 0 0-3.42 0Z"
                stroke="currentColor"
                stroke-width="2"
                stroke-linecap="round"
                stroke-linejoin="round"
              />
            </svg>
          </div>

          <div>
            <h3 class="text-base font-semibold text-slate-900">
              Reject referral?
            </h3>
            <p class="mt-2 text-sm text-slate-600">
              Referral #{{ referralPendingReject.referralId }} for
              {{ referralPendingReject.patientName }} will be returned to the
              originating coordinator for rerouting.
            </p>
          </div>
        </div>

        <div class="mt-6 flex justify-end gap-3">
          <button
            type="button"
            class="rounded-xl px-4 py-2 text-sm font-medium text-slate-600 hover:bg-slate-100 disabled:cursor-not-allowed disabled:opacity-60"
            :disabled="rejecting"
            @click="closeRejectConfirm"
          >
            Cancel
          </button>

          <button
            type="button"
            class="rounded-xl bg-red-600 px-4 py-2 text-sm font-semibold text-white hover:bg-red-700 disabled:cursor-not-allowed disabled:opacity-60"
            :disabled="rejecting"
            @click="confirmRejectReferral"
          >
            {{ rejecting ? "Rejecting..." : "Reject Referral" }}
          </button>
        </div>
      </div>
    </div>
  </DashboardLayout>
</template>
