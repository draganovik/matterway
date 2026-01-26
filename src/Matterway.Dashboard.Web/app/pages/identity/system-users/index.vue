<script setup lang="ts">
import { formatDateTime } from '~/utils/formatters'
import { useApiClient } from '~/composables/useApiClient'
import { useRequestState } from '~/composables/useRequestState'
import { buildQuery, normalizeList } from '~/utils/http'

definePageMeta({
  title: 'System Users',
  service: 'identity',
  level: 'operator'
})

const api = useApiClient()

const queryForm = reactive({
  page: 1,
  pageSize: 25
})
const queryState = useRequestState()
const queryResults = ref<Array<{
  id: string
  email: string
  role: string
  created: string
}>>([])

const lookupForm = reactive({
  userId: ''
})
const lookupState = useRequestState()
const lookupResult = ref<{
  id: string
  email: string
  role: string
  created: string
} | null>(null)

async function queryUsers() {
  queryState.error = ''
  queryState.empty = ''
  queryState.loading = true
  try {
    const query = buildQuery({
      page: queryForm.page,
      pageSize: queryForm.pageSize
    })
    const result = await api.request<unknown>('identity', `admin/system-users${query}`)
    if (!result.ok) {
      queryState.error = result.error || 'Failed to load users.'
      queryResults.value = []
      return
    }
    queryResults.value = normalizeList(result.data)
    if (!queryResults.value.length) {
      queryState.empty = 'No users found.'
    }
  } catch (err) {
    queryState.error = err instanceof Error ? err.message : 'Failed to load users.'
    queryResults.value = []
  } finally {
    queryState.loading = false
  }
}

async function lookupUser() {
  lookupState.error = ''
  lookupState.loading = true
  lookupResult.value = null
  try {
    const result = await api.request('identity', `admin/system-users/${lookupForm.userId}`)
    if (!result.ok) {
      lookupState.error = result.error || 'Failed to load user.'
      return
    }
    lookupResult.value = result.data as typeof lookupResult.value
  } catch (err) {
    lookupState.error = err instanceof Error ? err.message : 'Failed to load user.'
  } finally {
    lookupState.loading = false
  }
}
</script>

<template>
  <FeatureShell>
    <div
      class="space-y-6"
    >
      <UCard class="border border-default">
        <template #header>
          <h2 class="text-lg font-semibold">
            Query users
          </h2>
        </template>
        <div class="grid gap-4 md:grid-cols-[1fr_1fr_auto]">
          <UFormField label="Page">
            <UInput
              v-model.number="queryForm.page"
              type="number"
              min="1"
            />
          </UFormField>
          <UFormField label="Page Size">
            <UInput
              v-model.number="queryForm.pageSize"
              type="number"
              min="1"
            />
          </UFormField>
          <UButton
            color="primary"
            class="self-end"
            @click="queryUsers"
          >
            Query
          </UButton>
        </div>
        <div class="mt-4">
          <FormStatus
            :loading="queryState.loading"
            :error="queryState.error"
            :empty="queryState.empty"
          />
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

      <UCard class="border border-default">
        <template #header>
          <h2 class="text-lg font-semibold">
            Lookup user
          </h2>
        </template>
        <div class="flex gap-3">
          <UFormField
            label="User Id"
            required
            class="flex-1"
          >
            <UInput
              v-model="lookupForm.userId"
              placeholder="GUID"
            />
          </UFormField>
          <UButton
            color="neutral"
            variant="outline"
            class="self-end"
            @click="lookupUser"
          >
            Load
          </UButton>
        </div>
        <FormStatus
          :loading="lookupState.loading"
          :error="lookupState.error"
        />
        <div
          v-if="lookupResult"
          class="mt-4 rounded-lg border border-default p-4 text-sm"
        >
          <p class="font-semibold">
            {{ lookupResult.email }}
          </p>
          <p class="text-muted">
            Role: {{ lookupResult.role }}
          </p>
          <p class="text-muted">
            Created: {{ formatDateTime(lookupResult.created) }}
          </p>
        </div>
      </UCard>
    </div>
  </FeatureShell>
</template>
