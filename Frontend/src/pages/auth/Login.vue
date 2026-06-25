<script setup lang="ts">
import { ref } from "vue";
import { useRouter } from "vue-router";
import { login } from "../../api/authApi";

const router = useRouter();

const email = ref("");
const password = ref("");
const showPassword = ref(false);

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
  <div class="min-h-screen bg-[#0f1117] flex items-center justify-center px-4">
    <div class="w-full max-w-sm">
      <!-- Logo -->
      <div class="flex flex-col items-center mb-8">
        <div
          class="flex h-11 w-11 items-center justify-center rounded-2xl bg-blue-600 mb-4"
        >
          <svg
            viewBox="0 0 24 24"
            fill="none"
            class="h-5 w-5 text-white"
            stroke="currentColor"
            stroke-width="2"
          >
            <path
              stroke-linecap="round"
              stroke-linejoin="round"
              d="M9 12h6m-3-3v6M3 12a9 9 0 1118 0 9 9 0 01-18 0z"
            />
          </svg>
        </div>
        <h1 class="text-[18px] font-semibold text-white">ReferralHub</h1>
        <p class="text-[12px] text-slate-500 mt-1">Sign in to your account</p>
      </div>

      <!-- Card -->
      <div
        class="rounded-2xl bg-white/5 border border-white/8 px-7 py-7 backdrop-blur-sm"
      >
        <form @submit.prevent="handleLogin" class="space-y-4">
          <!-- Email -->
          <div>
            <label class="block text-[12px] font-medium text-slate-400 mb-1.5"
              >Email address</label
            >
            <input
              v-model="email"
              type="email"
              placeholder="you@example.com"
              autocomplete="email"
              class="w-full rounded-xl bg-white/5 border border-white/10 px-4 py-2.5 text-[13px] text-white placeholder:text-slate-600 focus:outline-none focus:border-blue-500 focus:ring-2 focus:ring-blue-500/20 transition-all"
            />
          </div>

          <!-- Password -->
          <div>
            <label class="block text-[12px] font-medium text-slate-400 mb-1.5"
              >Password</label
            >
            <div class="relative">
              <input
                v-model="password"
                :type="showPassword ? 'text' : 'password'"
                placeholder="••••••••"
                autocomplete="current-password"
                class="w-full rounded-xl bg-white/5 border border-white/10 px-4 py-2.5 pr-11 text-[13px] text-white placeholder:text-slate-600 focus:outline-none focus:border-blue-500 focus:ring-2 focus:ring-blue-500/20 transition-all"
              />
              <button
                type="button"
                class="absolute right-3 top-1/2 -translate-y-1/2 text-slate-500 hover:text-slate-300 transition-colors"
                @click="showPassword = !showPassword"
              >
                <!-- Eye open -->
                <svg
                  v-if="!showPassword"
                  viewBox="0 0 24 24"
                  fill="none"
                  class="h-4 w-4"
                  stroke="currentColor"
                  stroke-width="2"
                >
                  <path
                    stroke-linecap="round"
                    stroke-linejoin="round"
                    d="M15 12a3 3 0 11-6 0 3 3 0 016 0z"
                  />
                  <path
                    stroke-linecap="round"
                    stroke-linejoin="round"
                    d="M2.458 12C3.732 7.943 7.523 5 12 5c4.477 0 8.268 2.943 9.542 7-1.274 4.057-5.065 7-9.542 7-4.477 0-8.268-2.943-9.542-7z"
                  />
                </svg>
                <!-- Eye off -->
                <svg
                  v-else
                  viewBox="0 0 24 24"
                  fill="none"
                  class="h-4 w-4"
                  stroke="currentColor"
                  stroke-width="2"
                >
                  <path
                    stroke-linecap="round"
                    stroke-linejoin="round"
                    d="M13.875 18.825A10.05 10.05 0 0112 19c-4.477 0-8.268-2.943-9.542-7a9.97 9.97 0 012.12-3.364M6.343 6.343A9.956 9.956 0 0112 5c4.477 0 8.268 2.943 9.542 7a9.97 9.97 0 01-1.406 2.653M6.343 6.343L3 3m3.343 3.343l11.314 11.314M9.88 9.88a3 3 0 004.243 4.243M9.88 9.88L3 3m6.88 6.88l4.242 4.242"
                  />
                </svg>
              </button>
            </div>
          </div>

          <!-- Error -->
          <div
            v-if="errorMessage"
            class="flex items-center gap-2.5 rounded-xl border border-red-500/20 bg-red-500/10 px-4 py-3"
          >
            <svg
              xmlns="http://www.w3.org/2000/svg"
              class="h-4 w-4 text-red-400 shrink-0"
              fill="none"
              viewBox="0 0 24 24"
              stroke="currentColor"
              stroke-width="2"
            >
              <circle cx="12" cy="12" r="10" />
              <line x1="12" y1="8" x2="12" y2="12" />
              <line x1="12" y1="16" x2="12.01" y2="16" />
            </svg>
            <p class="text-[12px] text-red-400">{{ errorMessage }}</p>
          </div>

          <!-- Submit -->
          <button
            type="submit"
            :disabled="loading"
            class="w-full rounded-xl bg-blue-600 py-2.5 text-[13px] font-semibold text-white hover:bg-blue-500 disabled:opacity-50 disabled:cursor-not-allowed transition-all flex items-center justify-center gap-2 mt-2"
          >
            <svg
              v-if="loading"
              class="h-4 w-4 animate-spin"
              viewBox="0 0 24 24"
              fill="none"
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
            {{ loading ? "Signing in…" : "Sign in" }}
          </button>
        </form>
      </div>

      <p class="text-center text-[11px] text-slate-600 mt-6">
        ReferralHub · Care Coordination Platform
      </p>
    </div>
  </div>
</template>
