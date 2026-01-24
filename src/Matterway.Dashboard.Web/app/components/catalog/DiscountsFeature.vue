<script setup lang="ts">
import { useApiClient } from '~/composables/useApiClient'
import { useFeatureTabs } from '~/composables/useFeatureTabs'
import { formatMoney } from '~/utils/format'

const api = useApiClient()
const { active } = useFeatureTabs()

const createForm = reactive({
  code: '',
  percentage: 0.1,
  validFrom: '',
  validTo: '',
  articleIds: [] as string[]
})
const createState = reactive({ loading: false, error: '', success: '' })

const updateForm = reactive({
  code: '',
  percentage: 0.1,
  validFrom: '',
  validTo: '',
  articleIds: [] as string[]
})
const updateState = reactive({ loading: false, error: '', success: '' })

const deleteForm = reactive({
  code: ''
})
const deleteState = reactive({ loading: false, error: '', success: '' })

const searchForm = reactive({
  page: 1,
  pageSize: 10,
  filter: ''
})
const searchState = reactive({ loading: false, error: '', empty: '' })
const searchResults = ref<any[]>([])

function addArticleId(target: 'create' | 'update', id: string) {
  const list = target === 'create' ? createForm.articleIds : updateForm.articleIds
  if (!list.includes(id)) list.push(id)
}

function removeArticleId(target: 'create' | 'update', id: string) {
  const list = target === 'create' ? createForm.articleIds : updateForm.articleIds
  const index = list.indexOf(id)
  if (index >= 0) list.splice(index, 1)
}

function splitArticleIds(value: string) {
  return value
    .split(',')
    .map(item => item.trim())
    .filter(Boolean)
}

async function createDiscount() {
  createState.loading = true
  createState.error = ''
  createState.success = ''
  const payload = {
    code: createForm.code,
    percentage: createForm.percentage,
    validFrom: createForm.validFrom,
    validTo: createForm.validTo || null,
    articleIds: createForm.articleIds
  }
  const result = await api.request<any>('catalog', 'admin/discounts', {
    method: 'POST',
    body: JSON.stringify(payload)
  })
  createState.loading = false
  if (!result.ok) {
    createState.error = result.error || 'Failed to create discounts.'
    return
  }
  createState.success = 'Discounts created.'
}

async function updateDiscount() {
  updateState.loading = true
  updateState.error = ''
  updateState.success = ''
  const payload = {
    percentage: updateForm.percentage,
    validFrom: updateForm.validFrom,
    validTo: updateForm.validTo || null,
    articleIds: updateForm.articleIds
  }
  const result = await api.request<any>('catalog', `admin/discounts/${updateForm.code}`, {
    method: 'PUT',
    body: JSON.stringify(payload)
  })
  updateState.loading = false
  if (!result.ok) {
    updateState.error = result.error || 'Failed to update discounts.'
    return
  }
  updateState.success = 'Discounts updated.'
}

async function deleteDiscount() {
  deleteState.loading = true
  deleteState.error = ''
  deleteState.success = ''
  const result = await api.request<any>('catalog', `admin/discounts/${deleteForm.code}`, {
    method: 'DELETE'
  })
  deleteState.loading = false
  if (!result.ok) {
    deleteState.error = result.error || 'Failed to delete discounts.'
    return
  }
  deleteState.success = 'Discounts removed.'
}

async function searchArticles() {
  searchState.loading = true
  searchState.error = ''
  searchState.empty = ''
  searchResults.value = []
  const params = new URLSearchParams({
    page: searchForm.page.toString(),
    pageSize: searchForm.pageSize.toString()
  })
  if (searchForm.filter) params.set('filter', searchForm.filter)
  const result = await api.request<any>('catalog', `public/articles?${params.toString()}`, {}, true)
  searchState.loading = false
  if (!result.ok) {
    searchState.error = result.error || 'Failed to query articles.'
    return
  }
  const data = result.data?.data ?? []
  if (!data.length) searchState.empty = 'No articles found.'
  searchResults.value = data
}
</script>

<template>
  <FeatureShell>
    <div class="space-y-6">
      <div
        v-if="active === 'create'"
        class="grid gap-6 lg:grid-cols-[2fr_1fr]"
      >
        <UCard class="border border-default">
          <template #header>
            <h2 class="text-lg font-semibold">
              Create discounts
            </h2>
          </template>
          <UForm
            class="space-y-4"
            @submit="createDiscount"
          >
            <UFormField
              label="Code"
              required
            >
              <UInput
                v-model="createForm.code"
                placeholder="SUMMER24"
              />
            </UFormField>
            <div class="grid gap-4 md:grid-cols-2">
              <UFormField
                label="Percentage (0-1)"
                required
              >
                <UInput
                  v-model.number="createForm.percentage"
                  type="number"
                  min="0.01"
                  max="1"
                  step="0.01"
                />
              </UFormField>
              <UFormField
                label="Valid From"
                required
              >
                <UInput
                  v-model="createForm.validFrom"
                  type="datetime-local"
                />
              </UFormField>
            </div>
            <UFormField label="Valid To">
              <UInput
                v-model="createForm.validTo"
                type="datetime-local"
              />
            </UFormField>
            <UFormField
              label="Article Ids"
              required
            >
              <UTextarea
                :rows="2"
                :model-value="createForm.articleIds.join(', ')"
                placeholder="GUID, GUID"
                @update:model-value="(value: string) => (createForm.articleIds = splitArticleIds(value))"
              />
            </UFormField>
            <UButton
              type="submit"
              color="primary"
              :loading="createState.loading"
            >
              Create Discounts
            </UButton>
            <FormStatus
              :error="createState.error"
              :success="createState.success"
            />
          </UForm>
        </UCard>
        <UCard class="border border-default bg-elevated/40">
          <template #header>
            <h3 class="text-sm font-semibold text-muted">
              Selected Articles
            </h3>
          </template>
          <div
            v-if="createForm.articleIds.length"
            class="space-y-2 text-sm"
          >
            <div
              v-for="articleId in createForm.articleIds"
              :key="articleId"
              class="flex items-center justify-between rounded border border-default px-3 py-2"
            >
              <span class="font-mono text-xs">{{ articleId }}</span>
              <UButton
                size="xs"
                color="error"
                variant="ghost"
                @click="removeArticleId('create', articleId)"
              >
                Remove
              </UButton>
            </div>
          </div>
          <p
            v-else
            class="text-sm text-muted"
          >
            Add article IDs to apply the discount.
          </p>
        </UCard>
      </div>

      <div
        v-else-if="active === 'update'"
        class="grid gap-6 lg:grid-cols-[2fr_1fr]"
      >
        <UCard class="border border-default">
          <template #header>
            <h2 class="text-lg font-semibold">
              Update discounts
            </h2>
          </template>
          <UForm
            class="space-y-4"
            @submit="updateDiscount"
          >
            <UFormField
              label="Code"
              required
            >
              <UInput
                v-model="updateForm.code"
                placeholder="SUMMER24"
              />
            </UFormField>
            <div class="grid gap-4 md:grid-cols-2">
              <UFormField
                label="Percentage (0-1)"
                required
              >
                <UInput
                  v-model.number="updateForm.percentage"
                  type="number"
                  min="0.01"
                  max="1"
                  step="0.01"
                />
              </UFormField>
              <UFormField
                label="Valid From"
                required
              >
                <UInput
                  v-model="updateForm.validFrom"
                  type="datetime-local"
                />
              </UFormField>
            </div>
            <UFormField label="Valid To">
              <UInput
                v-model="updateForm.validTo"
                type="datetime-local"
              />
            </UFormField>
            <UFormField
              label="Article Ids"
              required
            >
              <UTextarea
                :rows="2"
                :model-value="updateForm.articleIds.join(', ')"
                placeholder="GUID, GUID"
                @update:model-value="(value: string) => (updateForm.articleIds = splitArticleIds(value))"
              />
            </UFormField>
            <UButton
              type="submit"
              color="primary"
              :loading="updateState.loading"
            >
              Update Discounts
            </UButton>
            <FormStatus
              :error="updateState.error"
              :success="updateState.success"
            />
          </UForm>
        </UCard>
        <UCard class="border border-default bg-elevated/40">
          <template #header>
            <h3 class="text-sm font-semibold text-muted">
              Selected Articles
            </h3>
          </template>
          <div
            v-if="updateForm.articleIds.length"
            class="space-y-2 text-sm"
          >
            <div
              v-for="articleId in updateForm.articleIds"
              :key="articleId"
              class="flex items-center justify-between rounded border border-default px-3 py-2"
            >
              <span class="font-mono text-xs">{{ articleId }}</span>
              <UButton
                size="xs"
                color="error"
                variant="ghost"
                @click="removeArticleId('update', articleId)"
              >
                Remove
              </UButton>
            </div>
          </div>
          <p
            v-else
            class="text-sm text-muted"
          >
            Add article IDs to update.
          </p>
        </UCard>
      </div>

      <div
        v-else-if="active === 'delete'"
        class="grid gap-6 lg:grid-cols-[2fr_1fr]"
      >
        <UCard class="border border-default">
          <template #header>
            <h2 class="text-lg font-semibold">
              Delete discounts
            </h2>
          </template>
          <UForm
            class="space-y-4"
            @submit="deleteDiscount"
          >
            <UFormField
              label="Code"
              required
            >
              <UInput
                v-model="deleteForm.code"
                placeholder="SUMMER24"
              />
            </UFormField>
            <UButton
              type="submit"
              color="error"
              variant="solid"
              :loading="deleteState.loading"
            >
              Delete Discounts
            </UButton>
            <FormStatus
              :error="deleteState.error"
              :success="deleteState.success"
            />
          </UForm>
        </UCard>
        <UCard class="border border-default bg-elevated/40">
          <template #header>
            <h3 class="text-sm font-semibold text-muted">
              Note
            </h3>
          </template>
          <p class="text-sm text-muted">
            Deleting a code removes all associated discounts across article IDs.
          </p>
        </UCard>
      </div>

      <UCard
        v-if="active === 'query'"
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
