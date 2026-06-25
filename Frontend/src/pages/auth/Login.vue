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

    console.log(response.data.data);
    console.log(response.data.data.roleId);
    console.log(typeof response.data.data.roleId);

    localStorage.setItem("token", response.data.data.token);
    console.log(response.data.data.token);
    localStorage.setItem("roleId", response.data.data.roleId.toString());

    switch (response.data.data.roleId) {
      case 1:
        router.push("/patient");
        break;

      case 2:
        router.push("/coordinator");
        break;

      case 3:
        router.push("/specialist");
        break;

      case 4:
        router.push("/admin");
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
  <div class="min-h-screen bg-slate-50 flex items-center justify-center px-4">
    <div
      class="w-full max-w-md rounded-2xl border border-slate-200 bg-white p-8 shadow-sm"
    >
      <!-- Header -->
      <div class="mb-8 text-center">
        <h1 class="text-3xl font-bold text-slate-900">Referral Manager</h1>

        <p class="mt-2 text-sm text-slate-500">
          Sign in to continue to your dashboard
        </p>
      </div>

      <!-- Form -->
      <form @submit.prevent="handleLogin" class="space-y-5">
        <div>
          <label class="mb-2 block text-sm font-medium text-slate-700">
            Email Address
          </label>

          <input
            v-model="email"
            type="email"
            placeholder="Enter your email"
            class="w-full rounded-xl border border-slate-300 px-4 py-3 text-slate-900 placeholder:text-slate-400 focus:border-blue-500 focus:outline-none focus:ring-4 focus:ring-blue-100"
          />
        </div>

        <div>
          <label class="mb-2 block text-sm font-medium text-slate-700">
            Password
          </label>

          <input
            v-model="password"
            type="password"
            placeholder="Enter your password"
            class="w-full rounded-xl border border-slate-300 px-4 py-3 text-slate-900 placeholder:text-slate-400 focus:border-blue-500 focus:outline-none focus:ring-4 focus:ring-blue-100"
          />
        </div>

        <div
          v-if="errorMessage"
          class="rounded-xl border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-600"
        >
          {{ errorMessage }}
        </div>

        <button
          type="submit"
          :disabled="loading"
          class="w-full rounded-xl bg-blue-600 py-3 font-semibold text-white transition hover:bg-blue-700 disabled:cursor-not-allowed disabled:opacity-50"
        >
          {{ loading ? "Signing In..." : "Sign In" }}
        </button>
      </form>
    </div>
  </div>
</template>
