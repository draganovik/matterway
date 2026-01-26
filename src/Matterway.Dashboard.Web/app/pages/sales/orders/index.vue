<script setup lang="ts">
import { formatDateTime, formatMoney } from '~/utils/formatters'
import { useApiClient } from '~/composables/useApiClient'
import { useRequestState } from '~/composables/useRequestState'
import { buildQuery, normalizeList } from '~/utils/http'

definePageMeta({
  title: 'Orders',
  service: 'sales',
  level: 'observer'
})

const api = useApiClient()

const queryForm = reactive({
  page: 1,
  pageSize: 25,
  customerId: ''
})
const queryState = useRequestState()
const queryResults = ref<Array<{
  id: string
  customerId: string
  type: string
  totalAmount: number
  placedAt: string
}>>([])

const lookupForm = reactive({
  orderId: ''
})
const lookupState = useRequestState()
const lookupResult = ref<{
  id: string
  totalAmount: number
  placedAt: string
  statusHistory: Array<{ status: string, changedAt: string, note?: string }>
} | null>(null)

async function queryOrders() {
  queryState.error = ''
  queryState.empty = ''
  queryState.loading = true
  try {
    const query = buildQuery({
      page: queryForm.page,
      pageSize: queryForm.pageSize,
      customerId: queryForm.customerId
    })
    const result = await api.request<unknown>('sales', `admin/orders${query}`)
    if (!result.ok) {
      queryState.error = result.error || 'Failed to load orders.'
      queryResults.value = []
      return
    }
    queryResults.value = normalizeList(result.data)
    if (!queryResults.value.length) {
      queryState.empty = 'No orders found.'
    }
  } catch (err) {
    queryState.error = err instanceof Error ? err.message : 'Failed to load orders.'
    queryResults.value = []
  } finally {
    queryState.loading = false
  }
}

async function lookupOrder() {
  lookupState.error = ''
  lookupState.loading = true
  lookupResult.value = null
  try {
    const result = await api.request('sales', `admin/orders/${lookupForm.orderId}`)
    if (!result.ok) {
      lookupState.error = result.error || 'Failed to load order.'
      return
    }
    lookupResult.value = result.data as typeof lookupResult.value
  } catch (err) {
    lookupState.error = err instanceof Error ? err.message : 'Failed to load order.'
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
            Query orders
          </h2>
        </template>
        <div class="grid gap-4 md:grid-cols-[1fr_1fr_2fr_auto]">
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
          <UFormField label="Customer Id (optional)">
            <UInput
              v-model="queryForm.customerId"
              placeholder="GUID"
            />
          </UFormField>
          <UButton
            color="primary"
            class="self-end"
            @click="queryOrders"
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
              { key: 'id', label: 'Order Id' },
              { key: 'customerId', label: 'Customer' },
              { key: 'type', label: 'Type' },
              { key: 'totalAmount', label: 'Total' },
              { key: 'placedAt', label: 'Placed' }
            ]"
          >
            <template #id-data="{ row }">
              <span class="font-mono text-xs">{{ row.id }}</span>
            </template>
            <template #totalAmount-data="{ row }">
              {{ formatMoney(row.totalAmount) }}
            </template>
            <template #placedAt-data="{ row }">
              {{ formatDateTime(row.placedAt) }}
            </template>
          </UTable>
        </div>
      </UCard>

      <UCard class="border border-default">
        <template #header>
          <h2 class="text-lg font-semibold">
            Lookup order
          </h2>
        </template>
        <div class="flex gap-3">
          <UFormField
            label="Order Id"
            required
            class="flex-1"
          >
            <UInput
              v-model="lookupForm.orderId"
              placeholder="GUID"
            />
          </UFormField>
          <UButton
            color="neutral"
            variant="outline"
            class="self-end"
            @click="lookupOrder"
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
          class="mt-4 space-y-3 text-sm"
        >
          <div class="rounded-lg border border-default p-4">
            <p class="font-semibold">
              Order {{ lookupResult.id }}
            </p>
            <p class="text-muted">
              Total: {{ formatMoney(lookupResult.totalAmount) }}
            </p>
            <p class="text-muted">
              Placed: {{ formatDateTime(lookupResult.placedAt) }}
            </p>
          </div>
          <div class="rounded-lg border border-default p-4">
            <p class="font-semibold">
              Status history
            </p>
            <ul class="mt-2 space-y-1">
              <li
                v-for="status in lookupResult.statusHistory"
                :key="status.changedAt"
              >
                {{ status.status }} • {{ formatDateTime(status.changedAt) }} {{ status.note ? `(${status.note})` : '' }}
              </li>
            </ul>
          </div>
        </div>
      </UCard>
    </div>
  </FeatureShell>
</template>
