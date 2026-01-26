<script setup lang="ts">
import { useApiClient } from '~/composables/useApiClient'
import { useRequestState } from '~/composables/useRequestState'
import { buildQuery, normalizeList } from '~/utils/http'

definePageMeta({
  title: 'Customers',
  service: 'customers',
  level: 'observer'
})

const api = useApiClient()

const queryForm = reactive({
  page: 1,
  pageSize: 25
})
const queryState = useRequestState()
const queryResults = ref<Array<{
  systemUserId: string
  firstName: string
  lastName: string
  birthDate: string
}>>([])

const lookupForm = reactive({
  customerId: ''
})
const lookupState = useRequestState()
const lookupResult = ref<{
  systemUserId: string
  firstName: string
  lastName: string
  birthDate: string
} | null>(null)

async function queryCustomers() {
  queryState.error = ''
  queryState.empty = ''
  queryState.loading = true
  try {
    const query = buildQuery({
      page: queryForm.page,
      pageSize: queryForm.pageSize
    })
    const result = await api.request<unknown>('customers', `admin/customers${query}`)
    if (!result.ok) {
      queryState.error = result.error || 'Failed to load customers.'
      queryResults.value = []
      return
    }
    queryResults.value = normalizeList(result.data)
    if (!queryResults.value.length) {
      queryState.empty = 'No customers found.'
    }
  } catch (err) {
    queryState.error = err instanceof Error ? err.message : 'Failed to load customers.'
    queryResults.value = []
  } finally {
    queryState.loading = false
  }
}

async function lookupCustomer() {
  lookupState.error = ''
  lookupState.loading = true
  lookupResult.value = null
  try {
    const result = await api.request(
      'customers',
      `admin/customers/${lookupForm.customerId}`
    )
    if (!result.ok) {
      lookupState.error = result.error || 'Failed to load customer.'
      return
    }
    lookupResult.value = result.data as typeof lookupResult.value
  } catch (err) {
    lookupState.error = err instanceof Error ? err.message : 'Failed to load customer.'
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
            Query customers
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
            @click="queryCustomers"
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
              { key: 'systemUserId', label: 'User Id' },
              { key: 'firstName', label: 'First Name' },
              { key: 'lastName', label: 'Last Name' },
              { key: 'birthDate', label: 'Birth Date' }
            ]"
          >
            <template #systemUserId-data="{ row }">
              <span class="font-mono text-xs">{{ row.systemUserId }}</span>
            </template>
          </UTable>
        </div>
      </UCard>

      <UCard class="border border-default">
        <template #header>
          <h2 class="text-lg font-semibold">
            Lookup customer
          </h2>
        </template>
        <div class="flex gap-3">
          <UFormField
            label="Customer Id"
            required
            class="flex-1"
          >
            <UInput
              v-model="lookupForm.customerId"
              placeholder="GUID"
            />
          </UFormField>
          <UButton
            color="neutral"
            variant="outline"
            class="self-end"
            @click="lookupCustomer"
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
            {{ lookupResult.firstName }} {{ lookupResult.lastName }}
          </p>
          <p class="text-muted">
            User Id: {{ lookupResult.systemUserId }}
          </p>
          <p class="text-muted">
            Birth Date: {{ lookupResult.birthDate }}
          </p>
        </div>
      </UCard>
    </div>
  </FeatureShell>
</template>
