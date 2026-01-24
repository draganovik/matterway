<script setup lang="ts">
import { useApiClient } from '~/composables/useApiClient'
import { useFeatureTabs } from '~/composables/useFeatureTabs'
import { formatDateTime, formatMoney } from '~/utils/format'

definePageMeta({
  title: 'Payments',
  service: 'sales',
  level: 'observer'
})

const api = useApiClient()
const { tabs, active } = useFeatureTabs()

const queryForm = reactive({
  page: 1,
  pageSize: 10,
  orderId: ''
})
const queryState = reactive({ loading: false, error: '', empty: '' })
const queryResults = ref<any[]>([])

const lookupForm = reactive({
  paymentId: ''
})
const lookupState = reactive({ loading: false, error: '' })
const lookupResult = ref<any | null>(null)

async function queryPayments() {
  queryState.loading = true
  queryState.error = ''
  queryState.empty = ''
  queryResults.value = []
  const params = new URLSearchParams({
    page: queryForm.page.toString(),
    pageSize: queryForm.pageSize.toString()
  })
  if (queryForm.orderId) params.set('orderId', queryForm.orderId)
  const result = await api.request<any>('sales', `admin/payments?${params.toString()}`)
  queryState.loading = false
  if (!result.ok) {
    queryState.error = result.error || 'Failed to query payments.'
    return
  }
  queryResults.value = result.data?.data ?? []
  if (!queryResults.value.length) queryState.empty = 'No payments found.'
}

async function lookupPayment() {
  lookupState.loading = true
  lookupState.error = ''
  lookupResult.value = null
  const result = await api.request<any>('sales', `admin/payments/${lookupForm.paymentId}`)
  lookupState.loading = false
  if (!result.ok) {
    lookupState.error = result.error || 'Payment not found.'
    return
  }
  lookupResult.value = result.data
}
</script>

<template>
  <div class="space-y-6">
    <UCard class="border border-slate-200/60">
      <div>
        <p class="text-xs uppercase tracking-[0.3em] text-emerald-600">Sales</p>
        <h1 class="text-2xl font-semibold text-slate-900">Payments</h1>
        <p class="text-sm text-muted">Inspect payment records.</p>
      </div>
    </UCard>

    <FeatureTabs v-model="active" :tabs="tabs" />

    <div v-if="active === 'query'" class="space-y-6">
      <UCard class="border border-slate-200/60">
        <template #header>
          <h2 class="text-lg font-semibold">Query payments</h2>
        </template>
        <div class="grid gap-4 md:grid-cols-[1fr_1fr_2fr_auto]">
          <UFormField label="Page">
            <UInput v-model.number="queryForm.page" type="number" min="1" />
          </UFormField>
          <UFormField label="Page Size">
            <UInput v-model.number="queryForm.pageSize" type="number" min="1" />
          </UFormField>
          <UFormField label="Order Id (optional)">
            <UInput v-model="queryForm.orderId" placeholder="GUID" />
          </UFormField>
          <UButton color="primary" class="self-end" @click="queryPayments">
            Query
          </UButton>
        </div>
        <div class="mt-4">
          <FormStatus :loading="queryState.loading" :error="queryState.error" :empty="queryState.empty" />
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

      <UCard class="border border-slate-200/60">
        <template #header>
          <h2 class="text-lg font-semibold">Lookup payment</h2>
        </template>
        <div class="flex gap-3">
          <UFormField label="Payment Id" required class="flex-1">
            <UInput v-model="lookupForm.paymentId" placeholder="GUID" />
          </UFormField>
          <UButton color="neutral" variant="outline" class="self-end" @click="lookupPayment">
            Load
          </UButton>
        </div>
        <FormStatus :loading="lookupState.loading" :error="lookupState.error" />
        <div v-if="lookupResult" class="mt-4 rounded-lg border border-slate-200/60 p-4 text-sm">
          <p class="font-semibold">Payment {{ lookupResult.id }}</p>
          <p class="text-muted">Provider: {{ lookupResult.provider }}</p>
          <p class="text-muted">Amount: {{ formatMoney(lookupResult.amount) }}</p>
          <p class="text-muted">Status: {{ lookupResult.status }}</p>
          <p class="text-muted">Created: {{ formatDateTime(lookupResult.createdAt) }}</p>
        </div>
      </UCard>
    </div>
  </div>
</template>
