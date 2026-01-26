<script setup lang="ts">
import { formatMoney } from '~/utils/formatters'
import { useApiClient } from '~/composables/useApiClient'
import { useRequestState } from '~/composables/useRequestState'
import { useDiscountSelection } from '~/composables/useDiscountSelection'
import { buildQuery, normalizeList } from '~/utils/http'

definePageMeta({
  title: 'Discounts',
  service: 'catalog',
  level: 'operator'
})

const api = useApiClient()

const searchForm = reactive({
  filter: '',
  page: 1,
  pageSize: 25
})
const searchState = useRequestState()
const searchResults = ref<Array<{
  id: string
  title: string
  price: number
  isAvailable: boolean
}>>([])

const { addArticleId } = useDiscountSelection()

async function searchArticles() {
  searchState.error = ''
  searchState.empty = ''
  searchState.loading = true
  try {
    const query = buildQuery({
      filter: searchForm.filter,
      page: searchForm.page,
      pageSize: searchForm.pageSize
    })
    const result = await api.request<unknown>('catalog', `admin/articles${query}`)
    if (!result.ok) {
      searchState.error = result.error || 'Failed to load articles.'
      searchResults.value = []
      return
    }
    searchResults.value = normalizeList(result.data)
    if (!searchResults.value.length) {
      searchState.empty = 'No articles found.'
    }
  } catch (err) {
    searchState.error = err instanceof Error ? err.message : 'Failed to load articles.'
    searchResults.value = []
  } finally {
    searchState.loading = false
  }
}
</script>

<template>
  <FeatureShell>
    <div class="space-y-6">
      <UCard
        class="border border-default"
      >
        <template #header>
          <div>
            <h2 class="text-lg font-semibold">
              Find articles
            </h2>
            <p class="text-sm text-muted">
              Use public catalog query to select article IDs.
            </p>
          </div>
        </template>
        <div class="grid gap-4 md:grid-cols-[2fr_1fr_1fr_auto]">
          <UFormField label="Filter">
            <UInput
              v-model="searchForm.filter"
              placeholder="title==Router"
            />
          </UFormField>
          <UFormField label="Page">
            <UInput
              v-model.number="searchForm.page"
              type="number"
              min="1"
            />
          </UFormField>
          <UFormField label="Page Size">
            <UInput
              v-model.number="searchForm.pageSize"
              type="number"
              min="1"
            />
          </UFormField>
          <UButton
            color="neutral"
            variant="outline"
            class="self-end"
            @click="searchArticles"
          >
            Search
          </UButton>
        </div>
        <div class="mt-4">
          <FormStatus
            :loading="searchState.loading"
            :error="searchState.error"
            :empty="searchState.empty"
          />
          <UTable
            v-if="searchResults.length"
            :rows="searchResults"
            :columns="[
              { key: 'id', label: 'Id' },
              { key: 'title', label: 'Title' },
              { key: 'price', label: 'Price' },
              { key: 'isAvailable', label: 'Available' }
            ]"
          >
            <template #price-data="{ row }">
              {{ formatMoney(row.price) }}
            </template>
            <template #isAvailable-data="{ row }">
              {{ row.isAvailable ? 'Yes' : 'No' }}
            </template>
            <template #id-data="{ row }">
              <div class="flex items-center gap-2">
                <span class="font-mono text-xs">{{ row.id }}</span>
                <div class="flex gap-1">
                  <UButton
                    size="xs"
                    variant="ghost"
                    @click="addArticleId('create', row.id)"
                  >
                    Add to Create
                  </UButton>
                  <UButton
                    size="xs"
                    variant="ghost"
                    @click="addArticleId('update', row.id)"
                  >
                    Add to Update
                  </UButton>
                </div>
              </div>
            </template>
          </UTable>
        </div>
      </UCard>
    </div>
  </FeatureShell>
</template>
