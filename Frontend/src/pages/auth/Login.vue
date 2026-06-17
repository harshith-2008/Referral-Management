<script setup lang="ts">
import { ref } from "vue";
import { useRouter } from "vue-router";
import { login } from "../../api/authApi";

const router = useRouter();

const email = ref("");
const password = ref("");

const loading = ref(false);
const errorMessage = ref("");

const handleLogin = async () => {
  try {
    loading.value = true;
    errorMessage.value = "";

    const response = await login({
      email: email.value,
      password: password.value,
    });

    localStorage.setItem("token", response.data.token);
    console.log(response.data.token);
    localStorage.setItem("roleId", response.data.roleId.toString());

    switch (response.data.roleId) {
      case 1:
        router.push("/admin");
        break;

      case 2:
        router.push("/coordinator");
        break;

      case 3:
        router.push("/patient");
        break;

      case 4:
        router.push("/specialist");
        break;

      default:
        router.push("/login");
        break;
    }
  } catch {
    errorMessage.value = "Invalid email or password.";
  } finally {
    loading.value = false;
  }
};
</script>

<template>
  <div class="min-h-screen bg-gray-100 flex items-center justify-center px-4">
    <div class="w-full max-w-md bg-white rounded-xl shadow-lg p-8">
      <div class="text-center mb-8">
        <h1 class="text-3xl font-bold text-gray-800">Referral Manager</h1>

        <p class="text-gray-500 mt-2">Referral Management System</p>
      </div>

      <form @submit.prevent="handleLogin" class="space-y-5">
        <div>
          <label class="block text-sm font-medium text-gray-700 mb-2">
            Email
          </label>

          <input
            v-model="email"
            type="email"
            placeholder="Enter your email"
            class="w-full border border-gray-300 rounded-lg px-4 py-3 focus:outline-none focus:ring-2 focus:ring-blue-500"
          />
        </div>

        <div>
          <label class="block text-sm font-medium text-gray-700 mb-2">
            Password
          </label>

          <input
            v-model="password"
            type="password"
            placeholder="Enter your password"
            class="w-full border border-gray-300 rounded-lg px-4 py-3 focus:outline-none focus:ring-2 focus:ring-blue-500"
          />
        </div>

        <div v-if="errorMessage" class="text-sm text-red-600">
          {{ errorMessage }}
        </div>

        <button
          type="submit"
          :disabled="loading"
          class="w-full bg-blue-600 text-white py-3 rounded-lg font-semibold hover:bg-blue-700 transition disabled:opacity-50 disabled:cursor-not-allowed"
        >
          {{ loading ? "Signing In..." : "Sign In" }}
        </button>
      </form>
    </div>
  </div>
</template>
