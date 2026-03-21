<script setup lang="ts">
import type { SystemUserResponse } from "~/types/identity"
import { formatDateTime } from "~/utils/formatters"
import { formatIdentityRole } from "~/utils/labels"

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
    saveError?: string
    saveSuccess?: string
  }>(),
  {
    systemUser: null,
    error: "",
    canOperate: false,
    canManage: false,
    saveLoading: false,
    saveError: "",
    saveSuccess: "",
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
        {{ systemUser ? "Izmena naloga" : "Uređivanje naloga" }}
      </h3>
      <p class="text-muted text-sm">
        {{
          canOperate
            ? "Za izmene je potrebna dozvola operatera. Za promenu dozvola i brisanje potrebna je dozvola menadžera."
            : "Režim samo za čitanje: za izmene je potrebna dozvola operatera."
        }}
      </p>
    </div>

    <StatusMessages v-if="error" :error="error" />

    <EntitiesEmptyState
      v-else-if="!systemUser"
      title="Ništa nije izabrano"
      description="Izaberite nalog sa liste da biste započeli izmenu."
    />

    <div v-else class="grid gap-4">
      <div class="grid gap-2 text-sm">
        <div class="text-muted">ID naloga: {{ systemUser.id }}</div>
        <div class="text-muted">
          Primarna uloga: {{ formatIdentityRole(systemUser.role) }}
        </div>
        <div class="text-muted">Kreiran: {{ createdLabel }}</div>
      </div>

      <UsersAccountsInformationForm v-model="form" :disabled="!canOperate" />

      <div class="flex flex-wrap items-center gap-3">
        <UButton
          color="primary"
          :loading="saveLoading"
          :disabled="!canOperate"
          @click="emit('save')"
        >
          {{ saveLoading ? "Čuvanje naloga" : "Sačuvaj nalog" }}
        </UButton>

        <UButton
          color="error"
          variant="ghost"
          :disabled="!canManage"
          @click="emit('remove')"
        >
          Obriši nalog
        </UButton>
      </div>

      <div
        class="border-default/70 flex flex-wrap items-start justify-between gap-3 rounded-lg border p-3"
      >
        <div class="space-y-1">
          <h4 class="text-foreground text-sm font-semibold">Dozvole</h4>
          <p class="text-muted text-sm">
            Pregledajte dodeljene dozvole po servisima i upravljajte njima ako
            ste menadžer.
          </p>
        </div>

        <UButton variant="outline" @click="emit('revealRoles')">
          Otvori dozvole
        </UButton>
      </div>

      <StatusMessages :error="saveError" :success="saveSuccess" />
    </div>
  </div>
</template>
