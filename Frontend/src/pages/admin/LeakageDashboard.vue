<template>
  <div>
    <h2>Leakage Dashboard</h2>

    <div class="grid">
      <StatCard title="In-Network" value="748" />
      <StatCard title="Out-of-Network" value="169" />
      <StatCard title="Leakage Rate" value="18%" />
    </div>
  </div>
</template>

<script setup>
import StatCard from "../../components/admin/StatCard.vue";
</script>

<style scoped>
.grid {
  display: flex;
  gap: 15px;
}
</style>