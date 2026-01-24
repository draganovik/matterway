<script setup lang="ts">
import { useApiClient } from '~/composables/useApiClient'
import { useFeatureTabs } from '~/composables/useFeatureTabs'
import { formatDateTime, formatMoney } from '~/utils/format'

definePageMeta({
  title: 'Orders',
  service: 'sales',
  level: 'observer'
})

const api = useApiClient()
const { tabs, active } = useFeatureTabs()

const queryForm = reactive({
  page: 1,
  pageSize: 10,
  customerId: ''
})
const queryState = reactive({ loading: false, error: '', empty: '' })
const queryResults = ref<any[]>([])

const lookupForm = reactive({
  orderId: ''
})
const lookupState = reactive({ loading: false, error: '' })
const lookupResult = ref<any | null>(null)

const statusForm = reactive({
  orderId: '',
  status: 'Processing',
  note: ''
})
const statusState = reactive({ loading: false, error: '', success: '' })

const statusOptions = [
  'Processing',
  'Reserved',
  'Delivery',
  'Completed',
  'Cancelled'
]

async function queryOrders() {
  queryState.loading = true
  queryState.error = ''
  queryState.empty = ''
  queryResults.value = []
  const params = new URLSearchParams({
    page: queryForm.page.toString(),
    pageSize: queryForm.pageSize.toString()
  })
  if (queryForm.customerId) params.set('customerId', queryForm.customerId)
  const result = await api.request<any>('sales', `admin/orders?${params.toString()}`)
  queryState.loading = false
  if (!result.ok) {
    queryState.error = result.error || 'Failed to query orders.'
    return
  }
  queryResults.value = result.data?.data ?? []
  if (!queryResults.value.length) queryState.empty = 'No orders found.'
}

async function lookupOrder() {
  lookupState.loading = true
  lookupState.error = ''
  lookupResult.value = null
  const result = await api.request<any>('sales', `admin/orders/${lookupForm.orderId}`)
  lookupState.loading = false
  if (!result.ok) {
    lookupState.error = result.error || 'Order not found.'
    return
  }
  lookupResult.value = result.data
}

async function addStatus() {
  statusState.loading = true
  statusState.error = ''
  statusState.success = ''
  const result = await api.request<any>(
    'sales',
    `admin/orders/${statusForm.orderId}/statuses`,
    {
      method: 'POST',
      body: JSON.stringify({
        status: statusForm.status,
        note: statusForm.note || null
      })
    }
  )
  statusState.loading = false
  if (!result.ok) {
    statusState.error = result.error || 'Failed to add status.'
    return
  }
  statusState.success = 'Status added.'
}
</script>

<template>
  <div class="space-y-6">
    <UCard class="border border-slate-200/60">
      <div>
        <p class="text-xs uppercase tracking-[0.3em] text-emerald-600">Sales</p>
        <h1 class="text-2xl font-semibold text-slate-900">Orders</h1>
        <p class="text-sm text-muted">Monitor and update order lifecycle.</p>
      </div>
    </UCard>

    <FeatureTabs v-model="active" :tabs="tabs" />

    <div v-if="active === 'query'" class="space-y-6">
      <UCard class="border border-slate-200/60">
        <template #header>
          <h2 class="text-lg font-semibold">Query orders</h2>
        </template>
        <div class="grid gap-4 md:grid-cols-[1fr_1fr_2fr_auto]">
          <UFormField label="Page">
            <UInput v-model.number="queryForm.page" type="number" min="1" />
          </UFormField>
          <UFormField label="Page Size">
            <UInput v-model.number="queryForm.pageSize" type="number" min="1" />
          </UFormField>
          <UFormField label="Customer Id (optional)">
            <UInput v-model="queryForm.customerId" placeholder="GUID" />
          </UFormField>
          <UButton color="primary" class="self-end" @click="queryOrders">
            Query
          </UButton>
        </div>
        <div class="mt-4">
          <FormStatus :loading="queryState.loading" :error="queryState.error" :empty="queryState.empty" />
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

      <UCard class="border border-slate-200/60">
        <template #header>
          <h2 class="text-lg font-semibold">Lookup order</h2>
        </template>
        <div class="flex gap-3">
          <UFormField label="Order Id" required class="flex-1">
            <UInput v-model="lookupForm.orderId" placeholder="GUID" />
          </UFormField>
          <UButton color="neutral" variant="outline" class="self-end" @click="lookupOrder">
            Load
          </UButton>
        </div>
        <FormStatus :loading="lookupState.loading" :error="lookupState.error" />
        <div v-if="lookupResult" class="mt-4 space-y-3 text-sm">
          <div class="rounded-lg border border-slate-200/60 p-4">
            <p class="font-semibold">Order {{ lookupResult.id }}</p>
            <p class="text-muted">Total: {{ formatMoney(lookupResult.totalAmount) }}</p>
            <p class="text-muted">Placed: {{ formatDateTime(lookupResult.placedAt) }}</p>
          </div>
          <div class="rounded-lg border border-slate-200/60 p-4">
            <p class="font-semibold">Status history</p>
            <ul class="mt-2 space-y-1">
              <li v-for="status in lookupResult.statusHistory" :key="status.changedAt">
                {{ status.status }} • {{ formatDateTime(status.changedAt) }} {{ status.note ? `(${status.note})` : '' }}
              </li>
            </ul>
          </div>
        </div>
      </UCard>
    </div>

    <div v-else-if="active === 'create'" class="grid gap-6 lg:grid-cols-[2fr_1fr]">
      <UCard class="border border-slate-200/60">
        <template #header>
          <h2 class="text-lg font-semibold">Add order status</h2>
        </template>
        <UForm @submit="addStatus" class="space-y-4">
          <UFormField label="Order Id" required>
            <UInput v-model="statusForm.orderId" placeholder="GUID" />
          </UFormField>
          <UFormField label="Status" required>
            <USelectMenu v-model="statusForm.status" :items="statusOptions" />
          </UFormField>
          <UFormField label="Note">
            <UTextarea v-model="statusForm.note" :rows="3" />
          </UFormField>
          <UButton type="submit" color="primary" :loading="statusState.loading">
            Add Status
          </UButton>
          <FormStatus :error="statusState.error" :success="statusState.success" />
        </UForm>
      </UCard>
      <UCard class="border border-slate-200/60 bg-white/80">
        <template #header>
          <h3 class="text-sm font-semibold text-muted">Workflow</h3>
        </template>
        <p class="text-sm text-muted">
          Status updates are appended to the order history and affect downstream fulfillment.
        </p>
      </UCard>
    </div>
  </div>
</template>
