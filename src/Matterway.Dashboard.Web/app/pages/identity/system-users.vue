<script setup lang="ts">
import { useApiClient } from '~/composables/useApiClient'
import { useFeatureTabs } from '~/composables/useFeatureTabs'
import { formatDateTime } from '~/utils/format'

definePageMeta({
  title: 'System Users',
  service: 'identity',
  level: 'operator'
})

const api = useApiClient()
const { tabs, active } = useFeatureTabs()

const queryForm = reactive({
  page: 1,
  pageSize: 10
})
const queryState = reactive({ loading: false, error: '', empty: '' })
const queryResults = ref<any[]>([])

const lookupForm = reactive({
  userId: ''
})
const lookupState = reactive({ loading: false, error: '' })
const lookupResult = ref<any | null>(null)

const updateForm = reactive({
  userId: '',
  email: '',
  password: ''
})
const updateState = reactive({ loading: false, error: '', success: '' })

const deleteForm = reactive({
  userId: ''
})
const deleteState = reactive({ loading: false, error: '', success: '' })

async function queryUsers() {
  queryState.loading = true
  queryState.error = ''
  queryState.empty = ''
  queryResults.value = []
  const params = new URLSearchParams({
    page: queryForm.page.toString(),
    pageSize: queryForm.pageSize.toString()
  })
  const result = await api.request<any>('identity', `admin/system-users?${params.toString()}`)
  queryState.loading = false
  if (!result.ok) {
    queryState.error = result.error || 'Failed to query system users.'
    return
  }
  queryResults.value = result.data?.data ?? []
  if (!queryResults.value.length) queryState.empty = 'No system users found.'
}

async function lookupUser() {
  lookupState.loading = true
  lookupState.error = ''
  lookupResult.value = null
  const result = await api.request<any>('identity', `admin/system-users/${lookupForm.userId}`)
  lookupState.loading = false
  if (!result.ok) {
    lookupState.error = result.error || 'User not found.'
    return
  }
  lookupResult.value = result.data
}

async function updateUser() {
  updateState.loading = true
  updateState.error = ''
  updateState.success = ''
  const payload: Record<string, any> = {}
  if (updateForm.email) payload.email = updateForm.email
  if (updateForm.password) payload.password = updateForm.password

  const result = await api.request<any>(
    'identity',
    `admin/system-users/${updateForm.userId}`,
    {
      method: 'PATCH',
      body: JSON.stringify(payload)
    }
  )
  updateState.loading = false
  if (!result.ok) {
    updateState.error = result.error || 'Failed to update user.'
    return
  }
  updateState.success = 'User updated.'
}

async function deleteUser() {
  deleteState.loading = true
  deleteState.error = ''
  deleteState.success = ''
  const result = await api.request<any>(
    'identity',
    `admin/system-users/${deleteForm.userId}`,
    { method: 'DELETE' }
  )
  deleteState.loading = false
  if (!result.ok) {
    deleteState.error = result.error || 'Failed to delete user.'
    return
  }
  deleteState.success = 'User deleted.'
}
</script>

<template>
  <div class="space-y-6">
    <UCard class="border border-slate-200/60">
      <div>
        <p class="text-xs uppercase tracking-[0.3em] text-emerald-600">Identity</p>
        <h1 class="text-2xl font-semibold text-slate-900">System Users</h1>
        <p class="text-sm text-muted">View and manage Identity system users.</p>
      </div>
    </UCard>

    <FeatureTabs v-model="active" :tabs="tabs" />

    <div v-if="active === 'query'" class="space-y-6">
      <UCard class="border border-slate-200/60">
        <template #header>
          <h2 class="text-lg font-semibold">Query users</h2>
        </template>
        <div class="grid gap-4 md:grid-cols-[1fr_1fr_auto]">
          <UFormField label="Page">
            <UInput v-model.number="queryForm.page" type="number" min="1" />
          </UFormField>
          <UFormField label="Page Size">
            <UInput v-model.number="queryForm.pageSize" type="number" min="1" />
          </UFormField>
          <UButton color="primary" class="self-end" @click="queryUsers">
            Query
          </UButton>
        </div>
        <div class="mt-4">
          <FormStatus :loading="queryState.loading" :error="queryState.error" :empty="queryState.empty" />
          <UTable
            v-if="queryResults.length"
            :rows="queryResults"
            :columns="[
              { key: 'id', label: 'Id' },
              { key: 'email', label: 'Email' },
              { key: 'role', label: 'Role' },
              { key: 'created', label: 'Created' }
            ]"
          >
            <template #id-data="{ row }">
              <span class="font-mono text-xs">{{ row.id }}</span>
            </template>
            <template #created-data="{ row }">
              {{ formatDateTime(row.created) }}
            </template>
          </UTable>
        </div>
      </UCard>

      <UCard class="border border-slate-200/60">
        <template #header>
          <h2 class="text-lg font-semibold">Lookup user</h2>
        </template>
        <div class="flex gap-3">
          <UFormField label="User Id" required class="flex-1">
            <UInput v-model="lookupForm.userId" placeholder="GUID" />
          </UFormField>
          <UButton color="neutral" variant="outline" class="self-end" @click="lookupUser">
            Load
          </UButton>
        </div>
        <FormStatus :loading="lookupState.loading" :error="lookupState.error" />
        <div v-if="lookupResult" class="mt-4 rounded-lg border border-slate-200/60 p-4 text-sm">
          <p class="font-semibold">{{ lookupResult.email }}</p>
          <p class="text-muted">Role: {{ lookupResult.role }}</p>
          <p class="text-muted">Created: {{ formatDateTime(lookupResult.created) }}</p>
        </div>
      </UCard>
    </div>

    <div v-else-if="active === 'update'" class="grid gap-6 lg:grid-cols-[2fr_1fr]">
      <UCard class="border border-slate-200/60">
        <template #header>
          <h2 class="text-lg font-semibold">Update user</h2>
        </template>
        <UForm @submit="updateUser" class="space-y-4">
          <UFormField label="User Id" required>
            <UInput v-model="updateForm.userId" placeholder="GUID" />
          </UFormField>
          <UFormField label="Email">
            <UInput v-model="updateForm.email" type="email" placeholder="user@matterway.local" />
          </UFormField>
          <UFormField label="Password">
            <UInput v-model="updateForm.password" type="password" placeholder="New password" />
          </UFormField>
          <UButton type="submit" color="primary" :loading="updateState.loading">
            Update User
          </UButton>
          <FormStatus :error="updateState.error" :success="updateState.success" />
        </UForm>
      </UCard>
      <UCard class="border border-slate-200/60 bg-white/80">
        <template #header>
          <h3 class="text-sm font-semibold text-muted">Note</h3>
        </template>
        <p class="text-sm text-muted">
          Operators can only update Customer users unless they are Administrators.
        </p>
      </UCard>
    </div>

    <div v-else-if="active === 'delete'" class="grid gap-6 lg:grid-cols-[2fr_1fr]">
      <UCard class="border border-slate-200/60">
        <template #header>
          <h2 class="text-lg font-semibold">Delete user</h2>
        </template>
        <UForm @submit="deleteUser" class="space-y-4">
          <UFormField label="User Id" required>
            <UInput v-model="deleteForm.userId" placeholder="GUID" />
          </UFormField>
          <UButton type="submit" color="red" variant="solid" :loading="deleteState.loading">
            Delete User
          </UButton>
          <FormStatus :error="deleteState.error" :success="deleteState.success" />
        </UForm>
      </UCard>
      <UCard class="border border-slate-200/60 bg-white/80">
        <template #header>
          <h3 class="text-sm font-semibold text-muted">Permissions</h3>
        </template>
        <p class="text-sm text-muted">
          Deleting a user requires Administrator permissions.
        </p>
      </UCard>
    </div>
  </div>
</template>
