<script setup lang="ts">
import { computed, ref } from "vue";

const props = defineProps<{
  referrals: any[];
}>();

const emit = defineEmits<{
  view: [referral: any];
}>();

const search = ref("");

const filteredReferrals = computed(() => {
  const q = search.value.toLowerCase();

  return props.referrals.filter(
    (r) =>
      !q ||
      r.patientName.toLowerCase().includes(q) ||
      r.referralId.toString().includes(q),
  );
});
</script>

<template>
  <div class="rounded-xl border bg-white shadow-sm">
    <div class="p-4 border-b">
      <input
        v-model="search"
        placeholder="Search..."
        class="w-full rounded-lg border px-3 py-2"
      />
    </div>

    <table class="w-full">
      <thead>
        <tr class="bg-slate-50">
          <th class="p-3 text-left">Referral ID</th>
          <th class="p-3 text-left">Patient</th>
          <th class="p-3 text-left">Origin</th>
          <th class="p-3 text-left">Destination</th>
          <th class="p-3 text-left">Specialty</th>
          <th class="p-3 text-left">Urgency</th>
          <th class="p-3 text-left">Action</th>
        </tr>
      </thead>

      <tbody>
        <tr
          v-for="referral in filteredReferrals"
          :key="referral.referralId"
          class="border-t"
        >
          <td class="p-3">#{{ referral.referralId }}</td>
          <td class="p-3">{{ referral.patientName }}</td>
          <td class="p-3">{{ referral.originFacility }}</td>
          <td class="p-3">{{ referral.destinationFacility }}</td>
          <td class="p-3">{{ referral.specialty }}</td>
          <td class="p-3">{{ referral.urgency }}</td>

          <td class="p-3">
            <button
              class="rounded bg-blue-600 px-3 py-1 text-white"
              @click="emit('view', referral)"
            >
              View
            </button>
          </td>
        </tr>
      </tbody>
    </table>
  </div>
</template>
