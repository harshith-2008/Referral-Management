<script setup lang="ts">
import { ref } from "vue";
import DashboardLayout from "../../components/layout/DashboardLayout.vue";
import ReferralsTable from "../../components/referrals/ReferralsTable.vue";
import ReferralReviewModal from "../../components/referrals/ReferralReviewModal.vue";
import { specialistNavLinks } from "../../config/navigation";
import { mockReferrals } from "../../data/mockReferrals";
import type { Referral } from "../../types/referral";

const user = ref({
  name: "Dr. James Rivera",
  welcomeName: "Dr. Rivera",
  role: "Cardiologist",
  initials: "JR",
});

const referrals = ref<Referral[]>([...mockReferrals]);
const selectedReferral = ref<Referral | null>(null);

const openReview = (referral: Referral) => {
  selectedReferral.value = referral;
};

const closeReview = () => {
  selectedReferral.value = null;
};
</script>

<template>
  <DashboardLayout
    :nav-links="specialistNavLinks"
    :user="user"
    title="Assigned Referrals"
    subtitle="Manage your referral workload"
    :notification-count="2"
  >
    <ReferralsTable
      :referrals="referrals"
      show-actions
      @review="openReview"
    />

    <ReferralReviewModal
      v-if="selectedReferral"
      :referral="selectedReferral"
      @close="closeReview"
    />
  </DashboardLayout>
</template>
