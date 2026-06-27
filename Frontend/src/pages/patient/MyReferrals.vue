<script setup lang="ts">
import { onMounted, ref } from "vue";

import DashboardLayout from "../../components/layout/DashboardLayout.vue";
import { patientNavLinks } from "../../config/navigation";

import { getReferrals, getReferral } from "../../api/patient";

import type { ReferralDTO, ReferralDetailsDTO } from "../../types/patient";

/* ---------------- REFERRALS LIST ---------------- */
const referrals = ref<ReferralDTO[]>([]);

/* ---------------- MODAL STATE ---------------- */
const showModal = ref(false);
const loadingDetails = ref(false);
const selectedReferral = ref<ReferralDetailsDTO | null>(null);

/* ---------------- FORMATTERS ---------------- */
const formatDateTime = (date?: string | null) =>
  date ? new Date(date).toLocaleString() : "—";

const formatAppointmentDate = (date?: string | null) =>
  date
    ? new Date(`${date}T00:00:00`).toLocaleDateString(undefined, {
        weekday: "short",
        day: "numeric",
        month: "short",
        year: "numeric",
      })
    : "—";

const formatAppointmentTime = (time?: string | null) =>
  time?.slice(0, 5) || "—";

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
    selectedReferral.value = null;

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
    <!-- ================= TABLE ================= -->
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
      class="fixed inset-0 z-50 flex items-start justify-center overflow-y-auto bg-black/50 p-6 backdrop-blur-sm sm:p-10"
      @click.self="closeModal"
    >
      <div class="w-full max-w-lg space-y-4 pb-10">
        <!-- Header -->
        <div
          class="flex items-start justify-between gap-4 rounded-2xl bg-white px-6 py-5 shadow-2xl"
        >
          <div>
            <p class="mb-2 text-xs font-medium text-slate-400">
              Referral Details
            </p>

            <div
              v-if="loadingDetails"
              class="h-7 w-40 animate-pulse rounded-lg bg-slate-100"
            />

            <h2
              v-else
              class="text-xl font-semibold leading-tight text-slate-900"
            >
              {{ selectedReferral?.specialty ?? "—" }}
            </h2>

            <p class="mt-1 text-[12px] text-slate-400">
              {{
                selectedReferral
                  ? formatDateTime(selectedReferral.createdAt)
                  : ""
              }}
            </p>
          </div>

          <button
            type="button"
            class="mt-1 flex h-8 w-8 shrink-0 items-center justify-center rounded-full text-slate-400 transition-colors hover:bg-slate-100 hover:text-slate-700"
            @click="closeModal"
          >
            <svg viewBox="0 0 24 24" fill="none" class="h-4 w-4">
              <path
                d="M18 6L6 18M6 6l12 12"
                stroke="currentColor"
                stroke-width="2"
                stroke-linecap="round"
              />
            </svg>
          </button>
        </div>

        <!-- Body -->
        <div class="rounded-2xl bg-white px-6 py-5 shadow-xl">
          <!-- Loading -->
          <div v-if="loadingDetails" class="space-y-4">
            <div v-for="i in 4" :key="i" class="space-y-1.5">
              <div class="h-2.5 w-16 animate-pulse rounded bg-slate-100" />
              <div class="h-4 w-48 animate-pulse rounded bg-slate-100" />
            </div>
          </div>

          <!-- Content -->
          <div
            v-else-if="selectedReferral"
            class="grid grid-cols-2 gap-x-8 gap-y-5"
          >
            <div>
              <p
                class="mb-1 text-[10px] font-medium uppercase tracking-wider text-slate-400"
              >
                Specialty
              </p>
              <p class="text-sm font-medium text-slate-800">
                {{ selectedReferral.specialty }}
              </p>
            </div>

            <div>
              <p
                class="mb-1 text-[10px] font-medium uppercase tracking-wider text-slate-400"
              >
                Status
              </p>
              <p class="text-sm font-medium text-slate-800">
                {{ selectedReferral.referralStatus }}
              </p>
            </div>

            <div>
              <p
                class="mb-1 text-[10px] font-medium uppercase tracking-wider text-slate-400"
              >
                Urgency
              </p>
              <p class="text-sm font-medium text-slate-800">
                {{ selectedReferral.urgency }}
              </p>
            </div>

            <div>
              <p
                class="mb-1 text-[10px] font-medium uppercase tracking-wider text-slate-400"
              >
                Origin Facility
              </p>
              <p class="text-sm font-medium text-slate-800">
                {{ selectedReferral.originFacility || "—" }}
              </p>
            </div>

            <div>
              <p
                class="mb-1 text-[10px] font-medium uppercase tracking-wider text-slate-400"
              >
                Destination Facility
              </p>
              <p class="text-sm font-medium text-slate-800">
                {{ selectedReferral.destinationFacility || "Being arranged" }}
              </p>
            </div>

            <div>
              <p
                class="mb-1 text-[10px] font-medium uppercase tracking-wider text-slate-400"
              >
                Created At
              </p>
              <p class="text-sm font-medium text-slate-800">
                {{ formatDateTime(selectedReferral.createdAt) }}
              </p>
            </div>

            <!-- Appointment Details -->
            <div
              class="col-span-2 rounded-xl border border-blue-100 bg-blue-50/60 px-4 py-4"
            >
              <p
                class="text-[10px] font-semibold uppercase tracking-wider text-blue-600"
              >
                Appointment details
              </p>

              <template v-if="selectedReferral.appointmentId">
                <div class="mt-3 grid grid-cols-2 gap-x-6 gap-y-3">
                  <div>
                    <p class="text-xs text-slate-500">Date & time</p>
                    <p class="mt-0.5 text-sm font-semibold text-slate-800">
                      {{
                        formatAppointmentDate(selectedReferral.appointmentDate)
                      }}
                      ·
                      {{
                        formatAppointmentTime(selectedReferral.appointmentTime)
                      }}
                    </p>
                  </div>

                  <div>
                    <p class="text-xs text-slate-500">Status</p>
                    <p class="mt-0.5 text-sm font-semibold text-slate-800">
                      {{ selectedReferral.appointmentStatus || "—" }}
                    </p>
                  </div>

                  <div>
                    <p class="text-xs text-slate-500">Specialist</p>
                    <p class="mt-0.5 text-sm font-semibold text-slate-800">
                      {{ selectedReferral.specialistName || "—" }}
                    </p>
                  </div>

                  <div>
                    <p class="text-xs text-slate-500">Care facility</p>
                    <p class="mt-0.5 text-sm font-semibold text-slate-800">
                      {{ selectedReferral.specialistFacility || "—" }}
                    </p>
                  </div>
                </div>
              </template>

              <p v-else class="mt-2 text-sm text-slate-600">
                An appointment has not been scheduled for this referral yet.
              </p>
            </div>

            <div class="col-span-2">
              <p
                class="mb-1 text-[10px] font-medium uppercase tracking-wider text-slate-400"
              >
                Referral reason
              </p>
              <p class="text-sm font-medium text-slate-800">
                {{ selectedReferral.referralReason || "Not provided" }}
              </p>
            </div>

            <div class="col-span-2">
              <p
                class="mb-1 text-[10px] font-medium uppercase tracking-wider text-slate-400"
              >
                Diagnosis code
              </p>
              <p class="text-sm font-medium text-slate-800">
                {{ selectedReferral.diagnosisCode || "Not provided" }}
              </p>
            </div>
          </div>

          <!-- Null state -->
          <div
            v-else
            class="flex items-start gap-3 rounded-xl border border-red-100 bg-red-50 px-4 py-3"
          >
            <svg
              xmlns="http://www.w3.org/2000/svg"
              class="mt-0.5 h-4 w-4 shrink-0 text-red-400"
              fill="none"
              viewBox="0 0 24 24"
              stroke="currentColor"
              stroke-width="2"
            >
              <circle cx="12" cy="12" r="10" />
              <line x1="12" y1="8" x2="12" y2="12" />
              <line x1="12" y1="16" x2="12.01" y2="16" />
            </svg>

            <p class="text-sm text-red-600">Failed to load referral details.</p>
          </div>
        </div>
      </div>
    </div>
  </DashboardLayout>
</template>
