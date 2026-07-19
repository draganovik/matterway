<script setup lang="ts">
import { useIdentityClient } from "~/composables/api/useIdentityClient"
import type { SystemUserResponse } from "~/types/identity"
import { useRequestState } from "~/composables/workflows/state/useRequestState"
import { useResetOnModalOpen } from "~/composables/workflows/modal/useResetOnModalOpen"

type CreateEmployeeForm = {
  email: string
  password: string
}

const { canManage = false } = defineProps<{
  canManage?: boolean
}>()

const emit = defineEmits<{
  created: [systemUser: SystemUserResponse]
}>()

const isOpen = defineModel<boolean>("open", { required: true })

const api = useIdentityClient()
const createState = useRequestState()

const form = ref<CreateEmployeeForm>({
  email: "",
  password: "",
})

function resetForm() {
  form.value = {
    email: "",
    password: "",
  }
  createState.error = ""
}

useResetOnModalOpen(isOpen, resetForm)

async function createEmployee() {
  createState.error = ""
  if (!canManage) return

  const email = form.value.email.trim()
  const password = form.value.password.trim()

  if (!email || !password) {
    createState.error = "Imejl i lozinka su obavezni."
    return
  }

  createState.loading = true
  const result = await api.createEmployee({
    email,
    password,
  })
  createState.loading = false

  if (!result.ok || !result.data) {
    createState.error = result.error || "Kreiranje zaposlenog nije uspelo."
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
        <h3 class="text-highlighted text-base font-semibold">Novi zaposleni</h3>
        <p class="text-muted text-sm">
          Ovu radnju može da izvrši samo menadžer. Novi nalog se kreira sa
          ulogom zaposlenog.
        </p>
      </div>
    </template>

    <template #body>
      <div class="grid gap-4">
        <UFormField label="Imejl" required>
          <UInput
            v-model="form.email"
            type="email"
            placeholder="ime.prezime@domen.com"
            :disabled="!canManage || createState.loading"
            class="w-full"
          />
        </UFormField>

        <UFormField label="Lozinka" required>
          <UInput
            v-model="form.password"
            type="password"
            placeholder="Najmanje 6 karaktera"
            :disabled="!canManage || createState.loading"
            class="w-full"
          />
        </UFormField>

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
          :disabled="!canManage"
          @click="createEmployee"
        >
          {{
            createState.loading ? "Kreiranje zaposlenog" : "Kreiraj zaposlenog"
          }}
        </UButton>
      </div>
    </template>
  </UModal>
</template>
