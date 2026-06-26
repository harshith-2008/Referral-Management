<script setup lang="ts">
import { computed, onMounted, ref } from "vue";
import axios from "axios";

import { getFacilitiesDropdown, routeReferral } from "../../api/referral";

import type { FacilitiesDropdownResponseDTO } from "../../types/referral";

import { getErrorMessage } from "../../utils/errorHandler";

const props = defineProps<{
  referral: any;
}>();

const emit = defineEmits<{
  close: [];
  success: [];
}>();

const facilities = ref<FacilitiesDropdownResponseDTO>({
  inNetwork: [],
  outOfNetwork: [],
});
const selectedFacilities = ref<number[]>([]);

const allFacilities = computed(() => [
  ...facilities.value.inNetwork,
  ...facilities.value.outOfNetwork,
]);

const facilityGroups = computed(() => [
  {
    key: "in-network",
    title: "In network",
    description: "Facilities within the origin hospital network",
    facilities: facilities.value.inNetwork,
    badgeClass: "bg-emerald-50 text-emerald-700 ring-emerald-200",
  },
  {
    key: "out-of-network",
    title: "Out of network",
    description: "Facilities at other hospitals",
    facilities: facilities.value.outOfNetwork,
    badgeClass: "bg-amber-50 text-amber-700 ring-amber-200",
  },
].filter((group) => group.facilities.length));

const toggleFacility = (facilityId: number) => {
  const selectedIndex = selectedFacilities.value.indexOf(facilityId);

  if (selectedIndex >= 0) {
    selectedFacilities.value.splice(selectedIndex, 1);
    return;
  }

  selectedFacilities.value.push(facilityId);
};

const loadingFacilities = ref(false);
const submitting = ref(false);
const facilitiesMessage = ref("");
const facilitiesError = ref("");

const loadFacilities = async () => {
  loadingFacilities.value = true;

  facilitiesMessage.value = "";
  facilitiesError.value = "";

  try {
    const response = await getFacilitiesDropdown(props.referral.referralId);

    facilities.value = response.data.data ?? {
      inNetwork: [],
      outOfNetwork: [],
    };

    if (!allFacilities.value.length) {
      facilitiesMessage.value =
        "No facilities are currently available for this specialty.";
    }
  } catch (error) {
    console.error("Failed to load facilities:", error);

    facilities.value = { inNetwork: [], outOfNetwork: [] };

    if (axios.isAxiosError(error) && error.response?.status === 404) {
      facilitiesMessage.value =
        "No facilities are currently available for this specialty.";
    } else {
      facilitiesError.value = getErrorMessage(error);
    }
  } finally {
    loadingFacilities.value = false;
  }
};

const submitRouting = async () => {
  if (!selectedFacilities.value.length) {
    alert("Please select at least one facility.");
    return;
  }

  submitting.value = true;

  try {
    await routeReferral({
      referralId: props.referral.referralId,
      patientId: props.referral.patientId,
      originFacilityId: props.referral.originFacilityId,

      specialtyRequestId: props.referral.specialtyRequestId,

      referralReason: props.referral.referralReason,

      diagnosisCode: props.referral.diagnosisCode,

      urgencyLevelId: props.referral.urgencyLevelId,

      referralStatusId: props.referral.referralStatusId,

      destinationFacilityIds: selectedFacilities.value,
    });

    emit("success");
  } catch (error) {
    alert(getErrorMessage(error));
  } finally {
    submitting.value = false;
  }
};

onMounted(loadFacilities);
</script>

<template>
  <div
    class="fixed inset-0 z-50 flex items-end justify-center sm:items-center bg-black/50 backdrop-blur-sm"
  >
    <div
      class="bg-white w-full sm:max-w-lg sm:rounded-2xl shadow-2xl overflow-hidden flex flex-col max-h-[92vh]"
    >
      <!-- Header -->
      <div
        class="flex items-center justify-between px-6 py-4 border-b border-slate-100"
      >
        <div>
          <p
            class="text-xs font-medium tracking-widest text-slate-400 uppercase mb-0.5"
          >
            Referral Management
          </p>
          <h2 class="text-base font-semibold text-slate-900">Route Referral</h2>
        </div>
        <button
          @click="emit('close')"
          class="w-8 h-8 flex items-center justify-center rounded-full text-slate-400 hover:text-slate-700 hover:bg-slate-100 transition-colors"
        >
          <svg
            xmlns="http://www.w3.org/2000/svg"
            class="w-4 h-4"
            fill="none"
            viewBox="0 0 24 24"
            stroke="currentColor"
            stroke-width="2"
          >
            <path
              stroke-linecap="round"
              stroke-linejoin="round"
              d="M6 18L18 6M6 6l12 12"
            />
          </svg>
        </button>
      </div>

      <!-- Body -->
      <div class="overflow-y-auto flex-1 px-6 py-5 space-y-3">
        <!-- Loading -->
        <div
          v-if="loadingFacilities"
          class="flex items-center gap-2 text-sm text-slate-400 py-4"
        >
          <svg
            class="w-4 h-4 animate-spin text-blue-400"
            xmlns="http://www.w3.org/2000/svg"
            fill="none"
            viewBox="0 0 24 24"
          >
            <circle
              class="opacity-25"
              cx="12"
              cy="12"
              r="10"
              stroke="currentColor"
              stroke-width="4"
            />
            <path
              class="opacity-75"
              fill="currentColor"
              d="M4 12a8 8 0 018-8v8H4z"
            />
          </svg>
          Loading available facilities…
        </div>

        <!-- Info message (no facilities) -->
        <div
          v-else-if="facilitiesMessage"
          class="flex items-start gap-3 rounded-xl border border-amber-200 bg-amber-50 px-4 py-3"
        >
          <svg
            xmlns="http://www.w3.org/2000/svg"
            class="w-4 h-4 mt-0.5 text-amber-500 shrink-0"
            fill="none"
            viewBox="0 0 24 24"
            stroke="currentColor"
            stroke-width="2"
          >
            <path
              stroke-linecap="round"
              stroke-linejoin="round"
              d="M12 9v2m0 4h.01M10.29 3.86L1.82 18a2 2 0 001.71 3h16.94a2 2 0 001.71-3L13.71 3.86a2 2 0 00-3.42 0z"
            />
          </svg>
          <p class="text-sm text-amber-700">{{ facilitiesMessage }}</p>
        </div>

        <!-- Error message -->
        <div
          v-else-if="facilitiesError"
          class="flex items-start gap-3 rounded-xl border border-red-200 bg-red-50 px-4 py-3"
        >
          <svg
            xmlns="http://www.w3.org/2000/svg"
            class="w-4 h-4 mt-0.5 text-red-400 shrink-0"
            fill="none"
            viewBox="0 0 24 24"
            stroke="currentColor"
            stroke-width="2"
          >
            <circle cx="12" cy="12" r="10" />
            <line x1="12" y1="8" x2="12" y2="12" />
            <line x1="12" y1="16" x2="12.01" y2="16" />
          </svg>
          <p class="text-sm text-red-600">{{ facilitiesError }}</p>
        </div>

        <!-- Facility list -->
        <template v-if="allFacilities.length > 0">
          <p
            class="text-xs font-medium tracking-widest text-slate-400 uppercase mb-1"
          >
            Select Destinations
          </p>
          <section
            v-for="group in facilityGroups"
            :key="group.key"
            class="space-y-2"
          >
            <div class="flex items-start justify-between gap-3 pt-2">
              <div>
                <h3 class="text-sm font-semibold text-slate-800">{{ group.title }}</h3>
                <p class="text-xs text-slate-400">{{ group.description }}</p>
              </div>
              <span
                class="rounded-full px-2 py-1 text-xs font-medium ring-1 ring-inset"
                :class="group.badgeClass"
              >
                {{ group.facilities.length }} available
              </span>
            </div>
            <button
              v-for="facility in group.facilities"
              :key="facility.facilityId"
              class="w-full text-left flex items-center gap-4 rounded-xl border px-4 py-3 transition-all focus:outline-none focus-visible:ring-2 focus-visible:ring-blue-500"
              :class="
                selectedFacilities.includes(facility.facilityId)
                  ? 'border-blue-500 bg-blue-50 shadow-sm'
                  : 'border-slate-200 bg-white hover:border-slate-300 hover:bg-slate-50'
              "
              @click="toggleFacility(facility.facilityId)"
            >
            <!-- Icon -->
            <div
              class="w-9 h-9 rounded-full flex items-center justify-center shrink-0 transition-colors"
              :class="
                selectedFacilities.includes(facility.facilityId)
                  ? 'bg-blue-500'
                  : 'bg-slate-100'
              "
            >
              <svg
                xmlns="http://www.w3.org/2000/svg"
                class="w-4 h-4 transition-colors"
                :class="
                  selectedFacilities.includes(facility.facilityId)
                    ? 'text-white'
                    : 'text-slate-400'
                "
                fill="none"
                viewBox="0 0 24 24"
                stroke="currentColor"
                stroke-width="2"
              >
                <path
                  stroke-linecap="round"
                  stroke-linejoin="round"
                  d="M19 21V5a2 2 0 00-2-2H7a2 2 0 00-2 2v16m14 0h2m-2 0h-5m-9 0H3m2 0h5M9 7h1m-1 4h1m4-4h1m-1 4h1m-5 10v-5a1 1 0 011-1h2a1 1 0 011 1v5m-4 0h4"
                />
              </svg>
            </div>

            <!-- Info -->
              <div class="flex-1 min-w-0">
                <p class="text-sm font-medium text-slate-900 truncate">
                  {{ facility.facilityName }}
                </p>
                <p class="text-xs text-slate-400">
                  {{ facility.hospitalName }}<span v-if="facility.city || facility.state"> · {{ [facility.city, facility.state].filter(Boolean).join(', ') }}</span>
                </p>
                <p class="text-xs text-slate-400">
                  {{ facility.availableSpecialists }} specialists available
                </p>
              </div>

              <span
                class="shrink-0 rounded-full bg-slate-100 px-2 py-1 text-xs font-medium text-slate-600"
              >
                {{ facility.distanceKm == null ? "Distance unavailable" : `${facility.distanceKm.toFixed(1)} km away` }}
              </span>

            <!-- Checkbox -->
            <div
              class="w-5 h-5 rounded border-2 flex items-center justify-center shrink-0 transition-colors"
              :class="
                selectedFacilities.includes(facility.facilityId)
                  ? 'border-blue-500 bg-blue-500'
                  : 'border-slate-300'
              "
            >
              <svg
                v-if="selectedFacilities.includes(facility.facilityId)"
                xmlns="http://www.w3.org/2000/svg"
                class="w-3 h-3 text-white"
                fill="none"
                viewBox="0 0 24 24"
                stroke="currentColor"
                stroke-width="3"
              >
                <path
                  stroke-linecap="round"
                  stroke-linejoin="round"
                  d="M5 13l4 4L19 7"
                />
              </svg>
            </div>
            </button>
          </section>
        </template>
      </div>

      <!-- Footer -->
      <div
        class="px-6 py-4 border-t border-slate-100 flex items-center justify-between gap-3"
      >
        <p v-if="selectedFacilities.length" class="text-xs text-slate-400">
          {{ selectedFacilities.length }} facility{{
            selectedFacilities.length > 1 ? "ies" : ""
          }}
          selected
        </p>
        <p v-else class="text-xs text-slate-300">No facilities selected</p>

        <div class="flex items-center gap-3">
          <button
            class="px-4 py-2 rounded-xl text-sm font-medium text-slate-600 hover:bg-slate-100 transition-colors"
            @click="emit('close')"
          >
            Cancel
          </button>

          <button
            v-if="allFacilities.length > 0"
            :disabled="submitting || !selectedFacilities.length"
            class="px-5 py-2 rounded-xl text-sm font-semibold text-white bg-blue-600 hover:bg-blue-700 disabled:opacity-50 disabled:cursor-not-allowed transition-colors flex items-center gap-2"
            @click="submitRouting"
          >
            <svg
              v-if="submitting"
              class="w-4 h-4 animate-spin"
              xmlns="http://www.w3.org/2000/svg"
              fill="none"
              viewBox="0 0 24 24"
            >
              <circle
                class="opacity-25"
                cx="12"
                cy="12"
                r="10"
                stroke="currentColor"
                stroke-width="4"
              />
              <path
                class="opacity-75"
                fill="currentColor"
                d="M4 12a8 8 0 018-8v8H4z"
              />
            </svg>
            {{ submitting ? "Routing…" : "Route Referral" }}
          </button>
        </div>
      </div>
    </div>
  </div>
</template>
