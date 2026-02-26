<script setup lang="ts">
import type { SystemUserResponse } from "~/types/identity"
import { formatDateTime } from "~/utils/formatters"

type SystemUserForm = {
  email: string
  password: string
}

const form = defineModel<SystemUserForm>({ required: true })

const props = withDefaults(
  defineProps<{
    systemUser?: SystemUserResponse | null
    error?: string
    canOperate?: boolean
    canManage?: boolean
    saveLoading?: boolean
    removeLoading?: boolean
    saveError?: string
    removeError?: string
    saveSuccess?: string
    removeSuccess?: string
  }>(),
  {
    systemUser: null,
    error: "",
    canOperate: false,
    canManage: false,
    saveLoading: false,
    removeLoading: false,
    saveError: "",
    removeError: "",
    saveSuccess: "",
    removeSuccess: "",
  },
)

const emit = defineEmits<{
  (event: "save" | "remove" | "revealRoles"): void
}>()

const createdLabel = computed(() => formatDateTime(props.systemUser?.created))
</script>

<template>
  <div class="space-y-4">
    <div class="space-y-1">
      <h3 class="text-foreground text-base font-semibold">
        {{ systemUser ? "Edit System User" : "System User Editor" }}
      </h3>
      <p class="text-muted text-sm">
        {{
          canOperate
            ? "Operator permission is required for updates. Administrator permission is required for role changes and delete."
            : "Read-only mode: operator permission required for updates."
        }}
      </p>
    </div>

    <StatusMessages v-if="error" :error="error" />

    <EntitiesEmptyState
      v-else-if="!systemUser"
      title="Nothing selected"
      description="Select a system user from the list to start editing."
    />

    <div v-else class="grid gap-4">
      <div class="grid gap-2 text-sm">
        <div class="text-muted">System User ID: {{ systemUser.id }}</div>
        <div class="text-muted">Primary Role: {{ systemUser.role }}</div>
        <div class="text-muted">Created: {{ createdLabel }}</div>
      </div>

      <IdentitySystemUsersPanelInformationForm
        v-model="form"
        :disabled="!canOperate"
      />

      <div class="flex flex-wrap items-center gap-3">
        <UButton
          color="primary"
          :loading="saveLoading"
          :disabled="!canOperate"
          @click="emit('save')"
        >
          Update System User
        </UButton>

        <UButton
          color="error"
          variant="ghost"
          :loading="removeLoading"
          :disabled="!canManage"
          @click="emit('remove')"
        >
          Delete System User
        </UButton>
      </div>

      <div
        class="border-default/70 flex flex-wrap items-start justify-between gap-3 rounded-lg border p-3"
      >
        <div class="space-y-1">
          <h4 class="text-foreground text-sm font-semibold">Roles</h4>
          <p class="text-muted text-sm">
            Reveal assigned service roles and manage them as admin.
          </p>
        </div>

        <UButton variant="outline" @click="emit('revealRoles')">
          Reveal Roles
        </UButton>
      </div>

      <StatusMessages
        :error="saveError || removeError"
        :success="saveSuccess || removeSuccess"
      />
    </div>
  </div>
</template>
