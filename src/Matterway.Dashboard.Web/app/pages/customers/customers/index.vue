<script setup lang="ts">
definePageMeta({
  title: 'Customers',
  service: 'customers',
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
