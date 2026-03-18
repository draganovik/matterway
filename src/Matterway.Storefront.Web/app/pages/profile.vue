<script setup lang="ts">
import { useProfilePage } from "~/composables/features/profile/useProfilePage"

definePageMeta({
  title: "Moj profil",
})

const {
  isLoading,
  profileSaving,
  addressSaving,
  profileForm,
  addressForm,
  profileError,
  profileSuccess,
  addressError,
  addressSuccess,
  addressInfo,
  loadData,
  saveProfile,
  saveAddress,
  initialize,
} = useProfilePage()

await initialize()
</script>

<template>
  <div class="space-y-6">
    <UCard class="border-default border">
      <div class="flex flex-wrap items-center justify-between gap-3">
        <div>
          <p class="text-primary text-xs tracking-[0.3em] uppercase">Profil</p>
          <h1 class="text-2xl font-semibold">Moji podaci</h1>
          <p class="text-muted text-sm">
            Ažurirajte lične podatke i adresu za isporuku.
          </p>
        </div>

        <UButton
          color="neutral"
          variant="soft"
          icon="i-lucide-refresh-cw"
          :loading="isLoading"
          @click="loadData"
        >
          {{ isLoading ? "Osvežavanje" : "Osveži" }}
        </UButton>
      </div>
    </UCard>

    <div v-if="isLoading" class="grid gap-6 lg:grid-cols-2">
      <USkeleton class="h-96" />
      <USkeleton class="h-96" />
    </div>

    <div v-else class="grid gap-6 lg:grid-cols-2">
      <ProfileIdentityPanel
        v-model="profileForm"
        :loading="profileSaving"
        :disabled="isLoading"
        :error="profileError"
        :success="profileSuccess"
        @save="saveProfile"
      />

      <ProfileAddressPanel
        v-model="addressForm"
        :loading="addressSaving"
        :disabled="isLoading"
        :error="addressError"
        :info="addressInfo"
        :success="addressSuccess"
        @save="saveAddress"
      />
    </div>
  </div>
</template>
