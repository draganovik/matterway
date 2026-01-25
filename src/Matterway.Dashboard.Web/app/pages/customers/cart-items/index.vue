<script setup lang="ts">
import { useApiClient } from '~/composables/useApiClient'
import { formatMoney } from '~/utils/format'

definePageMeta({
  title: 'Cart Items',
  service: 'customers',
  level: 'observer'
})
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
