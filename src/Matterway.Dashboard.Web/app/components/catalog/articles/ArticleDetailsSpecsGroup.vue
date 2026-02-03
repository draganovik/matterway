<script setup lang="ts">
import { useCatalogApi, type ArticleDetailProperty } from '~/composables/useCatalogApi'
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
const mutateState = useRequestState()
const removeState = useRequestState()
const detailModalOpen = ref(false)
const detailModalMode = ref<'add' | 'edit'>('add')
const activeDetail = ref<ArticleDetailProperty | null>(null)

watch(
  () => props.details,
  (value) => {
    detailList.value = Array.isArray(value) ? [...value] : []
  },
  { immediate: true }
)

function updateDetails(next: ArticleDetailProperty[]) {
  detailList.value = [...next]
  emit('update:details', detailList.value)
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

function openAddDetailModal() {
  mutateState.error = ''
  mutateState.success = ''
  if (!props.articleId) {
    mutateState.error = 'Create the article before adding details.'
    return
  }
  detailModalMode.value = 'add'
  activeDetail.value = null
  detailModalOpen.value = true
}

function openEditDetailModal(detail: ArticleDetailProperty) {
  mutateState.error = ''
  mutateState.success = ''
  detailModalMode.value = 'edit'
  activeDetail.value = detail
  detailModalOpen.value = true
}

async function handleDetailSubmit(payload: {
  detailSlug: string
  title?: string | null
  unit?: string | null
  textValue?: string | null
  numericValue?: number | null
}) {
  mutateState.error = ''
  mutateState.success = ''
  if (!props.articleId) {
    mutateState.error = 'Create the article before adding details.'
    return
  }

  mutateState.loading = true
  const existing = detailList.value.find(item => item.detailSlug === payload.detailSlug)
  if (detailModalMode.value === 'edit' && activeDetail.value?.detailSlug) {
    const targetSlug = activeDetail.value.detailSlug
    const result = await api.updateArticleDetail(props.articleId, targetSlug, {
      textValue: payload.textValue ?? undefined,
      numericValue: payload.numericValue ?? undefined
    })
    mutateState.loading = false
    if (!result.ok) {
      mutateState.error = result.error || 'Unable to update detail.'
      return
    }
    updateDetails(detailList.value.map(item =>
      item.detailSlug === targetSlug
        ? {
            ...item,
            textValue: payload.textValue ?? null,
            numericValue: payload.numericValue ?? null
          }
        : item
    ))
    mutateState.success = 'Detail updated.'
    detailModalOpen.value = false
    return
  }

  if (existing) {
    mutateState.loading = false
    mutateState.error = 'Detail already exists.'
    return
  }
  const addResult = await api.addArticleDetail(props.articleId, {
    detailSlug: payload.detailSlug,
    textValue: payload.textValue ?? undefined,
    numericValue: payload.numericValue ?? undefined
  })
  mutateState.loading = false
  if (!addResult.ok) {
    mutateState.error = addResult.error || 'Unable to add detail.'
    return
  }
  updateDetails([
    ...detailList.value,
    {
      detailSlug: payload.detailSlug,
      title: payload.title ?? payload.detailSlug,
      unit: payload.unit ?? null,
      textValue: payload.textValue ?? null,
      numericValue: payload.numericValue ?? null
    }
  ])
  mutateState.success = 'Detail added.'
  detailModalOpen.value = false
}
</script>

<template>
  <div class="flex flex-col gap-4">
    <div class="flex items-center justify-between">
      <h3 class="text-base font-semibold text-foreground">
        Details
      </h3>
      <UButton
        color="primary"
        variant="outline"
        :disabled="!canEdit || !articleId"
        @click="openAddDetailModal"
      >
        Add Detail
      </UButton>
    </div>

    <div
      v-if="!articleId"
      class="rounded-lg border border-default bg-background px-4 py-4 text-sm text-muted"
    >
      Create the article first to attach details.
    </div>

    <div
      v-else
      class="grid gap-4"
    >
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
                @click="openEditDetailModal(detail)"
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
        :error="mutateState.error || removeState.error"
        :success="mutateState.success || removeState.success"
      />
    </div>
  </div>

  <ArticleDetailModal
    v-model:open="detailModalOpen"
    :mode="detailModalMode"
    :detail="activeDetail"
    :can-edit="canEdit"
    :loading="mutateState.loading"
    :error="mutateState.error"
    @submit="handleDetailSubmit"
  />
</template>
