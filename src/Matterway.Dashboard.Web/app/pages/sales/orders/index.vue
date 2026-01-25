<script setup lang="ts">
import { formatDateTime, formatMoney } from '~/utils/format'

definePageMeta({
  title: 'Orders',
  service: 'sales',
  level: 'observer'
})
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
