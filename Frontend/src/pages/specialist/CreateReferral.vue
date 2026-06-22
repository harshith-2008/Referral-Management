```vue
<script setup lang="ts">
import { onMounted, reactive, ref } from "vue";
import type { ApiResponseDTO } from "../../types/common";

import DashboardLayout from "../../components/layout/DashboardLayout.vue";
import { specialistNavLinks } from "../../config/navigation";

import api from "../../api/axios";
import type {
  GetSpecialityDTO,
  GetUrgencyLevelDTO,
  ReferralIntakeCreateDTO,
} from "../../types/specialist.ts";
import type { PatientLookupDTO } from "../../types/patient";

// TODO: Replace with logged in specialist id
const specialistId = 1;

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

const form = reactive<ReferralIntakeCreateDTO>({
  patientId: 0,
  referralReason: "",
  diagnosisCode: "",
  urgencyLevelId: 0,
  specialtyRequestId: 0,
});

const loadUrgencies = async () => {
  try {
    const response = await api.get<GetUrgencyLevelDTO[]>(
      "/specialist/urgencyLevels"
    );

    urgencyLevels.value = response.data;
  } catch (error) {
    console.error("Failed to load urgency levels:", error);

    urgencyLevels.value = [
      {
        urgencyLevelId: 1,
        levelName: "Routine",
      },
      {
        urgencyLevelId: 2,
        levelName: "Urgent",
      },
      {
        urgencyLevelId: 3,
        levelName: "Emergency",
      },
    ];
  }
};

const loadSpecialities = async () => {
  try {
    const response = await api.get<GetSpecialityDTO[]>(
      "/specialist/specialities"
    );

    specialities.value = response.data;
  } catch (error) {
    console.error("Failed to load specialities:", error);

    specialities.value = [
      {
        specialityId: 1,
        specialityName: "Cardiology",
      },
      {
        specialityId: 2,
        specialityName: "Neurology",
      },
      {
        specialityId: 3,
        specialityName: "Orthopedics",
      },
      {
        specialityId: 4,
        specialityName: "Dermatology",
      },
    ];
  }
};

const searchPatient = async () => {
  if (!mrn.value.trim()) {
    alert("Please enter an MRN.");
    return;
  }

  searching.value = true;

  try {
    const response = await api.get<ApiResponseDTO<PatientLookupDTO>>(
      `/patient/lookup/${mrn.value}`
    );

    patient.value = response.data.data;

    form.patientId = patient.value.patientId;
  } catch (error) {
    console.error("Patient lookup failed:", error);

    // fallback mock data
    patient.value = {
      patientId: 1001,
      mrn: mrn.value,
      fullName: "John Smith",
      dob: "1988-06-10",
      gender: "Male",
      facilityName: "Cardiology Department",
    };

    form.patientId = patient.value.patientId;
  } finally {
    searching.value = false;
  }
};

const createReferral = async () => {
  if (!patient.value) {
    alert("Please search for a patient first.");
    return;
  }

  if (
    !form.specialtyRequestId ||
    !form.urgencyLevelId ||
    !form.referralReason.trim()
  ) {
    alert("Please complete all required fields.");
    return;
  }

  submitting.value = true;

  try {
    const response = await api.post<ApiResponseDTO<number>>(
      `/specialist/referral-intake/${specialistId}`,
      form
    );
    console.log("Referral Id:", response.data.data);
    console.log("Message:", response.data.message);
    console.log("Referral created:", response.data);

    alert(response.data.message);

    form.patientId = patient.value.patientId;
    form.referralReason = "";
    form.diagnosisCode = "";
    form.urgencyLevelId = 0;
    form.specialtyRequestId = 0;
  } catch (error) {
    console.error("Referral creation failed:", error);

    alert("Referral creation failed.");
  } finally {
    submitting.value = false;
  }
};

onMounted(async () => {
  loadingDropdowns.value = true;

  try {
    await Promise.all([loadUrgencies(), loadSpecialities()]);
  } finally {
    loadingDropdowns.value = false;
  }
});
</script>

<template>
  <DashboardLayout
    :nav-links="specialistNavLinks"
    :user="user"
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
```
