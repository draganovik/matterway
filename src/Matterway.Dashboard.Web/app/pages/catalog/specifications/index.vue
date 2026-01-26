<script setup lang="ts">
import { useApiClient } from '~/composables/useApiClient'
import { useRequestState } from '~/composables/useRequestState'
import { buildQuery, normalizeList } from '~/utils/http'

definePageMeta({
  title: 'Specifications',
  service: 'catalog',
  level: 'observer'
})

const api = useApiClient()

const queryForm = reactive({
  titleLike: '',
  limit: 25
})
const queryState = useRequestState()
const queryResults = ref<Array<{ slug: string, title: string, unit?: string }>>([])

async function querySpecs() {
  queryState.error = ''
  queryState.empty = ''
  queryState.loading = true
  try {
    const query = buildQuery({
      titleLike: queryForm.titleLike,
      limit: queryForm.limit
    })
    const result = await api.request<unknown>('catalog', `admin/specifications${query}`)
    if (!result.ok) {
      queryState.error = result.error || 'Failed to load specifications.'
      queryResults.value = []
      return
    }
    queryResults.value = normalizeList(result.data)
    if (!queryResults.value.length) {
      queryState.empty = 'No specifications found.'
    }
  } catch (err) {
    queryState.error = err instanceof Error ? err.message : 'Failed to load specifications.'
    queryResults.value = []
  } finally {
    queryState.loading = false
  }
}
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
