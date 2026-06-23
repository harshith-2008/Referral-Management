<script setup lang="ts">
import { onMounted, ref } from "vue";

import DashboardLayout from "../../components/layout/DashboardLayout.vue";
import IncomingRequestsTable from "../../components/coordinator/IncomingRequestsTable.vue";
import AssignSpecialistModal from "../../components/coordinator/AssignSpecialistModal.vue";

import { coordinatorNavLinks } from "../../config/navigation";

const referrals = ref<any[]>([]);
const selectedReferral = ref(null);
const showModal = ref(false);

const loadRequests = async () => {
  // backend call later

  referrals.value = [
    {
      referralId: 101,
      patientName: "John Smith",
      originFacility: "City Hospital",
      destinationFacility: "Metro Health",
      specialty: "Cardiology",
      urgency: "Urgent",
      status: "Requested",
    },
    {
      referralId: 102,
      patientName: "Mary Johnson",
      originFacility: "Regional Medical Center",
      destinationFacility: "Metro Health",
      specialty: "Orthopedics",
      urgency: "Routine",
      status: "Requested",
    },
  ];
};

const openModal = (referral: any) => {
  selectedReferral.value = referral;
  showModal.value = true;
};

const closeModal = () => {
  showModal.value = false;
  selectedReferral.value = null;
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
      v-if="showModal && selectedReferral"
      :referral="selectedReferral"
      @close="closeModal"
    />
  </DashboardLayout>
</template>
