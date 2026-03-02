<script setup lang="ts">
import { useCatalogApi } from "~/composables/useCatalogApi"
import type { QueryDetailResponse } from "~/types/catalog"
import { useRequestState } from "~/composables/useRequestState"
import { useAuthSession } from "~/composables/useAuthSession"
import { normalizeSlug } from "~/utils/normalization"

type DetailForm = {
  slug: string
  title: string
  unit: string
}

const auth = useAuthSession()
const api = useCatalogApi()

const canEdit = computed(() =>
  auth.hasPermission("catalog", ["operator", "manager"]),
)

const listState = useRequestState({ empty: "No detail definitions found." })
const saveState = useRequestState()
const removeState = useRequestState()

const details = ref<QueryDetailResponse[]>([])
const filter = ref("")
const limit = ref(20)

const selectedSlug = ref<string | null>(null)
const selectedDetail = ref<QueryDetailResponse | null>(null)
const createModalOpen = ref(false)

const form = ref<DetailForm>({
  slug: "",
  title: "",
  unit: "",
})

const canDelete = computed(() => Boolean(selectedSlug.value))

function sortDetails(items: QueryDetailResponse[]) {
  return [...items].sort((a, b) => {
    const left = (a.title || a.slug || "").toLowerCase()
    const right = (b.title || b.slug || "").toLowerCase()
    return left.localeCompare(right)
  })
}

function applyDetailToForm(detail: QueryDetailResponse | null) {
  if (!detail) {
    form.value = {
      slug: "",
      title: "",
      unit: "",
    }
    return
  }

  form.value = {
    slug: detail.slug || "",
    title: detail.title || "",
    unit: detail.unit || "",
  }
}

function resetMessages() {
  saveState.error = ""
  saveState.success = ""
  removeState.error = ""
  removeState.success = ""
}

async function loadDetails() {
  listState.loading = true
  listState.error = ""

  const result = await api.queryDetails({
    limit: Math.min(50, Math.max(1, limit.value)),
    titleLike: filter.value.trim() || undefined,
  })

  listState.loading = false

  if (!result.ok) {
    listState.error = result.error || "Unable to load detail definitions."
    details.value = []
    return
  }

  details.value = sortDetails(result.data || [])

  if (selectedSlug.value) {
    const match =
      details.value.find((item) => item.slug === selectedSlug.value) || null
    selectedDetail.value = match
    if (!match) {
      selectedSlug.value = null
      applyDetailToForm(null)
      return
    }
    applyDetailToForm(match)
  }
}

function beginCreate() {
  resetMessages()
  createModalOpen.value = true
}

function selectDetail(slug: string) {
  const match = details.value.find((item) => item.slug === slug)
  if (!match) return

  selectedSlug.value = slug
  selectedDetail.value = match
  applyDetailToForm(match)
  resetMessages()
}

function searchDetails() {
  void loadDetails()
}

function updateLimit(value: number) {
  const next = Math.min(50, Math.max(1, Number(value) || 20))
  if (next === limit.value) return
  limit.value = next
  void loadDetails()
}

async function saveDetail() {
  resetMessages()

  if (!canEdit.value) return

  const slug = normalizeSlug(selectedSlug.value || "")
  const title = form.value.title.trim()
  const unit = form.value.unit.trim()

  if (!slug) {
    saveState.error = "Select a detail to update."
    return
  }
  if (!title) {
    saveState.error = "Title is required."
    return
  }

  saveState.loading = true

  const result = await api.putDetail(slug, {
    title,
    unit: unit || null,
  })

  saveState.loading = false

  if (!result.ok) {
    saveState.error = result.error || "Unable to save detail."
    return
  }

  const nextDetail: QueryDetailResponse = {
    slug: result.data?.slug ?? slug,
    title: result.data?.title ?? title,
    unit: result.data?.unit ?? (unit || null),
  }

  const withoutCurrent = details.value.filter(
    (item) => item.slug !== nextDetail.slug,
  )
  details.value = sortDetails([...withoutCurrent, nextDetail])

  selectedSlug.value = nextDetail.slug || slug
  selectedDetail.value = nextDetail
  applyDetailToForm(nextDetail)

  saveState.success = "Detail updated."
}

async function removeDetail() {
  resetMessages()

  if (!canEdit.value) return

  const slug = selectedSlug.value || normalizeSlug(form.value.slug)
  if (!slug) {
    removeState.error = "Select a detail to delete."
    return
  }

  removeState.loading = true
  const result = await api.deleteDetail(slug)
  removeState.loading = false

  if (!result.ok) {
    removeState.error = result.error || "Unable to delete detail."
    return
  }

  details.value = details.value.filter((item) => item.slug !== slug)
  selectedSlug.value = null
  selectedDetail.value = null
  applyDetailToForm(null)

  removeState.success = result.data?.message || "Detail removed."
}

function handleDetailCreated(detail: QueryDetailResponse) {
  const next = details.value.filter((item) => item.slug !== detail.slug)
  details.value = sortDetails([...next, detail])

  const slug = detail.slug || ""
  if (!slug) return

  selectedSlug.value = slug
  selectedDetail.value = detail
  applyDetailToForm(detail)
}

onMounted(() => {
  void loadDetails()
})
</script>

<template>
  <div class="flex h-full min-h-0 flex-col gap-4 overflow-hidden">
    <div class="flex shrink-0 flex-wrap items-center justify-between gap-3">
      <div>
        <h2 class="text-foreground text-base font-semibold">Manage Details</h2>
        <p class="text-muted text-sm">
          Create and maintain detail definitions used across article detail
          values.
        </p>
      </div>

      <UButton color="primary" :disabled="!canEdit" @click="beginCreate">
        Create New
      </UButton>
    </div>

    <EntitiesSplitView
      class="min-h-0 flex-1"
      list-class="overflow-y-auto"
      detail-class="overflow-y-auto"
    >
      <template #list>
        <EntitiesListPanel
          title="Detail Definitions"
          description="Search by title and select one to update or remove."
          :items="details"
          item-key="slug"
          item-title-key="title"
          item-subtitle-key="slug"
          :selected-id="selectedSlug"
          :filter="filter"
          filter-input-type="input"
          filter-placeholder="Search detail definitions by title (e.g. screen size)."
          :loading="listState.loading"
          :error="listState.error"
          :empty-message="listState.empty"
          :page="1"
          :page-size="limit"
          :total-count="details.length"
          :total-pages="1"
          @update:filter="(value) => (filter = value)"
          @search="searchDetails"
          @update:page-size="updateLimit"
          @select="selectDetail"
        >
          <template #item="{ item }">
            <CatalogDetailsListItem :item="item" />
          </template>
        </EntitiesListPanel>
      </template>

      <template #detail>
        <CatalogDetailsPanelInformationView
          :detail="selectedDetail"
          :can-edit="canEdit"
          :can-delete="canDelete"
          :slug="form.slug"
          :title="form.title"
          :unit="form.unit"
          :save-loading="saveState.loading"
          :remove-loading="removeState.loading"
          :error="saveState.error || removeState.error"
          :success="saveState.success || removeState.success"
          @update:slug="(value) => (form.slug = value)"
          @update:title="(value) => (form.title = value)"
          @update:unit="(value) => (form.unit = value)"
          @save="saveDetail"
          @remove="removeDetail"
        />
      </template>
    </EntitiesSplitView>
  </div>

  <CatalogDetailsModalDetailManager
    v-model:open="createModalOpen"
    :can-edit="canEdit"
    @created="handleDetailCreated"
  />
</template>
