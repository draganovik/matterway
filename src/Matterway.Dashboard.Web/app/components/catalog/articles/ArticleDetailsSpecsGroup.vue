<script setup lang="ts">
import {
  useCatalogApi,
  type ArticleDetailProperty,
  type QueryDetailResponse
} from '~/composables/useCatalogApi'
import { useRequestState } from '~/composables/useRequestState'

const props = withDefaults(defineProps<{
  articleId?: string | null
  details?: ArticleDetailProperty[]
  canEdit?: boolean
}>(), {
  articleId: null,
  details: () => [],
  canEdit: false
})

const emit = defineEmits<{
  (event: 'update:details', value: ArticleDetailProperty[]): void
}>()

const api = useCatalogApi()

const detailList = ref<ArticleDetailProperty[]>([])

const definitionState = useRequestState()
const mutateState = useRequestState()
const removeState = useRequestState()

const searchTerm = ref('')
const detailOptions = ref<QueryDetailResponse[]>([])
const selectedKey = ref<string>('')
const valueInput = ref<string>('')

const combinedOptions = computed(() => {
  const detailItems = detailOptions.value
    .filter(option => option.slug)
    .map(option => ({
      id: option.slug as string,
      slug: option.slug as string,
      label: (option.title ?? option.slug ?? '').toString(),
      unit: option.unit || '',
      type: 'item' as const
    }))
  return [...detailItems]
})

const selectedOption = computed(() => {
  const match = combinedOptions.value.find(option => option.id === selectedKey.value)
  if (match) return match
  if (!selectedKey.value) return null
  return { id: selectedKey.value, slug: selectedKey.value, label: selectedKey.value, unit: '', type: 'item' as const }
})

watch(
  () => props.details,
  (value) => {
    detailList.value = Array.isArray(value) ? [...value] : []
  },
  { immediate: true }
)

watch(selectedOption, (option) => {
  if (!option) {
    valueInput.value = ''
    return
  }
  const existing = detailList.value.find(item => item.detailSlug === option.slug)
  valueInput.value =
    existing?.textValue ??
    existing?.numericValue?.toString() ??
    ''
})

watch(
  () => props.articleId,
  (value) => {
    selectedKey.value = ''
    valueInput.value = ''
    searchTerm.value = ''
    if (!value) return
    void loadDefinitions()
  }
)

function updateDetails(next: ArticleDetailProperty[]) {
  detailList.value = [...next]
  emit('update:details', detailList.value)
}

async function loadDefinitions() {
  definitionState.error = ''
  definitionState.success = ''
  definitionState.loading = true
  const titleLike = searchTerm.value.trim() || undefined
  const detailsResult = await api.queryDetails({ limit: 25, titleLike })
  definitionState.loading = false
  if (!detailsResult.ok) {
    definitionState.error = detailsResult.error || 'Unable to load definitions.'
    return
  }
  detailOptions.value = detailsResult.data || []
  definitionState.success = 'Definitions loaded.'
}

async function applyAssociation() {
  mutateState.error = ''
  mutateState.success = ''
  const option = selectedOption.value
  if (!props.articleId) {
    mutateState.error = 'Create the article before adding details.'
    return
  }
  if (!option) {
    mutateState.error = 'Select a detail.'
    return
  }
  const rawValue = valueInput.value.trim()
  const isNumeric = Boolean(option.unit)
  if (!rawValue) {
    mutateState.error = 'Provide a value.'
    return
  }
  mutateState.loading = true
  const existing = detailList.value.find(item => item.detailSlug === option.slug)
  if (isNumeric) {
    const numericValue = Number(rawValue)
    if (!Number.isFinite(numericValue)) {
      mutateState.loading = false
      mutateState.error = 'Numeric value is invalid.'
      return
    }
    if (existing) {
      const result = await api.updateArticleDetail(props.articleId, option.slug, { numericValue })
      mutateState.loading = false
      if (!result.ok) {
        mutateState.error = result.error || 'Unable to update detail.'
        return
      }
      updateDetails(detailList.value.map(item =>
        item.detailSlug === option.slug ? { ...item, numericValue, textValue: null } : item
      ))
      mutateState.success = 'Detail updated.'
      return
    }
    const addResult = await api.addArticleDetail(props.articleId, {
      detailSlug: option.slug,
      numericValue
    })
    mutateState.loading = false
    if (!addResult.ok) {
      mutateState.error = addResult.error || 'Unable to add detail.'
      return
    }
    updateDetails([
      ...detailList.value,
      { detailSlug: option.slug, title: option.label, unit: option.unit, numericValue, textValue: null }
    ])
    mutateState.success = 'Detail added.'
    return
  }

  if (existing) {
    const result = await api.updateArticleDetail(props.articleId, option.slug, { textValue: rawValue })
    mutateState.loading = false
    if (!result.ok) {
      mutateState.error = result.error || 'Unable to update detail.'
      return
    }
    updateDetails(detailList.value.map(item =>
      item.detailSlug === option.slug ? { ...item, textValue: rawValue, numericValue: null } : item
    ))
    mutateState.success = 'Detail updated.'
    return
  }
  const addResult = await api.addArticleDetail(props.articleId, {
    detailSlug: option.slug,
    textValue: rawValue
  })
  mutateState.loading = false
  if (!addResult.ok) {
    mutateState.error = addResult.error || 'Unable to add detail.'
    return
  }
  updateDetails([
    ...detailList.value,
    { detailSlug: option.slug, title: option.label, textValue: rawValue, numericValue: null }
  ])
  mutateState.success = 'Detail added.'
}

async function removeDetail(detail: ArticleDetailProperty) {
  removeState.error = ''
  removeState.success = ''
  if (!props.articleId || !detail.detailSlug) return
  removeState.loading = true
  const result = await api.removeArticleDetail(props.articleId, detail.detailSlug)
  removeState.loading = false
  if (!result.ok) {
    removeState.error = result.error || 'Unable to remove detail.'
    return
  }
  updateDetails(detailList.value.filter(item => item.detailSlug !== detail.detailSlug))
  removeState.success = 'Detail removed.'
}

function editDetail(detail: ArticleDetailProperty) {
  if (!detail.detailSlug) return
  selectedKey.value = detail.detailSlug
  valueInput.value = detail.textValue ?? detail.numericValue?.toString() ?? ''
}
</script>

<template>
  <div class="flex flex-col gap-4">
    <div class="flex items-center justify-between">
      <h3 class="text-base font-semibold text-foreground">
        Details
      </h3>
    </div>

    <div
      v-if="!articleId"
      class="rounded-lg border border-default bg-background px-4 py-4 text-sm text-muted"
    >
      Create the article first to attach details.
    </div>

    <div
      v-else
      class="grid gap-4 lg:grid-cols-2"
    >
      <div class="grid gap-3">
        <div class="grid gap-3 md:grid-cols-2">
          <UFormField
            label="Detail"
            required
          >
            <USelectMenu
              v-model="selectedKey"
              v-model:search-term="searchTerm"
              :items="combinedOptions"
              value-key="id"
              label-key="label"
              :search-input="true"
              :disabled="!canEdit"
              size="md"
              placeholder="Select detail"
            />
          </UFormField>
          <div class="flex items-end">
            <UButton
              variant="outline"
              :loading="definitionState.loading"
              :disabled="!canEdit"
              @click="loadDefinitions"
            >
              Search
            </UButton>
          </div>
        </div>

        <div class="grid gap-3 md:grid-cols-2">
          <UFormField
            :label="selectedOption?.unit ? 'Numeric Value' : 'Value'"
            required
          >
            <UInput
              v-model="valueInput"
              :type="selectedOption?.unit ? 'number' : 'text'"
              :disabled="!canEdit"
              size="md"
            />
          </UFormField>
          <div class="flex items-end">
            <UButton
              color="primary"
              :loading="mutateState.loading"
              :disabled="!canEdit"
              @click="applyAssociation"
            >
              Apply
            </UButton>
          </div>
        </div>

        <FormStatus :error="definitionState.error || mutateState.error" />
        <FormStatus :success="definitionState.success || mutateState.success" />
      </div>

      <div class="grid gap-4">
        <section class="space-y-2">
          <div
            v-if="!detailList.length"
            class="text-sm text-muted"
          >
            No details attached.
          </div>
          <div
            v-else
            class="rounded-md border border-default/40"
          >
            <div
              v-for="detail in detailList"
              :key="detail.detailSlug ?? detail.title ?? 'detail-unknown'"
              class="flex flex-wrap items-center justify-between gap-2 border-t border-default/40 px-3 py-2 first:border-t-0"
            >
              <div class="min-w-0">
                <div class="text-sm font-medium text-foreground">
                  {{ detail.title || detail.detailSlug }}
                </div>
                <div class="text-xs text-muted">
                  {{ detail.textValue ?? detail.numericValue ?? 'No value' }} {{ detail.unit || '' }}
                </div>
              </div>
              <div class="flex items-center gap-2">
                <UButton
                  variant="outline"
                  :disabled="!canEdit"
                  @click="editDetail(detail)"
                >
                  Edit
                </UButton>
                <UButton
                  color="error"
                  variant="ghost"
                  :disabled="!canEdit"
                  @click="removeDetail(detail)"
                >
                  Remove
                </UButton>
              </div>
            </div>
          </div>
        </section>

        <FormStatus
          :error="removeState.error"
          :success="removeState.success"
        />
      </div>
    </div>
  </div>
</template>
