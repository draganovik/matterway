<script setup lang="ts">
import type { PutSelfAddressRequest } from "~/types/customers"

withDefaults(
  defineProps<{
    loading?: boolean
    disabled?: boolean
    error?: string
    info?: string
    success?: string
  }>(),
  {
    loading: false,
    disabled: false,
    error: "",
    info: "",
    success: "",
  },
)

const form = defineModel<PutSelfAddressRequest>({ required: true })

const emit = defineEmits<{
  save: []
}>()
</script>

<template>
  <UCard class="border-default border">
    <template #header>
      <h2 class="text-base font-semibold">Adresa za isporuku</h2>
    </template>

    <form class="space-y-4" @submit.prevent="emit('save')">
      <div class="grid gap-4 sm:grid-cols-2">
        <UFormField label="Država" required>
          <UInput
            :model-value="form.country === 'Serbia' ? 'Srbija' : form.country"
            class="w-full"
            readonly
            disabled
          />
        </UFormField>

        <UFormField label="Grad" required>
          <UInput
            v-model="form.city"
            placeholder="npr. Novi Sad"
            class="w-full"
            :disabled="disabled || loading"
            required
          />
        </UFormField>

        <UFormField label="Poštanski broj" required>
          <UInput
            v-model="form.zipCode"
            placeholder="npr. 21000"
            class="w-full"
            :disabled="disabled || loading"
            required
          />
        </UFormField>

        <UFormField label="Kontakt telefon" required>
          <UInput
            v-model="form.contactPhone"
            placeholder="npr. +381 64 123 4567"
            class="w-full"
            :disabled="disabled || loading"
            required
          />
        </UFormField>
      </div>

      <UFormField label="Ulica i broj" required>
        <UInput
          v-model="form.addressLine1"
          placeholder="npr. Bulevar oslobođenja 15"
          class="w-full"
          :disabled="disabled || loading"
          required
        />
      </UFormField>

      <UFormField label="Stan, sprat ili dodatak" required>
        <UInput
          v-model="form.addressLine2"
          placeholder="npr. Stan 12, 3. sprat"
          class="w-full"
          :disabled="disabled || loading"
          required
        />
      </UFormField>

      <StatusMessages :error="error" :info="info" :success="success" />

      <UButton
        type="submit"
        color="primary"
        :loading="loading"
        :disabled="disabled"
      >
        {{ loading ? "Čuvanje adrese" : "Sačuvaj adresu" }}
      </UButton>
    </form>
  </UCard>
</template>
