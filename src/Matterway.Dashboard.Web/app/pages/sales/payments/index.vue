<script setup lang="ts">
import { formatDateTime, formatMoney } from '~/utils/formatters'
import { useApiClient } from '~/composables/useApiClient'
import { useRequestState } from '~/composables/useRequestState'
import { buildQuery, normalizeList } from '~/utils/http'

definePageMeta({
  title: 'Payments',
  service: 'sales',
  level: 'observer'
})

const api = useApiClient()

const queryForm = reactive({
  page: 1,
  pageSize: 25,
  orderId: ''
})
const queryState = useRequestState()
const queryResults = ref<Array<{
  id: string
  orderId: string
  provider: string
  amount: number
  status: string
  createdAt: string
}>>([])

const lookupForm = reactive({
  paymentId: ''
})
const lookupState = useRequestState()
const lookupResult = ref<{
  id: string
  provider: string
  amount: number
  status: string
  createdAt: string
} | null>(null)

async function queryPayments() {
  queryState.error = ''
  queryState.empty = ''
  queryState.loading = true
  try {
    const query = buildQuery({
      page: queryForm.page,
      pageSize: queryForm.pageSize,
      orderId: queryForm.orderId
    })
    const result = await api.request<unknown>('sales', `admin/payments${query}`)
    if (!result.ok) {
      queryState.error = result.error || 'Failed to load payments.'
      queryResults.value = []
      return
    }
    queryResults.value = normalizeList(result.data)
    if (!queryResults.value.length) {
      queryState.empty = 'No payments found.'
    }
  } catch (err) {
    queryState.error = err instanceof Error ? err.message : 'Failed to load payments.'
    queryResults.value = []
  } finally {
    queryState.loading = false
  }
}

async function lookupPayment() {
  lookupState.error = ''
  lookupState.loading = true
  lookupResult.value = null
  try {
    const result = await api.request('sales', `admin/payments/${lookupForm.paymentId}`)
    if (!result.ok) {
      lookupState.error = result.error || 'Failed to load payment.'
      return
    }
    lookupResult.value = result.data as typeof lookupResult.value
  } catch (err) {
    lookupState.error = err instanceof Error ? err.message : 'Failed to load payment.'
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
            Query payments
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
          <UFormField label="Order Id (optional)">
            <UInput
              v-model="queryForm.orderId"
              placeholder="GUID"
            />
          </UFormField>
          <UButton
            color="primary"
            class="self-end"
            @click="queryPayments"
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
              { key: 'id', label: 'Payment Id' },
              { key: 'orderId', label: 'Order' },
              { key: 'provider', label: 'Provider' },
              { key: 'amount', label: 'Amount' },
              { key: 'status', label: 'Status' },
              { key: 'createdAt', label: 'Created' }
            ]"
          >
            <template #id-data="{ row }">
              <span class="font-mono text-xs">{{ row.id }}</span>
            </template>
            <template #amount-data="{ row }">
              {{ formatMoney(row.amount) }}
            </template>
            <template #createdAt-data="{ row }">
              {{ formatDateTime(row.createdAt) }}
            </template>
          </UTable>
        </div>
      </UCard>

      <UCard class="border border-default">
        <template #header>
          <h2 class="text-lg font-semibold">
            Lookup payment
          </h2>
        </template>
        <div class="flex gap-3">
          <UFormField
            label="Payment Id"
            required
            class="flex-1"
          >
            <UInput
              v-model="lookupForm.paymentId"
              placeholder="GUID"
            />
          </UFormField>
          <UButton
            color="neutral"
            variant="outline"
            class="self-end"
            @click="lookupPayment"
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
            Payment {{ lookupResult.id }}
          </p>
          <p class="text-muted">
            Provider: {{ lookupResult.provider }}
          </p>
          <p class="text-muted">
            Amount: {{ formatMoney(lookupResult.amount) }}
          </p>
          <p class="text-muted">
            Status: {{ lookupResult.status }}
          </p>
          <p class="text-muted">
            Created: {{ formatDateTime(lookupResult.createdAt) }}
          </p>
        </div>
      </UCard>
    </div>
  </FeatureShell>
</template>
