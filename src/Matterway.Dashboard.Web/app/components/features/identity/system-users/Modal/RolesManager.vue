<script setup lang="ts">
import { useIdentityApi } from "~/composables/useIdentityApi"
import type {
  SystemUserPermLevel,
  SystemUserPermResponse,
} from "~/types/identity"
import { useModalCloseReset } from "~/composables/useModalCloseReset"
import { useRequestState } from "~/composables/useRequestState"

type RoleForm = {
  service: string
  level: SystemUserPermLevel
}

const props = withDefaults(
  defineProps<{
    systemUserId?: string | null
    userLabel?: string
    canManage?: boolean
  }>(),
  {
    systemUserId: null,
    userLabel: "",
    canManage: false,
  },
)

const isOpen = defineModel<boolean>("open", { required: true })

const api = useIdentityApi()
const loadState = useRequestState()
const saveState = useRequestState()
const removeState = useRequestState()

const roles = ref<SystemUserPermResponse[]>([])
const notFound = ref(false)
const removingKey = ref<string | null>(null)

const serviceOptions = [
  { label: "Identity", value: "identity" },
  { label: "Catalog", value: "catalog" },
  { label: "Customers", value: "customers" },
  { label: "Sales", value: "sales" },
]

const levelOptions: Array<{ label: string; value: SystemUserPermLevel }> = [
  { label: "Observer", value: "Observer" },
  { label: "Operator", value: "Operator" },
  { label: "Administrator", value: "Administrator" },
]

const form = ref<RoleForm>({
  service: serviceOptions[0]?.value || "identity",
  level: "Observer",
})

const displayLabel = computed(
  () => props.userLabel.trim() || "Selected system user",
)

function roleKey(role: SystemUserPermResponse) {
  return `${role.service.toLowerCase()}:${role.level.toLowerCase()}`
}

function normalizeRoles(items: SystemUserPermResponse[]) {
  const map = new Map<string, SystemUserPermResponse>()

  for (const item of items) {
    const service = item.service?.trim().toLowerCase()
    const level = item.level
    if (!service || !level) continue

    map.set(`${service}:${level.toLowerCase()}`, {
      service,
      level,
    })
  }

  return [...map.values()].sort((left, right) => {
    const byService = left.service.localeCompare(right.service)
    if (byService !== 0) return byService
    return left.level.localeCompare(right.level)
  })
}

function clearMessages() {
  saveState.error = ""
  saveState.success = ""
  removeState.error = ""
  removeState.success = ""
}

function resetModalState() {
  loadState.loading = false
  loadState.error = ""
  saveState.loading = false
  saveState.error = ""
  saveState.success = ""
  removeState.loading = false
  removeState.error = ""
  removeState.success = ""
  removingKey.value = null
  roles.value = []
  notFound.value = false
  form.value = {
    service: serviceOptions[0]?.value || "identity",
    level: "Observer",
  }
}

async function loadRoles() {
  const systemUserId = props.systemUserId?.trim()
  if (!systemUserId) {
    loadState.error = "Select a system user first."
    roles.value = []
    notFound.value = false
    return
  }

  loadState.loading = true
  loadState.error = ""
  clearMessages()
  roles.value = []
  notFound.value = false

  const result = await api.getSystemUserPerms(systemUserId)
  loadState.loading = false

  if (!result.ok) {
    if (result.status === 404) {
      notFound.value = true
      return
    }

    loadState.error = result.error || "Unable to reveal roles."
    return
  }

  roles.value = normalizeRoles(result.data || [])
}

async function addRole() {
  clearMessages()
  if (!props.canManage) return

  const systemUserId = props.systemUserId?.trim()
  if (!systemUserId) {
    saveState.error = "Select a system user first."
    return
  }

  const payload = {
    service: form.value.service.trim().toLowerCase(),
    level: form.value.level,
  }

  if (!payload.service) {
    saveState.error = "Select a service."
    return
  }

  saveState.loading = true
  const result = await api.addSystemUserPerm(systemUserId, payload)
  saveState.loading = false

  if (!result.ok) {
    saveState.error = result.error || "Unable to add role."
    return
  }

  roles.value = normalizeRoles(
    result.data || [
      ...roles.value,
      {
        service: payload.service,
        level: payload.level,
      },
    ],
  )

  saveState.success = "Role added."
}

async function removeRole(role: SystemUserPermResponse) {
  clearMessages()
  if (!props.canManage) return

  const systemUserId = props.systemUserId?.trim()
  if (!systemUserId) {
    removeState.error = "Select a system user first."
    return
  }

  const key = roleKey(role)
  removingKey.value = key
  removeState.loading = true

  const result = await api.deleteSystemUserPerm(systemUserId, {
    service: role.service,
    level: role.level,
  })

  removeState.loading = false
  removingKey.value = null

  if (!result.ok) {
    removeState.error = result.error || "Unable to remove role."
    return
  }

  roles.value = normalizeRoles(
    result.data || roles.value.filter((item) => roleKey(item) !== key),
  )

  removeState.success = "Role removed."
}

useModalCloseReset({
  isOpen,
  watchSources: [toRef(props, "systemUserId")],
  onCloseReset: resetModalState,
  onOpen: async () => {
    await loadRoles()
  },
})
</script>

<template>
  <UModal v-model:open="isOpen">
    <template #header>
      <div class="space-y-1">
        <h3 class="text-foreground text-base font-semibold">
          System User Roles
        </h3>
        <p class="text-muted text-sm">Revealed roles for {{ displayLabel }}.</p>
      </div>
    </template>

    <template #body>
      <div class="space-y-4">
        <StatusMessages
          v-if="loadState.loading || loadState.error"
          :loading="loadState.loading ? 'Revealing roles.' : false"
          :error="loadState.error"
        />

        <EntitiesEmptyState
          v-else-if="notFound"
          title="System user not found"
          description="The selected system user could not be loaded."
        />

        <template v-else>
          <EntitiesEmptyState
            v-if="!roles.length"
            title="No roles assigned"
            description="This user has no service permissions assigned yet."
          />

          <div v-else class="space-y-2">
            <div
              v-for="role in roles"
              :key="roleKey(role)"
              class="border-default/70 flex items-center justify-between gap-3 rounded-lg border px-3 py-2"
            >
              <div>
                <p class="text-foreground text-sm font-medium">
                  {{ role.service }}
                </p>
                <p class="text-muted text-xs">
                  {{ role.level }}
                </p>
              </div>

              <UButton
                v-if="canManage"
                color="error"
                variant="ghost"
                size="xs"
                :loading="removeState.loading && removingKey === roleKey(role)"
                @click="removeRole(role)"
              >
                Remove
              </UButton>
            </div>
          </div>

          <div
            v-if="canManage"
            class="border-default/70 space-y-3 rounded-lg border p-3"
          >
            <h4 class="text-foreground text-sm font-semibold">Add Role</h4>

            <div class="grid gap-3 md:grid-cols-[1fr_1fr_auto] md:items-end">
              <UFormField label="Service" required>
                <USelectMenu
                  v-model="form.service"
                  :items="serviceOptions"
                  value-key="value"
                  label-key="label"
                  class="w-full"
                  :disabled="saveState.loading"
                />
              </UFormField>

              <UFormField label="Level" required>
                <USelectMenu
                  v-model="form.level"
                  :items="levelOptions"
                  value-key="value"
                  label-key="label"
                  class="w-full"
                  :disabled="saveState.loading"
                />
              </UFormField>

              <UButton
                color="primary"
                :loading="saveState.loading"
                class="md:mb-0.5"
                @click="addRole"
              >
                Add
              </UButton>
            </div>

            <StatusMessages
              :error="saveState.error || removeState.error"
              :success="saveState.success || removeState.success"
            />
          </div>

          <p v-else class="text-muted text-sm">
            Administrator permission is required to change roles.
          </p>
        </template>
      </div>
    </template>

    <template #footer>
      <div class="flex w-full justify-end">
        <UButton variant="ghost" @click="isOpen = false">Close</UButton>
      </div>
    </template>
  </UModal>
</template>
