<script setup lang="ts">
import { useApiClient } from '~/composables/useApiClient'
import { useFeatureTabs } from '~/composables/useFeatureTabs'
import { formatMoney } from '~/utils/format'

const api = useApiClient()
const { active } = useFeatureTabs()

const createForm = reactive({
  articleCode: '',
  title: '',
  basePrice: 0,
  description: '',
  isAvailable: true
})
const createState = reactive({ loading: false, error: '', success: '' })

const updateForm = reactive({
  id: '',
  articleCode: '',
  title: '',
  basePrice: null as number | null,
  description: '',
  isAvailable: true
})
const updateState = reactive({ loading: false, error: '', success: '' })
const updatePreview = ref<any | null>(null)

const deleteForm = reactive({
  id: ''
})
const deleteState = reactive({ loading: false, error: '', success: '' })
const deletePreview = ref<any | null>(null)

const searchForm = reactive({
  page: 1,
  pageSize: 10,
  filter: ''
})
const searchState = reactive({ loading: false, error: '', empty: '' })
const searchResults = ref<any[]>([])

async function createArticle() {
  createState.loading = true
  createState.error = ''
  createState.success = ''
  const payload = {
    articleCode: createForm.articleCode,
    title: createForm.title,
    basePrice: createForm.basePrice,
    description: createForm.description,
    isAvailable: createForm.isAvailable
  }
  const result = await api.request<any>('catalog', 'admin/articles', {
    method: 'POST',
    body: JSON.stringify(payload)
  })
  createState.loading = false
  if (!result.ok) {
    createState.error = result.error || 'Failed to create article.'
    return
  }
  createState.success = 'Article created.'
}

async function loadArticle(target: 'update' | 'delete') {
  const id = target === 'update' ? updateForm.id : deleteForm.id
  if (!id) return
  const result = await api.request<any>('catalog', `public/articles/${id}`, {}, true)
  if (!result.ok || !result.data) {
    if (target === 'update') updateState.error = result.error || 'Article not found.'
    else deleteState.error = result.error || 'Article not found.'
    return
  }
  if (target === 'update') {
    updatePreview.value = result.data
    updateForm.articleCode = result.data.code || ''
    updateForm.title = result.data.title || ''
    updateForm.basePrice = result.data.basePrice ?? null
    updateForm.description = result.data.description || ''
    updateForm.isAvailable = !!result.data.isAvailable
  } else {
    deletePreview.value = result.data
  }
}

async function updateArticle() {
  if (!updateForm.id) {
    updateState.error = 'Article Id is required.'
    return
  }
  updateState.loading = true
  updateState.error = ''
  updateState.success = ''
  const payload: Record<string, any> = {
    isAvailable: updateForm.isAvailable
  }
  if (updateForm.articleCode) payload.articleCode = updateForm.articleCode
  if (updateForm.title) payload.title = updateForm.title
  if (updateForm.description) payload.description = updateForm.description
  if (updateForm.basePrice !== null) payload.basePrice = updateForm.basePrice

  const result = await api.request<any>('catalog', `admin/articles/${updateForm.id}`, {
    method: 'PATCH',
    body: JSON.stringify(payload)
  })
  updateState.loading = false
  if (!result.ok) {
    updateState.error = result.error || 'Failed to update article.'
    return
  }
  updateState.success = 'Article updated.'
  updatePreview.value = result.data
}

async function deleteArticle() {
  if (!deleteForm.id) {
    deleteState.error = 'Article Id is required.'
    return
  }
  deleteState.loading = true
  deleteState.error = ''
  deleteState.success = ''
  const result = await api.request<any>('catalog', `admin/articles/${deleteForm.id}`, {
    method: 'DELETE'
  })
  deleteState.loading = false
  if (!result.ok) {
    deleteState.error = result.error || 'Failed to delete article.'
    return
  }
  deleteState.success = 'Article deleted.'
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
  if (!data.length) {
    searchState.empty = 'No articles found.'
  }
  searchResults.value = data
}
</script>

<template>
  <FeatureShell>
    <div class="space-y-6">
      <div
        v-if="active === 'query'"
        class="space-y-4"
      >
        <UCard class="border border-default">
          <template #header>
            <div>
              <h3 class="text-lg font-semibold">
                Find articles
              </h3>
              <p class="text-sm text-muted">
                Use the public catalog to locate article IDs.
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
                { key: 'code', label: 'Code' },
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
                  <UButton
                    size="xs"
                    color="primary"
                    variant="ghost"
                    @click="updateForm.id = row.id"
                  >
                    Use
                  </UButton>
                </div>
              </template>
            </UTable>
          </div>
        </UCard>
      </div>

      <div
        v-else-if="active === 'create'"
        class="grid gap-6 lg:grid-cols-[2fr_1fr]"
      >
        <UCard class="border border-default">
          <template #header>
            <div>
              <h2 class="text-lg font-semibold">
                Create article
              </h2>
              <p class="text-sm text-muted">
                Operator permission required.
              </p>
            </div>
          </template>
          <UForm
            class="space-y-4"
            @submit="createArticle"
          >
            <div class="grid gap-4 md:grid-cols-2">
              <UFormField
                label="Article Code"
                required
              >
                <UInput
                  v-model="createForm.articleCode"
                  placeholder="ABC123"
                />
              </UFormField>
              <UFormField
                label="Base Price (RSD)"
                required
              >
                <UInput
                  v-model.number="createForm.basePrice"
                  type="number"
                  min="0.01"
                  step="0.01"
                />
              </UFormField>
            </div>
            <UFormField
              label="Title"
              required
            >
              <UInput
                v-model="createForm.title"
                placeholder="Router Pro X"
              />
            </UFormField>
            <UFormField
              label="Description"
              required
            >
              <UTextarea
                v-model="createForm.description"
                :rows="4"
              />
            </UFormField>
            <UFormField label="Available">
              <USwitch v-model="createForm.isAvailable" />
            </UFormField>
            <UButton
              type="submit"
              color="primary"
              :loading="createState.loading"
            >
              Create Article
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
              Tips
            </h3>
          </template>
          <ul class="space-y-2 text-sm text-muted">
            <li>Article codes must be 5-10 uppercase letters or numbers.</li>
            <li>Base price must be greater than 0.</li>
            <li>Availability controls public storefront visibility.</li>
          </ul>
        </UCard>
      </div>

      <div
        v-else-if="active === 'update'"
        class="space-y-6"
      >
        <UCard class="border border-default">
          <template #header>
            <div>
              <h2 class="text-lg font-semibold">
                Update article
              </h2>
              <p class="text-sm text-muted">
                Load an article, modify fields, then update.
              </p>
            </div>
          </template>
          <div class="grid gap-6 lg:grid-cols-[2fr_1fr]">
            <UForm
              class="space-y-4"
              @submit="updateArticle"
            >
              <div class="flex gap-3">
                <UFormField
                  label="Article Id"
                  required
                  class="flex-1"
                >
                  <UInput
                    v-model="updateForm.id"
                    placeholder="GUID"
                  />
                </UFormField>
                <UButton
                  color="neutral"
                  variant="outline"
                  class="self-end"
                  @click="loadArticle('update')"
                >
                  Load
                </UButton>
              </div>
              <div class="grid gap-4 md:grid-cols-2">
                <UFormField label="Article Code">
                  <UInput
                    v-model="updateForm.articleCode"
                    placeholder="ABC123"
                  />
                </UFormField>
                <UFormField label="Base Price (RSD)">
                  <UInput
                    v-model.number="updateForm.basePrice"
                    type="number"
                    min="0.01"
                    step="0.01"
                  />
                </UFormField>
              </div>
              <UFormField label="Title">
                <UInput
                  v-model="updateForm.title"
                  placeholder="Router Pro X"
                />
              </UFormField>
              <UFormField label="Description">
                <UTextarea
                  v-model="updateForm.description"
                  :rows="4"
                />
              </UFormField>
              <UFormField label="Available">
                <USwitch v-model="updateForm.isAvailable" />
              </UFormField>
              <UButton
                type="submit"
                color="primary"
                :loading="updateState.loading"
              >
                Update Article
              </UButton>
              <FormStatus
                :error="updateState.error"
                :success="updateState.success"
              />
            </UForm>
            <UCard class="border border-default bg-elevated/40">
              <template #header>
                <h3 class="text-sm font-semibold text-muted">
                  Loaded article
                </h3>
              </template>
              <div
                v-if="updatePreview"
                class="space-y-2 text-sm"
              >
                <p class="font-semibold">
                  {{ updatePreview.title }}
                </p>
                <p class="text-muted">
                  Code: {{ updatePreview.code }}
                </p>
                <p class="text-muted">
                  Price: {{ formatMoney(updatePreview.price) }}
                </p>
                <p class="text-muted">
                  Available: {{ updatePreview.isAvailable ? 'Yes' : 'No' }}
                </p>
              </div>
              <p
                v-else
                class="text-sm text-muted"
              >
                Load an article to preview.
              </p>
            </UCard>
          </div>
        </UCard>
      </div>

      <div
        v-else-if="active === 'delete'"
        class="grid gap-6 lg:grid-cols-[2fr_1fr]"
      >
        <UCard class="border border-default">
          <template #header>
            <div>
              <h2 class="text-lg font-semibold">
                Delete article
              </h2>
              <p class="text-sm text-muted">
                Removes the article and associated images.
              </p>
            </div>
          </template>
          <UForm
            class="space-y-4"
            @submit="deleteArticle"
          >
            <div class="flex gap-3">
              <UFormField
                label="Article Id"
                required
                class="flex-1"
              >
                <UInput
                  v-model="deleteForm.id"
                  placeholder="GUID"
                />
              </UFormField>
              <UButton
                color="neutral"
                variant="outline"
                class="self-end"
                @click="loadArticle('delete')"
              >
                Load
              </UButton>
            </div>
            <UButton
              type="submit"
              color="error"
              variant="solid"
              :loading="deleteState.loading"
            >
              Delete Article
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
              Article preview
            </h3>
          </template>
          <div
            v-if="deletePreview"
            class="space-y-2 text-sm"
          >
            <p class="font-semibold">
              {{ deletePreview.title }}
            </p>
            <p class="text-muted">
              Code: {{ deletePreview.code }}
            </p>
            <p class="text-muted">
              Price: {{ formatMoney(deletePreview.price) }}
            </p>
          </div>
          <p
            v-else
            class="text-sm text-muted"
          >
            Load an article to confirm deletion.
          </p>
        </UCard>
      </div>
    </div>
  </FeatureShell>
</template>
