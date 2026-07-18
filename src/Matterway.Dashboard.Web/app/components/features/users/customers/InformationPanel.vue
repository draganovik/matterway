<script setup lang="ts">
import type { CustomerResponse } from "~/types/customers"

type CustomerForm = {
  firstName: string
  lastName: string
  birthDate: string
  defaultAddressId: string
}

const form = defineModel<CustomerForm>({ required: true })

withDefaults(
  defineProps<{
    customer?: CustomerResponse | null
    error?: string
    canEdit?: boolean
    saveLoading?: boolean
    saveError?: string
    saveSuccess?: string
  }>(),
  {
    customer: null,
    error: "",
    canEdit: false,
    saveLoading: false,
    saveError: "",
    saveSuccess: "",
  },
)

const emit = defineEmits<{
  (event: "save" | "remove" | "revealAddress"): void
}>()
</script>

<template>
  <div class="space-y-4">
    <div class="space-y-1">
      <h3 class="text-highlighted text-base font-semibold">
        {{ customer ? "Izmena kupca" : "Uređivanje kupca" }}
      </h3>
      <p class="text-muted text-sm">
        {{
          canEdit
            ? "Za kreiranje, izmenu i brisanje potrebna je dozvola operatera."
            : "Režim samo za čitanje: za izmene je potrebna dozvola operatera."
        }}
      </p>
    </div>

    <StatusMessages v-if="error" :error="error" />

    <EntitiesEmptyState
      v-else-if="!customer"
      title="Ništa nije izabrano"
      description="Izaberite kupca sa liste da biste započeli izmenu."
    />

    <div v-else class="grid gap-4">
      <div class="text-muted text-sm">
        ID sistemskog korisnika: {{ customer.systemUserId }}
      </div>

      <UsersCustomersInformationForm
        v-model="form"
        :disabled="!canEdit"
        :show-system-user-id="false"
      />

      <div class="flex flex-wrap items-center gap-3">
        <UButton
          color="primary"
          :loading="saveLoading"
          :disabled="!canEdit"
          @click="emit('save')"
        >
          {{ saveLoading ? "Čuvanje kupca" : "Sačuvaj kupca" }}
        </UButton>

        <UButton
          color="error"
          variant="ghost"
          :disabled="!canEdit"
          @click="emit('remove')"
        >
          Obriši kupca
        </UButton>
      </div>

      <div
        class="border-default/70 flex flex-wrap items-start justify-between gap-3 rounded-md border p-3"
      >
        <div class="space-y-1">
          <h4 class="text-highlighted text-sm font-semibold">Adresa</h4>
          <p class="text-muted text-sm">
            Otvorite i upravljajte adresom izabranog kupca.
          </p>
        </div>

        <UButton variant="outline" @click="emit('revealAddress')">
          Otvori adresu
        </UButton>
      </div>

      <StatusMessages :error="saveError" :success="saveSuccess" />
    </div>
  </div>
</template>
