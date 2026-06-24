<script setup lang="ts">
import { onMounted, ref } from "vue";

import { getFacilitiesDropdown, routeReferral } from "../../api/referral";

import type { FacilityDTO } from "../../types/referral";

import { getErrorMessage } from "../../utils/errorHandler";

const props = defineProps<{
  referral: any;
}>();

const emit = defineEmits<{
  close: [];
  success: [];
}>();

const facilities = ref<FacilityDTO[]>([]);
const selectedFacilities = ref<number[]>([]);

const loadingFacilities = ref(false);
const submitting = ref(false);

const loadFacilities = async () => {
  loadingFacilities.value = true;

  try {
    const response = await getFacilitiesDropdown(props.referral.referralId);

    facilities.value = response.data;
  } catch (error) {
    console.error("Failed to load facilities:", error);

    // Demo facilities from parent page
    facilities.value = props.referral.facilities || [];

    alert(
      `Backend unavailable. Showing demo facilities.\n\n${getErrorMessage(
        error,
      )}`,
    );
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

    alert("Referral routed successfully.");

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
  <div class="fixed inset-0 z-50 flex items-center justify-center bg-black/40">
    <div class="w-full max-w-lg rounded-xl bg-white p-6 shadow-lg">
      <div class="mb-4 flex items-center justify-between">
        <h2 class="text-lg font-semibold">Route Referral</h2>

        <button class="text-slate-500" @click="emit('close')">✕</button>
      </div>

      <div v-if="loadingFacilities">Loading facilities...</div>

      <div
        v-for="facility in facilities"
        :key="facility.facilityId"
        class="flex items-center justify-between rounded-lg border border-slate-200 p-4 mb-2"
      >
        <div class="flex items-center gap-3">
          <input
            v-model="selectedFacilities"
            :value="facility.facilityId"
            type="checkbox"
          />

          <div>
            <p class="font-medium text-slate-900">
              {{ facility.facilityName }}
            </p>

            <p class="text-sm text-slate-500">
              {{ facility.availableSpecialists }} specialists available
            </p>
          </div>
        </div>
      </div>

      <div class="mt-6 flex justify-end gap-3">
        <button class="rounded border px-4 py-2" @click="emit('close')">
          Cancel
        </button>

        <button
          :disabled="submitting"
          class="rounded bg-blue-600 px-4 py-2 text-white"
          @click="submitRouting"
        >
          {{ submitting ? "Routing..." : "Route Referral" }}
        </button>
      </div>
    </div>
  </div>
</template>
