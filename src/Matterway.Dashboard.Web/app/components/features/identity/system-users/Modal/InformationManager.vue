<script setup lang="ts">
import { useIdentityApi } from "~/composables/useIdentityApi"
import type { SystemUserResponse } from "~/types/identity"
import { useRequestState } from "~/composables/useRequestState"
import { useResetOnModalOpen } from "~/composables/useResetOnModalOpen"

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

const api = useIdentityApi()
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
    createState.error = "Email and password are required."
    return
  }

  createState.loading = true
  const result = await api.createEmployee({
    email,
    password,
  })
  createState.loading = false

  if (!result.ok || !result.data) {
    createState.error = result.error || "Unable to create employee."
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
        <h3 class="text-foreground text-base font-semibold">
          Create Employee User
        </h3>
        <p class="text-muted text-sm">
          Manager-only action. New user is created with Employee role.
        </p>
      </div>
    </template>

    <template #body>
      <div class="grid gap-4">
        <UFormField label="Email" required>
          <UInput
            v-model="form.email"
            type="email"
            placeholder="name@matterway.local"
            :disabled="!canManage || createState.loading"
            class="w-full"
          />
        </UFormField>

        <UFormField label="Password" required>
          <UInput
            v-model="form.password"
            type="password"
            placeholder="At least 6 characters"
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
          @click="isOpen = false"
        >
          Cancel
        </UButton>
        <UButton
          color="primary"
          :loading="createState.loading"
          :disabled="!canManage"
          @click="createEmployee"
        >
          Create Employee
        </UButton>
      </div>
    </template>
  </UModal>
</template>
