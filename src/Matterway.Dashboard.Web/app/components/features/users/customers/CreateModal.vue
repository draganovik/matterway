<script setup lang="ts">
import { useCustomersClient } from "~/composables/api/useCustomersClient"
import type { CustomerResponse } from "~/types/customers"
import { useRequestState } from "~/composables/workflows/state/useRequestState"
import { useResetOnModalOpen } from "~/composables/workflows/modal/useResetOnModalOpen"

type CustomerCreateForm = {
  systemUserId: string
  firstName: string
  lastName: string
  birthDate: string
  defaultAddressId: string
}

const { canEdit = false } = defineProps<{
  canEdit?: boolean
}>()

const emit = defineEmits<{
  created: [customer: CustomerResponse]
}>()

const isOpen = defineModel<boolean>("open", { required: true })

const api = useCustomersClient()
const createState = useRequestState()

const form = ref<CustomerCreateForm>({
  systemUserId: "",
  firstName: "",
  lastName: "",
  birthDate: "",
  defaultAddressId: "",
})

function resetForm() {
  form.value = {
    systemUserId: "",
    firstName: "",
    lastName: "",
    birthDate: "",
    defaultAddressId: "",
  }
  createState.error = ""
}

useResetOnModalOpen(isOpen, resetForm)

async function createCustomer() {
  createState.error = ""
  if (!canEdit) return

  const payload = {
    systemUserId: form.value.systemUserId.trim(),
    firstName: form.value.firstName.trim(),
    lastName: form.value.lastName.trim(),
    birthDate: form.value.birthDate.trim(),
    defaultAddressId: form.value.defaultAddressId.trim() || null,
  }

  if (
    !payload.systemUserId ||
    !payload.firstName ||
    !payload.lastName ||
    !payload.birthDate
  ) {
    createState.error = "Popunite sva obavezna polja pre kreiranja."
    return
  }

  createState.loading = true
  const result = await api.createCustomer(payload)
  createState.loading = false

  if (!result.ok || !result.data) {
    createState.error = result.error || "Kreiranje kupca nije uspelo."
    return
  }

  emit("created", result.data)
  isOpen.value = false
}
</script>

<template>
  <UModal v-model:open="isOpen">
    <template #header>
      <div class="space-y-1">
        <h3 class="text-highlighted text-base font-semibold">Novi kupac</h3>
        <p class="text-muted text-sm">
          Kreirajte profil kupca povezan sa postojećim sistemskim korisnikom.
        </p>
      </div>
    </template>

    <template #body>
      <div class="grid gap-4">
        <UsersCustomersInformationForm
          v-model="form"
          :disabled="!canEdit || createState.loading"
        />
        <StatusMessages :error="createState.error" />
      </div>
    </template>

    <template #footer>
      <div class="flex w-full justify-between">
        <UButton
          variant="ghost"
          :disabled="createState.loading"
          @click="
            () => {
              isOpen = false
            }
          "
        >
          Otkaži
        </UButton>
        <UButton
          color="primary"
          :loading="createState.loading"
          :disabled="!canEdit"
          @click="createCustomer"
        >
          {{ createState.loading ? "Kreiranje kupca" : "Kreiraj kupca" }}
        </UButton>
      </div>
    </template>
  </UModal>
</template>
