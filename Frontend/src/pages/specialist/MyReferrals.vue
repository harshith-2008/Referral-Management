<script setup lang="ts">
import { ref, onMounted } from "vue";

import DashboardLayout from "../../components/layout/DashboardLayout.vue";
import ReferralReviewModal from "../../components/referrals/ReferralReviewModal.vue";

import { specialistNavLinks } from "../../config/navigation.ts";

import { getMyReferrals, getReferralDetails } from "../../api/referral.ts";
import MyReferralsTable from "../../components/specialist/MyReferralsTable.vue";
import type { ReferralDTO, ReferralDetailDTO } from "../../types/referral.ts";

const user = ref({
  name: "Dr. James Rivera",
  welcomeName: "Dr. Rivera",
  role: "Cardiologist",
  initials: "JR",
});

const referrals = ref<ReferralDTO[]>([]);
const selectedReferral = ref<ReferralDetailDTO | null>(null);

const loadReferrals = async () => {
  const response = await getMyReferrals();

  referrals.value = response.data.data;
};

const openReferral = async (referral: ReferralDTO) => {
  const response = await getReferralDetails(referral.referralId);

  selectedReferral.value = response.data.data;
};

const closeReferral = () => {
  selectedReferral.value = null;
};

onMounted(loadReferrals);
</script>

<template>
  <DashboardLayout
    :nav-links="specialistNavLinks"
    :user="user"
    title="My Referrals"
    subtitle="View all the referrals that you submitted"
    :notification-count="2"
  >
    <MyReferralsTable
      :referrals="referrals"
      show-actions
      @view="openReferral"
    />

    <ReferralReviewModal
      v-if="selectedReferral"
      :referral="selectedReferral"
      @close="closeReferral"
    />
  </DashboardLayout>
</template>
