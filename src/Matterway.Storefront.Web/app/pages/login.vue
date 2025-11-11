<script lang="ts" setup>
import LoginModel from "~/models/LoginModel";
import { useSessionStore } from "~/stores/session";

useHead({
  title: "Prijava",
});

definePageMeta({
  middleware: "auth",
  authNoSession: true,
});

const loginModel = ref(new LoginModel());
const sessionStore = useSessionStore();
const isSubmitting = ref(false);

const submitFormLogin = async () => {
  if (sessionStore.session) {
    return navigateTo("/");
  }
  try {
    isSubmitting.value = true;
    if (loginModel.value.validate()) {
      await sessionStore.login(loginModel.value);
    }
  } finally {
    isSubmitting.value = false;
    if (sessionStore.isLoggedIn) {
      navigateTo("/");
    }
  }
};
</script>

<template>
  <AuthLoginForm
    v-model="loginModel"
    :loading="isSubmitting"
    @submit="submitFormLogin"
  />
</template>
