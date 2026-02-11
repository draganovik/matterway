<script setup lang="ts">
import { useIdentityApi } from '~/composables/useIdentityApi'
import type { IdentityRole, SystemUserResponse } from '~/types/identity'
import { useRequestState } from '~/composables/useRequestState'
import { useAuthSession } from '~/composables/useAuthSession'
import { parseNumberOr } from '~/utils/numbers'
import { formatDateTime } from '~/utils/formatters'

type SystemUserForm = {
  email: string
  password: string
}

type RoleFilter = 'all' | 'customers' | 'employees'

const auth = useAuthSession()
const api = useIdentityApi()

const canOperate = computed(() => auth.hasPermission('identity', 'operator'))
const canManage = computed(() =>
  auth.hasPermission('identity', 'administrator')
)
const isLookupMode = computed(() => Boolean(filter.value.trim()))

const listState = useRequestState({ empty: 'No system users found.' })
const detailState = useRequestState()
const saveState = useRequestState()
const removeState = useRequestState()

const systemUsers = ref<SystemUserResponse[]>([])
const filter = ref('')
const roleFilter = ref<RoleFilter>('all')
const pagination = reactive({
  page: 1,
  pageSize: 20,
  totalCount: 0,
  totalPages: 1
})

const roleFilterOptions = [
  { label: 'All', value: 'all' as const },
  { label: 'Customers', value: 'customers' as const },
  { label: 'Employees', value: 'employees' as const }
]

const selectedId = ref<string | null>(null)
const selectedSystemUser = ref<SystemUserResponse | null>(null)
const rolesModalOpen = ref(false)

const form = ref<SystemUserForm>({
  email: '',
  password: ''
})

function applySystemUserToForm(systemUser: SystemUserResponse | null) {
  if (!systemUser) {
    form.value = {
      email: '',
      password: ''
    }
    return
  }

  form.value = {
    email: systemUser.email || '',
    password: ''
  }
}

function clearSelection() {
  selectedId.value = null
  selectedSystemUser.value = null
  rolesModalOpen.value = false
  detailState.error = ''
  applySystemUserToForm(null)
}

function resetMessages() {
  saveState.error = ''
  saveState.success = ''
  removeState.error = ''
  removeState.success = ''
}

async function loadSystemUsers() {
  listState.loading = true
  listState.error = ''

  const lookupId = filter.value.trim()
  if (lookupId) {
    const result = await api.getSystemUserById(lookupId)
    listState.loading = false

    if (!result.ok || !result.data) {
      listState.error = result.error || 'Unable to load system user.'
      systemUsers.value = []
      pagination.page = 1
      pagination.totalCount = 0
      pagination.totalPages = 1
      clearSelection()
      return
    }

    systemUsers.value = [result.data]
    pagination.page = 1
    pagination.totalCount = 1
    pagination.totalPages = 1

    if (selectedId.value && selectedId.value !== result.data.id) {
      clearSelection()
    }
    return
  }

  const result = await api.querySystemUsers({
    page: pagination.page,
    pageSize: pagination.pageSize,
    role: resolveRoleFilter()
  })

  listState.loading = false

  if (!result.ok) {
    listState.error = result.error || 'Unable to load system users.'
    systemUsers.value = []
    pagination.totalCount = 0
    pagination.totalPages = 1
    clearSelection()
    return
  }

  if (result.status === 204 || !result.data) {
    systemUsers.value = []
    pagination.totalCount = 0
    pagination.totalPages = 1
    clearSelection()
    return
  }

  systemUsers.value = result.data.data || []
  pagination.totalCount = parseNumberOr(
    result.data.meta?.totalCount,
    systemUsers.value.length
  )
  pagination.totalPages = Math.max(
    1,
    parseNumberOr(result.data.meta?.totalPages, 1)
  )
  pagination.page = Math.max(
    1,
    parseNumberOr(result.data.meta?.currentPage, pagination.page)
  )
  pagination.pageSize = Math.max(
    1,
    parseNumberOr(result.data.meta?.pageSize, pagination.pageSize)
  )

  if (selectedId.value) {
    const match =
      systemUsers.value.find((item) => item.id === selectedId.value) || null
    if (!match) clearSelection()
  }
}

async function loadSystemUser(systemUserId: string) {
  detailState.loading = true
  detailState.error = ''

  const result = await api.getSystemUserById(systemUserId)
  detailState.loading = false

  if (!result.ok || !result.data) {
    detailState.error = result.error || 'Unable to load system user details.'
    selectedSystemUser.value = null
    applySystemUserToForm(null)
    return
  }

  selectedSystemUser.value = result.data
  applySystemUserToForm(result.data)

  systemUsers.value = systemUsers.value.map((item) =>
    item.id === result.data?.id ? result.data : item
  )
}

function searchSystemUsers() {
  if (!filter.value.trim() && pagination.page !== 1) {
    pagination.page = 1
    return
  }
  void loadSystemUsers()
}

function selectSystemUser(systemUserId: string) {
  selectedId.value = systemUserId
  rolesModalOpen.value = false
  resetMessages()
  void loadSystemUser(systemUserId)
}

const selectedUserLabel = computed(() => {
  if (!selectedSystemUser.value) return 'Selected system user'
  return selectedSystemUser.value.email?.trim() || selectedSystemUser.value.id
})

const selectedCreatedLabel = computed(() =>
  formatDateTime(selectedSystemUser.value?.created)
)

function revealRoles() {
  if (!selectedSystemUser.value?.id) return
  rolesModalOpen.value = true
}

function changePage(page: number) {
  pagination.page = page
}

function changePageSize(pageSize: number) {
  pagination.pageSize = pageSize
  pagination.page = 1
}

function resolveRoleFilter(): IdentityRole | undefined {
  if (roleFilter.value === 'customers') return 'Customer'
  if (roleFilter.value === 'employees') return 'Employee'
  return undefined
}

function changeRoleFilter(value: RoleFilter) {
  roleFilter.value = value
  if (pagination.page !== 1) {
    pagination.page = 1
    return
  }
  void loadSystemUsers()
}

async function saveSystemUser() {
  resetMessages()

  if (!canOperate.value) return

  const systemUserId = selectedId.value
  const email = form.value.email.trim()
  const password = form.value.password.trim()

  if (!systemUserId) {
    saveState.error = 'Select a system user to update.'
    return
  }

  if (!email && !password) {
    saveState.error = 'Provide email and/or password to update.'
    return
  }

  saveState.loading = true
  const result = await api.updateSystemUser(systemUserId, {
    email: email || undefined,
    password: password || undefined
  })
  saveState.loading = false

  if (!result.ok || !result.data) {
    saveState.error = result.error || 'Unable to update system user.'
    return
  }

  const updated = result.data

  selectedId.value = updated.id
  selectedSystemUser.value = updated
  applySystemUserToForm(updated)

  systemUsers.value = systemUsers.value.map((item) =>
    item.id === updated.id ? updated : item
  )

  saveState.success = 'System user updated.'
}

async function removeSystemUser() {
  resetMessages()

  if (!canManage.value) return

  const systemUserId = selectedId.value
  if (!systemUserId) {
    removeState.error = 'Select a system user to delete.'
    return
  }

  removeState.loading = true
  const result = await api.deleteSystemUser(systemUserId)
  removeState.loading = false

  if (!result.ok) {
    removeState.error = result.error || 'Unable to delete system user.'
    return
  }

  systemUsers.value = systemUsers.value.filter(
    (item) => item.id !== systemUserId
  )

  if (!isLookupMode.value) {
    pagination.totalCount = Math.max(0, pagination.totalCount - 1)
    if (systemUsers.value.length === 0 && pagination.page > 1) {
      pagination.page -= 1
    } else {
      void loadSystemUsers()
    }
  } else {
    pagination.totalCount = systemUsers.value.length
    pagination.totalPages = 1
    pagination.page = 1
  }

  selectedId.value = null
  selectedSystemUser.value = null
  applySystemUserToForm(null)

  removeState.success = 'System user deleted.'
}

watch([() => pagination.page, () => pagination.pageSize], () => {
  if (filter.value.trim()) return
  void loadSystemUsers()
})

onMounted(() => {
  void loadSystemUsers()
})
</script>

<template>
  <div class="flex h-full min-h-0 flex-col gap-4 overflow-hidden">
    <div class="flex shrink-0 flex-wrap items-center justify-between gap-3">
      <div>
        <h2 class="text-foreground text-base font-semibold">
          Manage System Users
        </h2>
        <p class="text-muted text-sm">
          Browse users, update credentials, and manage service roles.
        </p>
      </div>

      <div class="flex items-center gap-2">
        <span class="text-muted text-sm">Role</span>
        <USelectMenu
          :items="roleFilterOptions"
          :model-value="roleFilter"
          value-key="value"
          label-key="label"
          class="min-w-36"
          @update:model-value="changeRoleFilter"
        />
      </div>
    </div>

    <EntitiesSplitView
      class="min-h-0 flex-1"
      list-class="overflow-y-auto"
      detail-class="overflow-y-auto"
    >
      <template #list>
        <EntitiesListPanel
          title="System Users"
          description="Filter by role, use pagination, or provide an exact System User ID to fetch one user."
          :items="systemUsers"
          item-key="id"
          item-title-key="email"
          item-subtitle-key="id"
          :selected-id="selectedId"
          :filter="filter"
          filter-input-type="input"
          filter-placeholder="Optional exact System User ID (GUID)."
          :loading="listState.loading"
          :error="listState.error"
          :empty-message="listState.empty"
          :page="pagination.page"
          :page-size="pagination.pageSize"
          :total-count="pagination.totalCount"
          :total-pages="pagination.totalPages"
          @update:filter="(value) => (filter = value)"
          @search="searchSystemUsers"
          @update:page="changePage"
          @update:page-size="changePageSize"
          @select="selectSystemUser"
        >
          <template #item="{ item }">
            <IdentitySystemUsersListItem :item="item" />
          </template>
        </EntitiesListPanel>
      </template>

      <template #detail>
        <div class="space-y-4">
          <div class="space-y-1">
            <h3 class="text-foreground text-base font-semibold">
              {{
                selectedSystemUser ? 'Edit System User' : 'System User Editor'
              }}
            </h3>
            <p class="text-muted text-sm">
              {{
                canOperate
                  ? 'Operator permission is required for updates. Administrator permission is required for role changes and delete.'
                  : 'Read-only mode: operator permission required for updates.'
              }}
            </p>
          </div>

          <StatusMessages
            v-if="detailState.loading || detailState.error"
            :loading="detailState.loading ? 'Loading system user.' : false"
            :error="detailState.error"
          />

          <EntitiesEmptyState
            v-else-if="!selectedSystemUser"
            title="Nothing selected"
            description="Select a system user from the list to start editing."
          />

          <div v-else class="grid gap-4">
            <div class="grid gap-2 text-sm">
              <div class="text-muted">
                System User ID: {{ selectedSystemUser.id }}
              </div>
              <div class="text-muted">
                Primary Role: {{ selectedSystemUser.role }}
              </div>
              <div class="text-muted">Created: {{ selectedCreatedLabel }}</div>
            </div>

            <IdentitySystemUsersBaseForm
              v-model="form"
              :disabled="!canOperate"
            />

            <div class="flex flex-wrap items-center gap-3">
              <UButton
                color="primary"
                :loading="saveState.loading"
                :disabled="!canOperate"
                @click="saveSystemUser"
              >
                Update System User
              </UButton>

              <UButton
                color="error"
                variant="ghost"
                :loading="removeState.loading"
                :disabled="!canManage"
                @click="removeSystemUser"
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

              <UButton variant="outline" @click="revealRoles">
                Reveal Roles
              </UButton>
            </div>

            <StatusMessages
              :error="saveState.error || removeState.error"
              :success="saveState.success || removeState.success"
            />
          </div>
        </div>
      </template>
    </EntitiesSplitView>
  </div>

  <IdentitySystemUsersRevealRolesModal
    v-model:open="rolesModalOpen"
    :system-user-id="selectedSystemUser?.id || null"
    :user-label="selectedUserLabel"
    :can-manage="canManage"
  />
</template>
