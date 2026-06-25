<script setup lang="ts">
import { ref, computed, onMounted } from "vue";
import DashboardLayout from "../../components/layout/DashboardLayout.vue";
import { adminNavLinks } from "../../config/navigation";
import { getUsers } from "../../api/admin";
import type { UserListDTO } from "../../types/admin";

// ✅ User (header)
const user = ref({
  name: "Admin User",
  welcomeName: "Admin",
  role: "Administrator",
  initials: "AD",
});

// ✅ State
const users = ref<UserListDTO[]>([]);
const loading = ref(true);
const searchQuery = ref("");
const selectedRole = ref("All");

// ✅ Fetch users
onMounted(async () => {
  try {
    const res = await getUsers();
    users.value = res.data;
  } catch (err) {
    console.error("Users fetch error:", err);
  } finally {
    loading.value = false;
  }
});

// ✅ Filter (same style as referrals)

const filteredUsers = computed(() => {
  const query = searchQuery.value.trim().toLowerCase();

  return users.value.filter((u) => {
    const matchesSearch =
      !query ||
      u.name.toLowerCase().includes(query) ||
      u.email.toLowerCase().includes(query) ||
      u.role.toLowerCase().includes(query) ||
      u.facility.toLowerCase().includes(query);

    const matchesRole =
      selectedRole.value === "All" || u.role === selectedRole.value;

    return matchesSearch && matchesRole;
  });
});


const roleOptions = computed(() => {
  const roles = users.value.map((u) => u.role);
  return ["All", ...new Set(roles)];
});

</script>

<template>
  <DashboardLayout
    :nav-links="adminNavLinks"
    :user="user"
    title="Users"
    subtitle="Manage system users"
  >
    <div class="space-y-6">

      <!-- ✅ Search -->
      
<div class="bg-white p-4 rounded-xl border border-slate-100 flex gap-4">

  <!-- ✅ Search -->
  <input
    v-model="searchQuery"
    type="text"
    placeholder="Search users..."
    class="flex-1 rounded-xl border border-slate-200 bg-white py-2.5 px-4 text-sm text-slate-900 outline-none focus:border-blue-400"
  />

  <!-- ✅ Role Filter -->
  <select
    v-model="selectedRole"
    class="rounded-xl border border-slate-200 px-3 py-2 text-sm"
  >
    <option v-for="role in roleOptions" :key="role" :value="role">
      {{ role }}
    </option>
  </select>

</div>


      <!-- ✅ Table -->
      <div class="rounded-xl border border-slate-100 bg-white shadow-sm overflow-hidden">
        <table class="w-full">
          <thead>
            <tr class="border-b border-slate-100 bg-slate-50/50">
              <th class="px-6 py-3 text-left text-xs font-semibold uppercase text-slate-500">
                User ID
              </th>

              <th class="px-6 py-3 text-left text-xs font-semibold uppercase text-slate-500">
                Name
              </th>

              <th class="px-6 py-3 text-left text-xs font-semibold uppercase text-slate-500">
                Email
              </th>

              <th class="px-6 py-3 text-left text-xs font-semibold uppercase text-slate-500">
                Role
              </th>

              <th class="px-6 py-3 text-left text-xs font-semibold uppercase text-slate-500">
                Facility
              </th>
            </tr>
          </thead>

          <tbody>
            <!-- ✅ Loading -->
            <tr v-if="loading">
              <td colspan="5" class="px-6 py-6 text-center text-sm text-slate-500">
                Loading users...
              </td>
            </tr>

            <!-- ✅ Data -->
            <tr
              v-for="u in filteredUsers"
              :key="u.userId"
              class="border-b border-slate-100 hover:bg-slate-50"
            >
              <td class="px-6 py-4 text-sm text-blue-600 font-medium">
                #{{ u.userId }}
              </td>

              <td class="px-6 py-4 text-sm font-semibold text-slate-900">
                {{ u.name }}
              </td>

              <td class="px-6 py-4 text-sm text-slate-600">
                {{ u.email }}
              </td>

              <td class="px-6 py-4 text-sm text-slate-600">
                {{ u.role }}
              </td>

              <td class="px-6 py-4 text-sm text-slate-600">
                {{ u.facility }}
              </td>
            </tr>

            <!-- ✅ Empty -->
            <tr v-if="filteredUsers.length === 0 && !loading">
              <td colspan="5" class="px-6 py-8 text-center text-sm text-slate-500">
                No users found.
              </td>
            </tr>
          </tbody>
        </table>
      </div>

    </div>
  </DashboardLayout>
</template>