<script setup lang="ts">
import { useAuthLoginPage } from "~/composables/features/auth/useAuthLoginPage"

definePageMeta({
  title: "Prijava",
  public: true,
})

const { model, loading, error, initialize, submit } = useAuthLoginPage()

await initialize()
</script>

<template>
  <AuthLoginPanel
    v-model="model"
    class="flex min-h-[calc(100dvh-14rem)] items-center"
    :loading="loading"
    :error="error"
    @submit="submit"
  />
</template>
