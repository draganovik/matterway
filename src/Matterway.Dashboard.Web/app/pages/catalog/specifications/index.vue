<script setup lang="ts">
import { useApiClient } from '~/composables/useApiClient'

definePageMeta({
  title: 'Specifications',
  service: 'catalog',
  level: 'observer'
})
</script>

<template>
  <div class="space-y-6">
    <div
      class="space-y-4"
    >
      <UCard class="border border-default">
        <template #header>
          <h2 class="text-lg font-semibold">
            Query specifications
          </h2>
        </template>
        <div class="grid gap-4 md:grid-cols-[2fr_1fr_auto]">
          <UFormField label="Title Like">
            <UInput
              v-model="queryForm.titleLike"
              placeholder="Bandwidth"
            />
          </UFormField>
          <UFormField label="Limit">
            <UInput
              v-model.number="queryForm.limit"
              type="number"
              min="1"
              max="50"
            />
          </UFormField>
          <UButton
            color="primary"
            class="self-end"
            @click="querySpecs"
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
              { key: 'slug', label: 'Slug' },
              { key: 'title', label: 'Title' },
              { key: 'unit', label: 'Unit' }
            ]"
          />
        </div>
      </UCard>
    </div>
  </div>
</template>
