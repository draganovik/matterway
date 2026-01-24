<script setup lang="ts">
import { createUser } from "@composables/createUser";
type RegisterFormModel = {
  firstName: string;
  lastName: string;
  birthDate: string; // yyyy-MM-dd
  email: string;
  password: string;
  confirmPassword: string;
};

const router = useRouter();
const isSubmitting = ref(false);
const errorMessage = ref<string | null>(null);
const registerModel = ref<RegisterFormModel>({
  firstName: "",
  lastName: "",
  birthDate: new Date(new Date().setFullYear(new Date().getFullYear() - 18))
    .toISOString()
    .split("T")[0],
  email: "",
  password: "",
  confirmPassword: "",
});

const submitFormRegister = async () => {
  if (registerModel.value.password !== registerModel.value.confirmPassword) {
    errorMessage.value = "Lozinke se ne poklapaju.";
    return;
  }
  errorMessage.value = null;
  isSubmitting.value = true;
  try {
    const response = await createUser(
      registerModel.value.firstName,
      registerModel.value.lastName,
      registerModel.value.birthDate, // already yyyy-MM-dd
      registerModel.value.email,
      registerModel.value.password,
    );
    if (response.ok) {
      router.push("/");
    }
  } finally {
    isSubmitting.value = false;
  }
};

useHead({
  title: "Registracija",
});
definePageMeta({
  middleware: "auth",
  authNoSession: true,
});
</script>

<template>
  <AuthRegisterForm
    v-model="registerModel"
    :loading="isSubmitting"
    :error-message="errorMessage"
    @submit="submitFormRegister"
  />
</template>
