<script setup lang="ts">
import { onMounted, ref } from "vue";

import DashboardLayout from "../../components/layout/DashboardLayout.vue";
import { patientNavLinks } from "../../config/navigation";

import { getReferrals, getReferral } from "../../api/patient";

import type { ReferralDTO } from "../../types/patient";

/* ---------------- USER ---------------- */
const user = ref({
  name: "tharun motipeta",
  welcomeName: "tharun",
  role: "Patient",
  initials: "TM",
});

/* ---------------- REFERRALS LIST ---------------- */
const referrals = ref<ReferralDTO[]>([]);

/* ---------------- MODAL STATE ---------------- */
const showModal = ref(false);
const loadingDetails = ref(false);
const selectedReferral = ref<any>(null);

/* ---------------- LOAD REFERRALS ---------------- */
const loadReferrals = async () => {
  try {
    const response = await getReferrals();
    referrals.value = response.data.data;
  } catch (error) {
    console.error("Failed to load referrals", error);
    referrals.value = [];
  }
};

/* ---------------- VIEW REFERRAL DETAILS ---------------- */
const viewReferral = async (referralId: number) => {
  try {
    showModal.value = true;
    loadingDetails.value = true;

    const response = await getReferral(referralId);
    selectedReferral.value = response.data.data;
  } catch (error) {
    console.error("Failed to load referral details", error);
    selectedReferral.value = null;
  } finally {
    loadingDetails.value = false;
  }
};

/* ---------------- CLOSE MODAL ---------------- */
const closeModal = () => {
  showModal.value = false;
  selectedReferral.value = null;
};

onMounted(loadReferrals);
</script>

<template>
  <DashboardLayout
    :nav-links="patientNavLinks"
    title="My Referrals"
    subtitle="Track all your referrals"
    :notification-count="1"
  >
    <!-- ================= UPDATED TABLE UI ================= -->
    <div class="rounded-xl border border-slate-100 bg-white shadow-sm">
      <div class="overflow-hidden">
        <table class="w-full">
          <thead>
            <tr class="border-b border-slate-100 bg-slate-50/50">
              <th
                class="px-6 py-3 text-left text-sm font-bold uppercase tracking-wide text-slate-700"
              >
                Referral ID
              </th>
              <th
                class="px-6 py-3 text-left text-sm font-bold uppercase tracking-wide text-slate-700"
              >
                Specialty
              </th>
              <th
                class="px-6 py-3 text-left text-sm font-bold uppercase tracking-wide text-slate-700"
              >
                Status
              </th>
              <th
                class="px-6 py-3 text-left text-sm font-bold uppercase tracking-wide text-slate-700"
              >
                Urgency
              </th>
              <th
                class="px-6 py-3 text-left text-sm font-bold uppercase tracking-wide text-slate-700"
              >
                Action
              </th>
            </tr>
          </thead>

          <tbody>
            <tr
              v-for="referral in referrals"
              :key="referral.referralId"
              class="border-b border-slate-100 last:border-b-0 transition-colors hover:bg-slate-50"
            >
              <td class="px-6 py-4">
                <span class="text-sm font-medium text-blue-600">
                  {{ referral.referralId }}
                </span>
              </td>

              <td class="px-6 py-4 text-sm text-slate-600">
                {{ referral.specialty }}
              </td>

              <td class="px-6 py-4 text-sm text-slate-600">
                {{ referral.referralStatus }}
              </td>

              <td class="px-6 py-4 text-sm text-slate-600">
                {{ referral.urgency }}
              </td>

              <td class="px-6 py-4">
                <button
                  class="inline-flex items-center gap-2 rounded-lg border border-blue-200 px-3 py-1.5 text-sm font-medium text-blue-600 transition-colors hover:bg-blue-50"
                  @click="viewReferral(referral.referralId)"
                >
                  View
                </button>
              </td>
            </tr>

            <!-- Empty state -->
            <tr v-if="referrals.length === 0">
              <td
                colspan="5"
                class="px-6 py-8 text-center text-sm text-slate-500"
              >
                No referrals found.
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- ================= MODAL ================= -->
    <div
      v-if="showModal"
      class="fixed inset-0 bg-gray-900/30 backdrop-blur-sm flex items-center justify-center z-50"
    >
      <div class="bg-white w-full max-w-lg rounded-xl shadow-lg p-6">
        <h2 class="text-xl font-semibold mb-4">Referral Details</h2>

        <div v-if="loadingDetails" class="text-center py-6">
          Loading referral details...
        </div>

        <div v-else-if="selectedReferral" class="space-y-2 text-sm">
          <p><b>Specialty:</b> {{ selectedReferral.specialty }}</p>
          <p><b>Status:</b> {{ selectedReferral.referralStatus }}</p>
          <p><b>Urgency:</b> {{ selectedReferral.urgency }}</p>
          <p><b>Origin Facility:</b> {{ selectedReferral.originFacility }}</p>
          <p>
            <b>Created At:</b>
            {{ new Date(selectedReferral.createdAt).toLocaleString() }}
          </p>
        </div>

        <div class="mt-6 text-right">
          <button
            class="bg-gray-500 text-white px-4 py-2 rounded"
            @click="closeModal"
          >
            Close
          </button>
        </div>
      </div>
    </div>
  </DashboardLayout>
</template>
