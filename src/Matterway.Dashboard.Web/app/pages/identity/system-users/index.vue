<script setup lang="ts">
import { formatDateTime } from '~/utils/format'

definePageMeta({
  title: 'System Users',
  service: 'identity',
  level: 'operator'
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
