<script setup lang="ts">
import { useAuthLoginPage } from "~/composables/features/auth/useAuthLoginPage"

definePageMeta({
  title: "Prijava",
  public: true,
})

const { model, loading, error, initialize, submit } = useAuthLoginPage()

onMounted(() => {
  void initialize()
})
</script>

<template>
  <AuthLoginPanel
    v-model="model"
    :loading="loading"
    :error="error"
    @submit="submit"
  />
</template>
