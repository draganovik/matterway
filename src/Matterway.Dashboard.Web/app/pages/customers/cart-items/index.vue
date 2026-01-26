<script setup lang="ts">
import { formatMoney } from '~/utils/formatters'
import { useApiClient } from '~/composables/useApiClient'
import { useRequestState } from '~/composables/useRequestState'

definePageMeta({
  title: 'Cart Items',
  service: 'customers',
  level: 'observer'
})

const api = useApiClient()

const queryForm = reactive({
  customerId: '',
  articleId: ''
})
const queryState = useRequestState()
const queryResult = ref<{
  articleName: string
  quantity: number
  unitPrice: number
} | null>(null)

async function queryCartItem() {
  queryState.error = ''
  queryState.loading = true
  queryResult.value = null
  try {
    const result = await api.request(
      'customers',
      `admin/customers/${queryForm.customerId}/cart-items/${queryForm.articleId}`
    )
    if (!result.ok) {
      queryState.error = result.error || 'Failed to load cart item.'
      return
    }
    queryResult.value = result.data as typeof queryResult.value
  } catch (err) {
    queryState.error = err instanceof Error ? err.message : 'Failed to load cart item.'
  } finally {
    queryState.loading = false
  }
}
</script>

<template>
  <FeatureShell>
    <div
      class="grid gap-6 lg:grid-cols-[2fr_1fr]"
    >
      <UCard class="border border-default">
        <template #header>
          <h2 class="text-lg font-semibold">
            Lookup cart item
          </h2>
        </template>
        <UForm
          class="space-y-4"
          @submit="queryCartItem"
        >
          <UFormField
            label="Customer Id"
            required
          >
            <UInput
              v-model="queryForm.customerId"
              placeholder="GUID"
            />
          </UFormField>
          <UFormField
            label="Article Id"
            required
          >
            <UInput
              v-model="queryForm.articleId"
              placeholder="GUID"
            />
          </UFormField>
          <UButton
            type="submit"
            color="primary"
            :loading="queryState.loading"
          >
            Fetch Cart Item
          </UButton>
          <FormStatus :error="queryState.error" />
        </UForm>
      </UCard>
      <UCard class="border border-default bg-elevated/40">
        <template #header>
          <h3 class="text-sm font-semibold text-muted">
            Result
          </h3>
        </template>
        <div
          v-if="queryResult"
          class="space-y-2 text-sm"
        >
          <p class="font-semibold">
            {{ queryResult.articleName }}
          </p>
          <p class="text-muted">
            Quantity: {{ queryResult.quantity }}
          </p>
          <p class="text-muted">
            Unit Price: {{ formatMoney(queryResult.unitPrice) }}
          </p>
        </div>
        <p
          v-else
          class="text-sm text-muted"
        >
          Submit a lookup to see details.
        </p>
      </UCard>
    </div>
  </FeatureShell>
</template>
