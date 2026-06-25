<script setup lang="ts">
import { ref, onMounted } from "vue";

import DashboardLayout from "../../components/layout/DashboardLayout.vue";
import ReferralReviewModal from "../../components/referrals/ReferralReviewModal.vue";
import MyReferralsTable from "../../components/specialist/MyReferralsTable.vue";

import { specialistNavLinks } from "../../config/navigation.ts";

import { getMyReferrals, getReferralDetails } from "../../api/referral.ts";

import { getErrorMessage } from "../../utils/errorHandler";

import type { ReferralDTO, ReferralDetailDTO } from "../../types/referral.ts";

const referrals = ref<ReferralDTO[]>([]);
const selectedReferral = ref<ReferralDetailDTO | null>(null);

const loading = ref(false);
const errorMessage = ref("");
const infoMessage = ref("");

const loadReferrals = async () => {
  errorMessage.value = "";
  infoMessage.value = "";

  try {
    loading.value = true;

    const response = await getMyReferrals();

    referrals.value = response.data.data ?? [];

    if (referrals.value.length === 0) {
      infoMessage.value = "You have not submitted any referrals yet.";
    }
  } catch (error) {
    referrals.value = [];
    errorMessage.value = getErrorMessage(error);
  } finally {
    loading.value = false;
  }
};

const openReferral = async (referral: ReferralDTO) => {
  errorMessage.value = "";

  try {
    const response = await getReferralDetails(referral.referralId);

    selectedReferral.value = response.data.data;
  } catch (error) {
    errorMessage.value = getErrorMessage(error);
  }
};

const closeReferral = () => {
  selectedReferral.value = null;
};

onMounted(loadReferrals);
</script>

<template>
  <DashboardLayout
    :nav-links="specialistNavLinks"
    title="My Referrals"
    subtitle="View all the referrals that you submitted"
    :notification-count="2"
  >
    <div class="space-y-4">
      <div
        v-if="errorMessage"
        class="rounded-lg border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-700"
      >
        {{ errorMessage }}
      </div>

      <div
        v-if="infoMessage"
        class="rounded-lg border border-blue-200 bg-blue-50 px-4 py-3 text-sm text-blue-700"
      >
        {{ infoMessage }}
      </div>

      <div
        v-if="loading"
        class="rounded-xl border border-slate-200 bg-white p-8 text-center text-slate-500"
      >
        Loading referrals...
      </div>

      <MyReferralsTable
        v-else
        :referrals="referrals"
        show-actions
        @view="openReferral"
      />
    </div>

    <ReferralReviewModal
      v-if="selectedReferral"
      :referral="selectedReferral"
      @close="closeReferral"
    />
  </DashboardLayout>
</template>
