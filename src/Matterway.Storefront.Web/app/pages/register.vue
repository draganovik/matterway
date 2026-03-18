<script setup lang="ts">
import { useAuthRegisterPage } from "~/composables/features/auth/useAuthRegisterPage"

definePageMeta({
  title: "Registracija",
  public: true,
})

const { model, loading, error, submit } = useAuthRegisterPage()
</script>

<template>
  <AuthRegisterPanel
    v-model="model"
    class="flex min-h-[calc(100dvh-14rem)] items-center"
    :loading="loading"
    :error="error"
    @submit="submit"
  />
</template>
