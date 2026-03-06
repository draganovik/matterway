<script setup lang="ts">
import { useIdentityClient } from "~/composables/api/useIdentityClient"
import type {
  SystemUserPermLevel,
  SystemUserPermResponse,
} from "~/types/identity"
import type { ServiceSection } from "~/types/services/definitions"
import { useModalCloseReset } from "~/composables/workflows/modal/useModalCloseReset"
import { useRequestState } from "~/composables/workflows/state/useRequestState"
import { permissionServices } from "~/data/serviceRegistry"

type PermissionForm = {
  service: ServiceSection["service"]
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

const api = useIdentityClient()
const loadState = useRequestState()
const saveState = useRequestState()
const resetState = useRequestState()

const permissions = ref<SystemUserPermResponse[]>([])
const notFound = ref(false)
const resettingService = ref<string | null>(null)

const serviceOptions = permissionServices

const setLevelOptions: Array<{ label: string; value: SystemUserPermLevel }> = [
  { label: "Operator", value: "Operator" },
  { label: "Manager", value: "Manager" },
]

const form = ref<PermissionForm>({
  service: (serviceOptions[0]?.value ||
    "identity") as ServiceSection["service"],
  level: "Operator",
})

const displayLabel = computed(
  () => props.userLabel.trim() || "Selected account",
)

function permissionKey(permission: SystemUserPermResponse) {
  return permission.service.toLowerCase()
}

function permissionRank(level: SystemUserPermLevel) {
  if (level === "Manager") return 3
  if (level === "Operator") return 2
  return 1
}

function normalizePermissions(items: SystemUserPermResponse[]) {
  const map = new Map<string, SystemUserPermResponse>()

  for (const item of items) {
    const service = item.service?.trim().toLowerCase()
    const level = item.level
    if (!service || !level) continue

    const existing = map.get(service)
    if (!existing || permissionRank(level) > permissionRank(existing.level)) {
      map.set(service, {
        service,
        level,
      })
    }
  }

  return [...map.values()].sort((left, right) => {
    return left.service.localeCompare(right.service)
  })
}

function clearMessages() {
  saveState.error = ""
  saveState.success = ""
  resetState.error = ""
  resetState.success = ""
}

function resetModalState() {
  loadState.loading = false
  loadState.error = ""
  saveState.loading = false
  saveState.error = ""
  saveState.success = ""
  resetState.loading = false
  resetState.error = ""
  resetState.success = ""
  resettingService.value = null
  permissions.value = []
  notFound.value = false
  form.value = {
    service: (serviceOptions[0]?.value ||
      "identity") as ServiceSection["service"],
    level: "Operator",
  }
}

async function loadPermissions() {
  const systemUserId = props.systemUserId?.trim()
  if (!systemUserId) {
    loadState.error = "Select an account first."
    permissions.value = []
    notFound.value = false
    return
  }

  loadState.loading = true
  loadState.error = ""
  clearMessages()
  permissions.value = []
  notFound.value = false

  const result = await api.getSystemUserPerms(systemUserId)
  loadState.loading = false

  if (!result.ok) {
    if (result.status === 404) {
      notFound.value = true
      return
    }

    loadState.error = result.error || "Unable to reveal permissions."
    return
  }

  permissions.value = normalizePermissions(result.data || [])
}

function applyLocalPermission(service: string, level: SystemUserPermLevel) {
  const normalizedService = service.trim().toLowerCase()
  permissions.value = normalizePermissions([
    ...permissions.value.filter((item) => item.service !== normalizedService),
    {
      service: normalizedService,
      level,
    },
  ])
}

async function setPermission() {
  clearMessages()
  if (!props.canManage) return

  const systemUserId = props.systemUserId?.trim()
  if (!systemUserId) {
    saveState.error = "Select an account first."
    return
  }

  const payload = {
    service: form.value.service,
    level: form.value.level,
  }

  if (!payload.service) {
    saveState.error = "Select a service."
    return
  }

  saveState.loading = true
  const result = await api.patchSystemUserPerm(systemUserId, payload)
  saveState.loading = false

  if (!result.ok) {
    saveState.error = result.error || "Unable to set permission."
    return
  }

  permissions.value = result.data
    ? normalizePermissions(result.data)
    : permissions.value
  if (!result.data) applyLocalPermission(payload.service, payload.level)

  saveState.success = "Permission updated."
}

async function resetPermission(permission: SystemUserPermResponse) {
  clearMessages()
  if (!props.canManage) return
  if (permission.level === "Observer") return

  const systemUserId = props.systemUserId?.trim()
  if (!systemUserId) {
    resetState.error = "Select an account first."
    return
  }

  resettingService.value = permission.service.toLowerCase()
  resetState.loading = true

  const result = await api.patchSystemUserPerm(systemUserId, {
    service: permission.service,
    level: "Observer",
  })

  resetState.loading = false
  resettingService.value = null

  if (!result.ok) {
    resetState.error = result.error || "Unable to reset permission."
    return
  }

  permissions.value = result.data
    ? normalizePermissions(result.data)
    : permissions.value
  if (!result.data) applyLocalPermission(permission.service, "Observer")

  resetState.success = "Permission reset to Observer."
}

useModalCloseReset({
  isOpen,
  watchSources: [toRef(props, "systemUserId")],
  onCloseReset: resetModalState,
  onOpen: async () => {
    await loadPermissions()
  },
})
</script>

<template>
  <UModal v-model:open="isOpen">
    <template #header>
      <div class="space-y-1">
        <h3 class="text-foreground text-base font-semibold">
          Account Permissions
        </h3>
        <p class="text-muted text-sm">
          Revealed permissions for {{ displayLabel }}.
        </p>
      </div>
    </template>

    <template #body>
      <div class="space-y-4">
        <StatusMessages
          v-if="loadState.loading || loadState.error"
          :loading="loadState.loading ? 'Revealing permissions.' : false"
          :error="loadState.error"
        />

        <EntitiesEmptyState
          v-else-if="notFound"
          title="Account not found"
          description="The selected account could not be loaded."
        />

        <template v-else>
          <EntitiesEmptyState
            v-if="!permissions.length"
            title="No permissions found"
            description="This user has no employee service permissions."
          />

          <div v-else class="space-y-2">
            <div
              v-for="permission in permissions"
              :key="permissionKey(permission)"
              class="border-default/70 flex items-center justify-between gap-3 rounded-lg border px-3 py-2"
            >
              <div>
                <p class="text-foreground text-sm font-medium">
                  {{ permission.service }}
                </p>
                <p class="text-muted text-xs">
                  {{ permission.level }}
                </p>
              </div>

              <UButton
                v-if="canManage && permission.level !== 'Observer'"
                color="error"
                variant="ghost"
                size="xs"
                :loading="
                  resetState.loading &&
                  resettingService === permission.service.toLowerCase()
                "
                @click="resetPermission(permission)"
              >
                Reset to Observer
              </UButton>
            </div>
          </div>

          <div
            v-if="canManage"
            class="border-default/70 space-y-3 rounded-lg border p-3"
          >
            <h4 class="text-foreground text-sm font-semibold">
              Set Permission
            </h4>

            <div class="grid gap-3 md:grid-cols-[1fr_1fr_auto] md:items-end">
              <UFormField label="Service" required>
                <USelectMenu
                  v-model="form.service"
                  :items="serviceOptions"
                  value-key="value"
                  label-key="label"
                  placeholder="Select service"
                  class="w-full"
                  :disabled="saveState.loading"
                />
              </UFormField>

              <UFormField label="Permission" required>
                <USelectMenu
                  v-model="form.level"
                  :items="setLevelOptions"
                  value-key="value"
                  label-key="label"
                  placeholder="Select permission"
                  class="w-full"
                  :disabled="saveState.loading"
                />
              </UFormField>

              <UButton
                color="primary"
                :loading="saveState.loading"
                class="md:mb-0.5"
                @click="setPermission"
              >
                Save
              </UButton>
            </div>

            <StatusMessages
              :error="saveState.error || resetState.error"
              :success="saveState.success || resetState.success"
            />
          </div>

          <p v-else class="text-muted text-sm">
            Manager permission is required to change permissions.
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
