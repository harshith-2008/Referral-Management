<script setup lang="ts">
import { computed, ref, watch } from "vue";

import type {
  ReferralDTO,
  ReferralDetailDTO,
  SpecialistMatchDTO,
} from "../../types/referral";
import { formatTime, getTodayInputValue } from "../../utils/date";

import { getAvailableSlots, createAppointment } from "../../api/appointment";

const props = defineProps<{
  referral: ReferralDTO;
  details: ReferralDetailDTO;
  specialists: SpecialistMatchDTO[];
}>();

const emit = defineEmits<{
  close: [];
  success: [appointment: {
    specialistName: string;
    appointmentDate: string;
    appointmentTime: string;
  }];
}>();

const errorMessage = ref("");
const slotMessage = ref("");
const loadingSlots = ref(false);
const scheduling = ref(false);

const selectedSpecialistId = ref<number>();
const selectedDate = ref("");
const slots = ref<any[]>([]);
const selectedSlot = ref("");
const minAppointmentDate = getTodayInputValue();

const selectedSpecialist = computed(() =>
  props.specialists.find(
    (specialist) => specialist.specialistId === selectedSpecialistId.value,
  ),
);

watch([selectedSpecialistId, selectedDate], async () => {
  slots.value = [];
  selectedSlot.value = "";
  errorMessage.value = "";
  slotMessage.value = "";

  if (!selectedSpecialistId.value || !selectedDate.value) return;

  loadingSlots.value = true;

  try {
    const response = await getAvailableSlots(
      selectedSpecialistId.value,
      selectedDate.value,
    );

    slots.value = response.data.data?.slots ?? [];

    if (!slots.value.length) {
      slotMessage.value =
        "No appointment slots available for the selected date.";
    }
  } catch (error) {
    console.error(error);

    slots.value = [];

    errorMessage.value =
      "Unable to load appointment slots. Please try another date.";
  } finally {
    loadingSlots.value = false;
  }
});

const scheduleAppointment = async () => {
  errorMessage.value = "";

  if (
    !selectedSpecialistId.value ||
    !selectedDate.value ||
    !selectedSlot.value
  ) {
    errorMessage.value =
      "Please select a specialist, appointment date and time slot.";

    return;
  }

  scheduling.value = true;

  try {
    await createAppointment({
      referralId: props.referral.referralId,
      specialistId: selectedSpecialistId.value,
      appointmentDate: selectedDate.value,
      appointmentTime: selectedSlot.value,
    });

    emit("success", {
      specialistName: selectedSpecialist.value?.specialistName ?? "the specialist",
      appointmentDate: selectedDate.value,
      appointmentTime: selectedSlot.value,
    });
  } catch (error: any) {
    errorMessage.value =
      error?.response?.data?.message ?? "Unable to schedule appointment.";
  } finally {
    scheduling.value = false;
  }
};
</script>

<template>
  <div
    class="fixed inset-0 z-50 flex items-end justify-center sm:items-center bg-black/50 backdrop-blur-sm"
  >
    <div
      class="bg-white w-full sm:max-w-3xl sm:rounded-2xl shadow-2xl overflow-hidden flex flex-col max-h-[92vh]"
    >
      <!-- Header -->
      <div
        class="flex items-center justify-between px-6 py-4 border-b border-slate-100"
      >
        <div>
          <p
            class="text-xs font-medium tracking-widest text-slate-400 uppercase mb-0.5"
          >
            Referral Assignment
          </p>
          <h2 class="text-base font-semibold text-slate-900">
            Schedule Specialist
          </h2>
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

      <!-- Scrollable body -->
      <div class="overflow-y-auto flex-1 px-6 py-5 space-y-6">
        <!-- Patient summary strip -->
        <div
          class="rounded-xl bg-slate-50 border border-slate-100 px-4 py-4 grid grid-cols-2 gap-x-6 gap-y-3"
        >
          <div
            class="col-span-2 flex items-center gap-3 pb-3 mb-1 border-b border-slate-200"
          >
            <div
              class="w-9 h-9 rounded-full bg-blue-100 flex items-center justify-center text-blue-600 font-semibold text-sm shrink-0"
            >
              {{ details.patientName?.charAt(0) }}
            </div>
            <div>
              <p class="font-semibold text-slate-900 text-sm leading-tight">
                {{ details.patientName }}
              </p>
              <p class="text-xs text-slate-400">MRN {{ details.mrn }}</p>
            </div>
          </div>

          <div
            v-for="item in [
              { label: 'Age', value: details.age },
              { label: 'Gender', value: details.gender },
              { label: 'Specialty', value: details.specialty },
              { label: 'Urgency', value: details.urgency },
              { label: 'Status', value: details.status },
              { label: 'Facility', value: details.primaryFacility },
            ]"
            :key="item.label"
            class="min-w-0"
          >
            <p
              class="text-[10px] font-medium tracking-wider text-slate-400 uppercase mb-0.5"
            >
              {{ item.label }}
            </p>
            <p class="text-sm text-slate-800 font-medium truncate">
              {{ item.value }}
            </p>
          </div>

          <div class="col-span-2 pt-2 border-t border-slate-200">
            <p
              class="text-[10px] font-medium tracking-wider text-slate-400 uppercase mb-0.5"
            >
              Diagnosis
            </p>
            <p class="text-sm text-slate-800 font-medium">
              {{ details.diagnosisCode }}
            </p>
          </div>
        </div>

        <!-- No specialists warning -->
        <div
          v-if="!props.specialists.length"
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
          <p class="text-sm text-amber-700">
            No matching specialists found for this referral.
          </p>
        </div>

        <!-- Step 1: Specialist -->
        <div>
          <p
            class="text-xs font-medium tracking-widest text-slate-400 uppercase mb-2"
          >
            1 — Select Specialist
          </p>
          <div class="space-y-2">
            <button
              v-for="s in props.specialists"
              :key="s.specialistId"
              class="w-full text-left flex items-center gap-4 rounded-xl border px-4 py-3 transition-all focus:outline-none focus-visible:ring-2 focus-visible:ring-blue-500"
              :class="
                selectedSpecialistId === s.specialistId
                  ? 'border-blue-500 bg-blue-50 shadow-sm'
                  : 'border-slate-200 bg-white hover:border-slate-300 hover:bg-slate-50'
              "
              @click="selectedSpecialistId = s.specialistId"
            >
              <div
                class="w-8 h-8 rounded-full flex items-center justify-center text-xs font-bold shrink-0 transition-colors"
                :class="
                  selectedSpecialistId === s.specialistId
                    ? 'bg-blue-500 text-white'
                    : 'bg-slate-100 text-slate-500'
                "
              >
                {{ s.specialistName?.charAt(0) }}
              </div>
              <div class="flex-1 min-w-0">
                <p class="text-sm font-medium text-slate-900 truncate">
                  {{ s.specialistName }}
                </p>
                <p class="text-xs text-slate-400 truncate">
                  {{ s.shiftBlock }}
                </p>
              </div>
              <div
                class="w-4 h-4 rounded-full border-2 flex items-center justify-center shrink-0 transition-colors"
                :class="
                  selectedSpecialistId === s.specialistId
                    ? 'border-blue-500'
                    : 'border-slate-300'
                "
              >
                <div
                  v-if="selectedSpecialistId === s.specialistId"
                  class="w-2 h-2 rounded-full bg-blue-500"
                />
              </div>
            </button>
          </div>
        </div>

        <!-- Step 2: Date -->
        <div>
          <p
            class="text-xs font-medium tracking-widest text-slate-400 uppercase mb-2"
          >
            2 — Pick a Date
          </p>
          <input
            v-model="selectedDate"
            type="date"
            :min="minAppointmentDate"
            class="w-full rounded-xl border border-slate-200 bg-white px-4 py-2.5 text-sm text-slate-900 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent transition"
          />
        </div>

        <!-- Step 3: Time Slot -->
        <div
          v-if="loadingSlots"
          class="flex items-center gap-2 text-sm text-slate-400"
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
          Loading available slots…
        </div>

        <div v-else-if="slots.length">
          <p
            class="text-xs font-medium tracking-widest text-slate-400 uppercase mb-2"
          >
            3 — Choose a Time
          </p>
          <div class="grid grid-cols-3 gap-2">
            <button
              v-for="slot in slots"
              :key="slot.startTime"
              class="rounded-xl border px-3 py-2.5 text-sm font-medium transition-all focus:outline-none focus-visible:ring-2 focus-visible:ring-blue-500"
              :class="
                selectedSlot === slot.startTime
                  ? 'bg-blue-500 text-white border-blue-500 shadow-sm'
                  : 'border-slate-200 text-slate-700 hover:border-blue-300 hover:bg-blue-50'
              "
              @click="selectedSlot = slot.startTime"
            >
              {{ formatTime(slot.startTime) }}
            </button>
          </div>
        </div>

        <div v-else-if="slotMessage" class="text-sm text-slate-400 italic">
          {{ slotMessage }}
        </div>

        <!-- Error -->
        <div
          v-if="errorMessage"
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
          <p class="text-sm text-red-600">{{ errorMessage }}</p>
        </div>
      </div>

      <!-- Footer -->
      <div
        class="px-6 py-4 border-t border-slate-100 flex items-center justify-end gap-3"
      >
        <button
          class="px-4 py-2 rounded-xl text-sm font-medium text-slate-600 hover:bg-slate-100 transition-colors"
          @click="emit('close')"
        >
          Cancel
        </button>
        <button
          class="px-5 py-2 rounded-xl text-sm font-semibold text-white bg-blue-600 hover:bg-blue-700 disabled:opacity-50 disabled:cursor-not-allowed transition-colors flex items-center gap-2"
          :disabled="scheduling"
          @click="scheduleAppointment"
        >
          <svg
            v-if="scheduling"
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
          {{ scheduling ? "Scheduling…" : "Confirm Appointment" }}
        </button>
      </div>
    </div>
  </div>
</template>
