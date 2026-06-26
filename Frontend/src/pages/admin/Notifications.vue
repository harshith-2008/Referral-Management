<template>
  <div>
    <h2>Notifications</h2>

    <div v-for="(n, i) in notifications" :key="i">
      {{ n }}
    </div>
  </div>
</template>

<script setup>
const notifications = [
  "New referral created",
  "Referral accepted",
  "Specialist assigned"
];
</script>
