<script setup lang="ts">
import type { CheckoutAddress } from "~/types/customers/address"

const address = defineModel<CheckoutAddress>({ required: true })
</script>

<template>
  <UCard
    class="border-default bg-default h-full border"
    :ui="{ body: 'h-full' }"
  >
    <template #header>
      <h2 class="text-lg font-semibold">Dostava</h2>
    </template>

    <div class="flex h-full flex-col gap-4">
      <div class="grid gap-4 sm:grid-cols-2">
        <UFormField label="Ime primaoca" required>
          <UInput
            v-model="address.receiverName"
            placeholder="npr. Petar Petrović"
            class="w-full"
            required
          />
        </UFormField>

        <UFormField label="Telefon">
          <UInput
            v-model="address.contactPhone"
            placeholder="npr. +381 64 123 4567"
            class="w-full"
          />
        </UFormField>

        <UFormField label="Ulica" required>
          <UInput
            v-model="address.street"
            placeholder="npr. Bulevar oslobođenja 15"
            class="w-full"
            required
          />
        </UFormField>

        <UFormField label="Dodatak adrese" required>
          <UInput
            v-model="address.residence"
            placeholder="npr. Stan 12, 3. sprat"
            class="w-full"
            required
          />
        </UFormField>

        <UFormField label="Grad" required>
          <UInput
            v-model="address.city"
            placeholder="npr. Novi Sad"
            class="w-full"
            required
          />
        </UFormField>

        <UFormField label="Poštanski broj" required>
          <UInput
            v-model="address.zipCode"
            placeholder="npr. 21000"
            class="w-full"
            required
          />
        </UFormField>

        <UFormField label="Država" class="sm:col-span-2">
          <UInput
            v-model="address.country"
            placeholder="npr. Srbija"
            class="w-full"
          />
        </UFormField>
      </div>

      <UFormField label="Napomena" class="flex min-h-0 flex-1 flex-col">
        <UTextarea
          v-model="address.note"
          placeholder="Napomena za dostavu (opciono)"
          class="w-full"
          :ui="{ root: 'h-full', base: 'h-full min-h-32 resize-none' }"
        />
      </UFormField>
    </div>
  </UCard>
</template>
