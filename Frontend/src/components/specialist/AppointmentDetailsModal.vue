<script setup lang="ts">
export interface AppointmentDetailsDTO {
  appointmentId: number;
  referralId: number;
  appointmentDate: string;
  appointmentTime: string;
  appointmentStatus: string;
  patientName: string;
  mrn: string;
  specialistName: string;
  referralReason: string;
}

defineProps<{
  appointment: AppointmentDetailsDTO;
}>();

defineEmits<{
  close: [];
}>();
</script>

<template>
  <div
    class="fixed inset-0 z-50 flex items-start justify-center overflow-y-auto bg-slate-900/40 p-8"
    @click.self="$emit('close')"
  >
    <div class="w-full max-w-4xl">
      <div class="rounded-xl bg-white p-6 shadow-lg">
        <div class="flex items-start justify-between">
          <div>
            <h2 class="text-2xl font-bold text-slate-900">
              Appointment #{{ appointment.appointmentId }}
            </h2>

            ```
            <p class="mt-1 text-sm text-slate-500">
              Referral #{{ appointment.referralId }}
            </p>
          </div>

          <button
            class="rounded-lg p-2 hover:bg-slate-100"
            @click="$emit('close')"
          >
            ✕
          </button>
        </div>
      </div>

      <div class="mt-4 grid grid-cols-2 gap-4">
        <div class="rounded-xl border border-slate-100 bg-white p-6 shadow-lg">
          <h3 class="font-bold text-slate-900">Patient Information</h3>

          <div class="mt-5 space-y-4">
            <div>
              <p class="text-xs font-semibold uppercase text-slate-500">
                Patient Name
              </p>
              <p>{{ appointment.patientName }}</p>
            </div>

            <div>
              <p class="text-xs font-semibold uppercase text-slate-500">MRN</p>
              <p>{{ appointment.mrn }}</p>
            </div>

            <div>
              <p class="text-xs font-semibold uppercase text-slate-500">
                Specialist
              </p>
              <p>{{ appointment.specialistName }}</p>
            </div>
          </div>
        </div>

        <div class="rounded-xl border border-slate-100 bg-white p-6 shadow-lg">
          <h3 class="font-bold text-slate-900">Appointment Details</h3>

          <div class="mt-5 space-y-4">
            <div>
              <p class="text-xs font-semibold uppercase text-slate-500">Date</p>
              <p>{{ appointment.appointmentDate }}</p>
            </div>

            <div>
              <p class="text-xs font-semibold uppercase text-slate-500">Time</p>
              <p>{{ appointment.appointmentTime }}</p>
            </div>

            <div>
              <p class="text-xs font-semibold uppercase text-slate-500">
                Status
              </p>
              <p>{{ appointment.appointmentStatus }}</p>
            </div>

            <div>
              <p class="text-xs font-semibold uppercase text-slate-500">
                Referral Reason
              </p>

              <p class="text-slate-700">
                {{ appointment.referralReason }}
              </p>
            </div>
          </div>
        </div>
      </div>
    </div>
    ```
  </div>
</template>
