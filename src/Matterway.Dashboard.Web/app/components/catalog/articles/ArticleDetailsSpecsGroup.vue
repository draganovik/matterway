<script setup lang="ts">
import {
  useCatalogApi,
  type ArticleDetailProperty,
  type ArticleSpecificationProperty,
  type QueryDetailResponse,
  type QuerySpecificationResponse
} from '~/composables/useCatalogApi'
import { useRequestState } from '~/composables/useRequestState'

const props = withDefaults(defineProps<{
  articleId?: string | null
  details?: ArticleDetailProperty[]
  specifications?: ArticleSpecificationProperty[]
  canEdit?: boolean
}>(), {
  articleId: null,
  details: () => [],
  specifications: () => [],
  canEdit: false
})

const emit = defineEmits<{
  (event: 'update:details', value: ArticleDetailProperty[]): void
  (event: 'update:specifications', value: ArticleSpecificationProperty[]): void
}>()

const api = useCatalogApi()

const detailList = ref<ArticleDetailProperty[]>([])
const specList = ref<ArticleSpecificationProperty[]>([])

const definitionState = useRequestState()
const mutateState = useRequestState()
const removeState = useRequestState()

const searchTerm = ref('')
const detailOptions = ref<QueryDetailResponse[]>([])
const specOptions = ref<QuerySpecificationResponse[]>([])
const selectedKey = ref<string>('')
const valueInput = ref<string>('')

const combinedOptions = computed(() => {
  const detailItems = detailOptions.value
    .filter(option => option.slug)
    .map(option => ({
      id: `detail:${option.slug}`,
      slug: option.slug as string,
      label: (option.title ?? option.slug ?? '').toString(),
      type: 'item' as const, // must be "item" for SelectMenuItem
      _mwType: 'detail' as const // custom property to distinguish type
    }))
  const specItems = specOptions.value
    .filter(option => option.slug)
    .map(option => ({
      id: `spec:${option.slug}`,
      slug: option.slug as string,
      label: (option.title ?? option.slug ?? '').toString(),
      unit: option.unit || '',
      type: 'item' as const, // must be "item" for SelectMenuItem
      _mwType: 'spec' as const // custom property to distinguish type
    }))
  return [...detailItems, ...specItems]
})

const selectedOption = computed(() => {
  const match = combinedOptions.value.find(option => option.id === selectedKey.value)
  if (match) return match
  if (!selectedKey.value) return null
  const [mwType, slug] = selectedKey.value.split(':', 2)
  if (!slug) return null
  if (mwType === 'detail') {
    return { id: selectedKey.value, slug, label: slug, type: 'item' as const, _mwType: 'detail' as const }
  }
  if (mwType === 'spec') {
    return { id: selectedKey.value, slug, label: slug, unit: '', type: 'item' as const, _mwType: 'spec' as const }
  }
  return null
})

watch(
  () => props.details,
  (value) => {
    detailList.value = Array.isArray(value) ? [...value] : []
  },
  { immediate: true }
)

watch(
  () => props.specifications,
  (value) => {
    specList.value = Array.isArray(value) ? [...value] : []
  },
  { immediate: true }
)

watch(selectedOption, (option) => {
  if (!option) {
    valueInput.value = ''
    return
  }
  if (option._mwType === 'detail') {
    const existing = detailList.value.find(item => item.detailSlug === option.slug)
    valueInput.value = existing?.value || ''
  } else if (option._mwType === 'spec') {
    const existing = specList.value.find(item => item.specificationSlug === option.slug)
    valueInput.value = existing?.value?.toString() || ''
  }
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

function updateSpecifications(next: ArticleSpecificationProperty[]) {
  specList.value = [...next]
  emit('update:specifications', specList.value)
}

async function loadDefinitions() {
  definitionState.error = ''
  definitionState.success = ''
  definitionState.loading = true
  const titleLike = searchTerm.value.trim() || undefined
  const [detailsResult, specsResult] = await Promise.all([
    api.queryDetails({ limit: 25, titleLike }),
    api.querySpecifications({ limit: 25, titleLike })
  ])
  definitionState.loading = false
  if (!detailsResult.ok || !specsResult.ok) {
    definitionState.error = detailsResult.error || specsResult.error || 'Unable to load definitions.'
    return
  }
  detailOptions.value = detailsResult.data || []
  specOptions.value = specsResult.data || []
  definitionState.success = 'Definitions loaded.'
}

async function applyAssociation() {
  mutateState.error = ''
  mutateState.success = ''
  const option = selectedOption.value
  if (!props.articleId) {
    mutateState.error = 'Create the article before adding details or specifications.'
    return
  }
  if (!option) {
    mutateState.error = 'Select a detail or specification.'
    return
  }
  const rawValue = valueInput.value.trim()
  if (!rawValue) {
    mutateState.error = 'Provide a value.'
    return
  }
  mutateState.loading = true
  if (option._mwType === 'detail') {
    const existing = detailList.value.find(item => item.detailSlug === option.slug)
    if (existing) {
      const result = await api.updateArticleDetail(props.articleId, option.slug, { value: rawValue })
      mutateState.loading = false
      if (!result.ok) {
        mutateState.error = result.error || 'Unable to update detail.'
        return
      }
      updateDetails(detailList.value.map(item =>
        item.detailSlug === option.slug ? { ...item, value: rawValue } : item
      ))
      mutateState.success = 'Detail updated.'
      return
    }
    const addResult = await api.addArticleDetail(props.articleId, {
      detailSlug: option.slug,
      value: rawValue
    })
    mutateState.loading = false
    if (!addResult.ok) {
      mutateState.error = addResult.error || 'Unable to add detail.'
      return
    }
    updateDetails([
      ...detailList.value,
      { detailSlug: option.slug, title: option.label, value: rawValue }
    ])
    mutateState.success = 'Detail added.'
    return
  }

  if (option._mwType === 'spec') {
    const existing = specList.value.find(item => item.specificationSlug === option.slug)
    const numericValue = Number.isNaN(Number(rawValue)) ? rawValue : Number(rawValue)
    if (existing) {
      const result = await api.updateArticleSpecification(props.articleId, option.slug, { value: numericValue })
      mutateState.loading = false
      if (!result.ok) {
        mutateState.error = result.error || 'Unable to update specification.'
        return
      }
      updateSpecifications(specList.value.map(item =>
        item.specificationSlug === option.slug ? { ...item, value: numericValue } : item
      ))
      mutateState.success = 'Specification updated.'
      return
    }
    const addResult = await api.addArticleSpecification(props.articleId, {
      specificationSlug: option.slug,
      value: numericValue
    })
    mutateState.loading = false
    if (!addResult.ok) {
      mutateState.error = addResult.error || 'Unable to add specification.'
      return
    }
    updateSpecifications([
      ...specList.value,
      { specificationSlug: option.slug, title: option.label, unit: option.unit, value: numericValue }
    ])
    mutateState.success = 'Specification added.'
  }
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

async function removeSpecification(spec: ArticleSpecificationProperty) {
  removeState.error = ''
  removeState.success = ''
  if (!props.articleId || !spec.specificationSlug) return
  removeState.loading = true
  const result = await api.removeArticleSpecification(props.articleId, spec.specificationSlug)
  removeState.loading = false
  if (!result.ok) {
    removeState.error = result.error || 'Unable to remove specification.'
    return
  }
  updateSpecifications(specList.value.filter(item => item.specificationSlug !== spec.specificationSlug))
  removeState.success = 'Specification removed.'
}

function editDetail(detail: ArticleDetailProperty) {
  if (!detail.detailSlug) return
  selectedKey.value = `detail:${detail.detailSlug}`
  valueInput.value = detail.value || ''
}

function editSpecification(spec: ArticleSpecificationProperty) {
  if (!spec.specificationSlug) return
  selectedKey.value = `spec:${spec.specificationSlug}`
  valueInput.value = spec.value?.toString() || ''
}
</script>

<template>
  <div class="flex flex-col gap-5">
    <div>
      <h3 class="text-base font-semibold text-foreground">
        Details & Specifications
      </h3>
      <p class="text-sm text-muted">
        Link rich attributes to the article using a single adaptive workflow.
      </p>
    </div>

    <div
      v-if="!articleId"
      class="rounded-lg border border-default bg-background px-4 py-4 text-sm text-muted"
    >
      Create the article first to attach details or specifications.
    </div>

    <div
      v-else
      class="grid gap-4"
    >
      <div class="rounded-lg border border-default bg-background p-4">
        <div class="grid gap-3 md:grid-cols-[minmax(0,1fr)_auto]">
          <UFormField
            label="Detail or Specification"
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
              placeholder="Select detail or specification"
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

        <div class="mt-3 grid gap-3 md:grid-cols-[minmax(0,1fr)_auto]">
          <UFormField
            :label="selectedOption?._mwType === 'spec' ? 'Numeric Value' : 'Value'"
            required
          >
            <UInput
              v-model="valueInput"
              :type="selectedOption?._mwType === 'spec' ? 'number' : 'text'"
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

        <div class="mt-3">
          <FormStatus :error="definitionState.error || mutateState.error" />
          <FormStatus :success="definitionState.success || mutateState.success" />
        </div>
      </div>

      <div class="grid gap-4 md:grid-cols-2">
        <div class="rounded-lg border border-default bg-background p-4">
          <h4 class="text-sm font-semibold text-muted">
            Details
          </h4>
          <div
            v-if="!detailList.length"
            class="mt-2 text-sm text-muted"
          >
            No details attached.
          </div>
          <div
            v-else
            class="mt-3 grid gap-3"
          >
            <div
              v-for="detail in detailList"
              :key="detail.detailSlug ?? detail.title ?? 'detail-unknown'"
              class="flex flex-wrap items-center justify-between gap-3 rounded-md border border-default px-3 py-3"
            >
              <div>
                <p class="text-sm font-medium text-foreground">
                  {{ detail.title || detail.detailSlug }}
                </p>
                <p class="text-sm text-muted">
                  {{ detail.value || 'No value' }}
                </p>
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
        </div>

        <div class="rounded-lg border border-default bg-background p-4">
          <h4 class="text-sm font-semibold text-muted">
            Specifications
          </h4>
          <div
            v-if="!specList.length"
            class="mt-2 text-sm text-muted"
          >
            No specifications attached.
          </div>
          <div
            v-else
            class="mt-3 grid gap-3"
          >
            <div
              v-for="spec in specList"
              :key="spec.specificationSlug ?? spec.title ?? 'spec-unknown'"
              class="flex flex-wrap items-center justify-between gap-3 rounded-md border border-default px-3 py-3"
            >
              <div>
                <p class="text-sm font-medium text-foreground">
                  {{ spec.title || spec.specificationSlug }}
                </p>
                <p class="text-sm text-muted">
                  {{ spec.value ?? 'No value' }} {{ spec.unit || '' }}
                </p>
              </div>
              <div class="flex items-center gap-2">
                <UButton
                  variant="outline"
                  :disabled="!canEdit"
                  @click="editSpecification(spec)"
                >
                  Edit
                </UButton>
                <UButton
                  color="error"
                  variant="ghost"
                  :disabled="!canEdit"
                  @click="removeSpecification(spec)"
                >
                  Remove
                </UButton>
              </div>
            </div>
          </div>
        </div>
      </div>

      <FormStatus
        :error="removeState.error"
        :success="removeState.success"
      />
    </div>
  </div>
</template>
