<script setup lang="ts">
import { ref, watch } from "vue";

import { getAvailableSlots, createAppointment } from "../../api/appointment";

const props = defineProps<{
  referral: any;
}>();

const emit = defineEmits<{
  close: [];
}>();

const specialists = ref([
  {
    specialistId: 1,
    specialistName: "Dr. Sarah Williams",
    shiftBlock: "Morning",
  },
  {
    specialistId: 2,
    specialistName: "Dr. James Carter",
    shiftBlock: "Evening",
  },
]);

const selectedSpecialistId = ref<number>();
const selectedDate = ref("");
const slots = ref<any[]>([]);
const selectedSlot = ref("");

watch([selectedSpecialistId, selectedDate], async () => {
  if (!selectedSpecialistId.value || !selectedDate.value) return;

  try {
    const response = await getAvailableSlots(
      selectedSpecialistId.value,
      selectedDate.value,
    );

    slots.value = response.data.data.slots;
  } catch {
    slots.value = [
      {
        startTime: "09:00",
        endTime: "09:30",
      },
      {
        startTime: "10:00",
        endTime: "10:30",
      },
      {
        startTime: "11:00",
        endTime: "11:30",
      },
    ];
  }
});

const scheduleAppointment = async () => {
  if (
    !selectedSpecialistId.value ||
    !selectedDate.value ||
    !selectedSlot.value
  ) {
    alert("Select specialist and slot");
    return;
  }

  try {
    await createAppointment({
      referralId: props.referral.referralId,
      specialistId: selectedSpecialistId.value,
      appointmentDate: selectedDate.value,
      appointmentTime: selectedSlot.value,
    });

    alert("Appointment scheduled");
    emit("close");
  } catch {
    alert("Mock Appointment Scheduled");
    emit("close");
  }
};
</script>

<template>
  <div class="fixed inset-0 bg-black/40 flex items-center justify-center z-50">
    <div class="bg-white rounded-xl w-full max-w-2xl p-6">
      <div class="flex justify-between mb-4">
        <h2 class="text-lg font-semibold">Assign Specialist</h2>

        <button @click="emit('close')">✕</button>
      </div>

      <div class="space-y-4">
        <div>
          <label class="block mb-2"> Specialist </label>

          <div class="space-y-2">
            <div
              v-for="s in specialists"
              :key="s.specialistId"
              class="border rounded p-3 cursor-pointer"
              :class="{
                'border-blue-600 bg-blue-50':
                  selectedSpecialistId === s.specialistId,
              }"
              @click="selectedSpecialistId = s.specialistId"
            >
              <div>{{ s.specialistName }}</div>

              <div class="text-sm text-slate-500">
                {{ s.shiftBlock }}
              </div>
            </div>
          </div>
        </div>

        <div>
          <label>Date</label>

          <input
            v-model="selectedDate"
            type="date"
            class="w-full border rounded p-2"
          />
        </div>

        <div v-if="slots.length">
          <label class="block mb-2"> Available Slots </label>

          <div class="grid grid-cols-3 gap-2">
            <button
              v-for="slot in slots"
              :key="slot.startTime"
              class="border rounded p-2"
              :class="{
                'bg-blue-600 text-white': selectedSlot === slot.startTime,
              }"
              @click="selectedSlot = slot.startTime"
            >
              {{ slot.startTime }}
            </button>
          </div>
        </div>

        <div class="flex justify-end gap-2 pt-4">
          <button class="border px-4 py-2 rounded" @click="emit('close')">
            Cancel
          </button>

          <button
            class="bg-blue-600 text-white px-4 py-2 rounded"
            @click="scheduleAppointment"
          >
            Schedule Appointment
          </button>
        </div>
      </div>
    </div>
  </div>
</template>
