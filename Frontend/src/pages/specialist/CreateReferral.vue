```vue
<script setup lang="ts">
import { onMounted, reactive, ref } from "vue";

import { getErrorMessage } from "../../utils/errorHandler";
import DashboardLayout from "../../components/layout/DashboardLayout.vue";
import { specialistNavLinks } from "../../config/navigation";
import { startSignalR, listenToEvents } from "../../services/signalr";


import type {
  GetSpecialityDTO,
  GetUrgencyLevelDTO,
  ReferralIntakeCreateDTO,
} from "../../types/specialist.ts";
import type { PatientLookupDTO } from "../../types/patient";
import { getUrgencyLevels,  getSpecialities, createReferralIntake } from "../../api/specialist";
import { getPatientByMrn } from "../../api/patient";
import axios from "axios";

const user = ref({
  name: "Dr. James Rivera",
  welcomeName: "Dr. Rivera",
  role: "Cardiologist",
  initials: "JR",
});

const mrn = ref("");
const searching = ref(false);
const submitting = ref(false);
const loadingDropdowns = ref(false);

const patient = ref<PatientLookupDTO | null>(null);

const urgencyLevels = ref<GetUrgencyLevelDTO[]>([]);
const specialities = ref<GetSpecialityDTO[]>([]);

const searchError = ref("");
const searchSuccess = ref("");

const referralError = ref("");
const referralSuccess = ref("");

const form = reactive<ReferralIntakeCreateDTO>({
  patientId: 0,
  referralReason: "",
  diagnosisCode: "",
  urgencyLevelId: 0,
  specialtyRequestId: 0,
});

const loadUrgencies = async () => {
  try {
    const response = await getUrgencyLevels();

    urgencyLevels.value = response.data;
  } catch (error) {
    console.error("Failed to load urgency levels:", error);

    urgencyLevels.value = [];

    alert(getErrorMessage(error));
  }
};

const loadSpecialities = async () => {
  try {
    const response = await getSpecialities();

    specialities.value = response.data;
  } catch (error) {
    console.error("Failed to load specialities:", error);

    specialities.value = [];

    alert(getErrorMessage(error));
  }
};

const searchPatient = async () => {
  searchError.value = "";
  searchSuccess.value = "";

  if (!mrn.value.trim()) {
    searchError.value = "Please enter an MRN.";
    return;
  }

  searching.value = true;

  try {
    const response = await getPatientByMrn(mrn.value);

    patient.value = response.data.data;

    form.patientId = patient.value.patientId;

    searchSuccess.value = "Patient found successfully.";
  } catch (error) {
    searchError.value = getErrorMessage(error);
  } finally {
    searching.value = false;
  }
};

const createReferral = async () => {
  if (!patient.value) {
    referralError.value = "Please search for a patient first.";
    return;
  }

  if (
    !form.specialtyRequestId ||
    !form.urgencyLevelId ||
    !form.referralReason.trim()
  ) {
    referralError.value = "Please complete all required fields.";
    return;
  }

  submitting.value = true;

  referralError.value = "";
  referralSuccess.value = "";

  submitting.value = true;

  try {
    const response = await createReferralIntake(form);

    referralSuccess.value = response.data.message;

    form.patientId = patient.value.patientId;
    form.referralReason = "";
    form.diagnosisCode = "";
    form.urgencyLevelId = 0;
    form.specialtyRequestId = 0;
  } catch (error) {
    referralError.value = getErrorMessage(error);
  } finally {
    submitting.value = false;
  }
};
onMounted(async () => {
  loadingDropdowns.value = true;

  try {
    await Promise.all([loadUrgencies(), loadSpecialities()]);

    // ✅ START SIGNALR
    await startSignalR();

    // ✅ LISTEN EVENTS
    listenToEvents((data: any) => {
      console.log("📩 Real-time event:", data);

      // ✅ Show UI notification
      referralSuccess.value = data.message || "New update received";
    });

  } finally {
    loadingDropdowns.value = false;
  }
});
</script>

<template>
  <DashboardLayout
    :nav-links="specialistNavLinks"
    title="Create Referral"
    subtitle="Submit a new referral request for coordinator routing"
    :notification-count="2"
  >
    <div class="space-y-6">
      <!-- Patient Lookup -->

      <div class="rounded-xl border border-slate-100 bg-white p-6 shadow-sm">
        <h2 class="mb-4 text-lg font-semibold text-slate-900">
          Patient Lookup
        </h2>

        <div class="flex gap-3">
          <input
            v-model="mrn"
            type="text"
            placeholder="Enter patient MRN"
            class="flex-1 rounded-lg border border-slate-200 px-4 py-3 text-sm outline-none transition focus:border-blue-500"
          />

          <button
            @click="searchPatient"
            :disabled="searching"
            class="rounded-lg bg-blue-600 px-5 py-3 text-sm font-medium text-white transition hover:bg-blue-700 disabled:cursor-not-allowed disabled:opacity-50"
          >
            {{ searching ? "Searching..." : "Search Patient" }}
          </button>
        </div>
        <div
          v-if="searchError"
          class="mt-4 rounded-lg border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-700"
        >
          {{ searchError }}
        </div>

        <div
          v-if="searchSuccess"
          class="mt-4 rounded-lg border border-green-200 bg-green-50 px-4 py-3 text-sm text-green-700"
        >
          {{ searchSuccess }}
        </div>
      </div>

      <!-- Patient Information -->

      <div
        v-if="patient"
        class="rounded-xl border border-slate-100 bg-white p-6 shadow-sm"
      >
        <h2 class="mb-5 text-lg font-semibold text-slate-900">
          Patient Information
        </h2>

        <div class="grid grid-cols-2 gap-5">
          <div>
            <p class="text-xs uppercase tracking-wide text-slate-400">
              Full Name
            </p>

            <p class="mt-1 font-medium text-slate-900">
              {{ patient.fullName }}
            </p>
          </div>

          <div>
            <p class="text-xs uppercase tracking-wide text-slate-400">MRN</p>

            <p class="mt-1 font-medium text-slate-900">
              {{ patient.mrn }}
            </p>
          </div>

          <div>
            <p class="text-xs uppercase tracking-wide text-slate-400">
              Date of Birth
            </p>

            <p class="mt-1 font-medium text-slate-900">
              {{ patient.dob }}
            </p>
          </div>

          <div>
            <p class="text-xs uppercase tracking-wide text-slate-400">Gender</p>

            <p class="mt-1 font-medium text-slate-900">
              {{ patient.gender }}
            </p>
          </div>

          <div class="col-span-2">
            <p class="text-xs uppercase tracking-wide text-slate-400">
              Facility
            </p>

            <p class="mt-1 font-medium text-slate-900">
              {{ patient.facilityName }}
            </p>
          </div>
        </div>
      </div>

      <!-- Referral Details -->

      <div
        v-if="patient"
        class="rounded-xl border border-slate-100 bg-white p-6 shadow-sm"
      >
        <h2 class="mb-6 text-lg font-semibold text-slate-900">
          Referral Details
        </h2>

        <div class="grid grid-cols-2 gap-5">
          <div>
            <label class="mb-2 block text-sm font-medium text-slate-700">
              Specialty Request *
            </label>

            <select
              v-model="form.specialtyRequestId"
              class="w-full rounded-lg border border-slate-200 px-4 py-3 text-sm outline-none focus:border-blue-500"
            >
              <option :value="0">Select Specialty</option>

              <option
                v-for="speciality in specialities"
                :key="speciality.specialityId"
                :value="speciality.specialityId"
              >
                {{ speciality.specialityName }}
              </option>
            </select>
          </div>

          <div>
            <label class="mb-2 block text-sm font-medium text-slate-700">
              Urgency Level *
            </label>

            <select
              v-model="form.urgencyLevelId"
              class="w-full rounded-lg border border-slate-200 px-4 py-3 text-sm outline-none focus:border-blue-500"
            >
              <option :value="0">Select Urgency</option>

              <option
                v-for="urgency in urgencyLevels"
                :key="urgency.urgencyLevelId"
                :value="urgency.urgencyLevelId"
              >
                {{ urgency.levelName }}
              </option>
            </select>
          </div>

          <div class="col-span-2">
            <label class="mb-2 block text-sm font-medium text-slate-700">
              Diagnosis Code
            </label>

            <input
              v-model="form.diagnosisCode"
              type="text"
              placeholder="Optional diagnosis code"
              class="w-full rounded-lg border border-slate-200 px-4 py-3 text-sm outline-none focus:border-blue-500"
            />
          </div>

          <div class="col-span-2">
            <label class="mb-2 block text-sm font-medium text-slate-700">
              Referral Reason *
            </label>

            <textarea
              v-model="form.referralReason"
              rows="5"
              placeholder="Provide detailed referral reason..."
              class="w-full rounded-lg border border-slate-200 px-4 py-3 text-sm outline-none focus:border-blue-500"
            />
          </div>
        </div>
        <div
          v-if="referralError"
          class="mb-4 rounded-lg border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-700"
        >
          {{ referralError }}
        </div>

        <div
          v-if="referralSuccess"
          class="mb-4 rounded-lg border border-green-200 bg-green-50 px-4 py-3 text-sm text-green-700"
        >
          {{ referralSuccess }}
        </div>
        <div class="mt-8 flex justify-end">
          <button
            @click="createReferral"
            :disabled="submitting"
            class="rounded-lg bg-blue-600 px-6 py-3 text-sm font-medium text-white transition hover:bg-blue-700 disabled:cursor-not-allowed disabled:opacity-50"
          >
            {{ submitting ? "Creating Referral..." : "Create Referral" }}
          </button>
        </div>
      </div>
    </div>
  </DashboardLayout>
</template>
