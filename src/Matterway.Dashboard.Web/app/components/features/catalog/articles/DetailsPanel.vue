<script setup lang="ts">
import { useCatalogClient } from "~/composables/api/useCatalogClient"
import type { ArticleDetailProperty } from "~/types/catalog"
import { useRequestState } from "~/composables/workflows/state/useRequestState"

const props = withDefaults(
  defineProps<{
    code?: string | null
    details?: ArticleDetailProperty[]
    canEdit?: boolean
  }>(),
  {
    code: null,
    details: () => [],
    canEdit: false,
  },
)

const emit = defineEmits<{
  (event: "update:details", value: ArticleDetailProperty[]): void
}>()

const api = useCatalogClient()

const detailList = ref<ArticleDetailProperty[]>([])
const mutateState = useRequestState()
const removeState = useRequestState()
const detailModalOpen = ref(false)
const detailModalMode = ref<"add" | "edit">("add")
const activeDetail = ref<ArticleDetailProperty | null>(null)

watch(
  () => props.details,
  (value) => {
    detailList.value = Array.isArray(value) ? [...value] : []
  },
  { immediate: true },
)

function updateDetails(next: ArticleDetailProperty[]) {
  detailList.value = [...next]
  emit("update:details", detailList.value)
}

async function removeDetail(detail: ArticleDetailProperty) {
  removeState.error = ""
  removeState.success = ""
  if (!props.code || !detail.detailSlug) return
  removeState.loading = true
  const result = await api.removeArticleDetail(props.code, detail.detailSlug)
  removeState.loading = false
  if (!result.ok) {
    removeState.error = result.error || "Uklanjanje detalja nije uspelo."
    return
  }
  updateDetails(
    detailList.value.filter((item) => item.detailSlug !== detail.detailSlug),
  )
  removeState.success = "Detalj je uspešno uklonjen."
}

function openAddDetailsModal() {
  mutateState.error = ""
  mutateState.success = ""
  if (!props.code) {
    mutateState.error = "Najpre sačuvajte artikal da biste dodali detalje."
    return
  }
  detailModalMode.value = "add"
  activeDetail.value = null
  detailModalOpen.value = true
}

function openEditDetailsModal(detail: ArticleDetailProperty) {
  mutateState.error = ""
  mutateState.success = ""
  detailModalMode.value = "edit"
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
  mutateState.error = ""
  mutateState.success = ""
  if (!props.code) {
    mutateState.error = "Najpre sačuvajte artikal da biste dodali detalje."
    return
  }

  mutateState.loading = true
  const existing = detailList.value.find(
    (item) => item.detailSlug === payload.detailSlug,
  )
  if (detailModalMode.value === "edit" && activeDetail.value?.detailSlug) {
    const targetSlug = activeDetail.value.detailSlug
    const result = await api.updateArticleDetail(props.code, targetSlug, {
      textValue: payload.textValue ?? undefined,
      numericValue: payload.numericValue ?? undefined,
    })
    mutateState.loading = false
    if (!result.ok) {
      mutateState.error = result.error || "Ažuriranje detalja nije uspelo."
      return
    }
    updateDetails(
      detailList.value.map((item) =>
        item.detailSlug === targetSlug
          ? {
              ...item,
              textValue: payload.textValue ?? null,
              numericValue: payload.numericValue ?? null,
            }
          : item,
      ),
    )
    mutateState.success = "Detalj je uspešno ažuriran."
    detailModalOpen.value = false
    return
  }

  if (existing) {
    mutateState.loading = false
    mutateState.error = "Detalj već postoji."
    return
  }
  const addResult = await api.addArticleDetail(props.code, {
    detailSlug: payload.detailSlug,
    textValue: payload.textValue ?? undefined,
    numericValue: payload.numericValue ?? undefined,
  })
  mutateState.loading = false
  if (!addResult.ok) {
    mutateState.error = addResult.error || "Dodavanje detalja nije uspelo."
    return
  }
  updateDetails([
    ...detailList.value,
    {
      detailSlug: payload.detailSlug,
      title: payload.title ?? payload.detailSlug,
      unit: payload.unit ?? null,
      textValue: payload.textValue ?? null,
      numericValue: payload.numericValue ?? null,
    },
  ])
  mutateState.success = "Detalj je uspešno dodat."
  detailModalOpen.value = false
}
</script>

<template>
  <div class="flex flex-col gap-4">
    <div class="flex items-center justify-between">
      <h3 class="text-foreground text-base font-semibold">Detalji</h3>
      <UButton
        color="primary"
        variant="outline"
        :disabled="!canEdit || !code"
        @click="openAddDetailsModal"
      >
        Dodaj detalj
      </UButton>
    </div>

    <div
      v-if="!code"
      class="border-default bg-background text-muted rounded-lg border px-4 py-4 text-sm"
    >
      Najpre sačuvajte artikal da biste dodali detalje.
    </div>

    <div v-else class="grid gap-4">
      <section class="space-y-2">
        <div v-if="!detailList.length" class="text-muted text-sm">
          Nema dodatih detalja.
        </div>
        <div v-else class="border-default/40 rounded-md border">
          <div
            v-for="detail in detailList"
            :key="detail.detailSlug ?? detail.title ?? 'detail-unknown'"
            class="border-default/40 flex flex-wrap items-center justify-between gap-2 border-t px-3 py-2 first:border-t-0"
          >
            <div class="min-w-0">
              <div class="text-foreground text-sm font-medium">
                {{ detail.title || detail.detailSlug }}
              </div>
              <div class="text-muted text-xs">
                {{ detail.textValue ?? detail.numericValue ?? "Bez vrednosti" }}
                {{ detail.unit || "" }}
              </div>
            </div>
            <div class="flex items-center gap-2">
              <UButton
                variant="outline"
                :disabled="!canEdit"
                @click="openEditDetailsModal(detail)"
              >
                Izmeni
              </UButton>
              <UButton
                color="error"
                variant="ghost"
                :disabled="!canEdit"
                @click="removeDetail(detail)"
              >
                Ukloni
              </UButton>
            </div>
          </div>
        </div>
      </section>

      <StatusMessages
        :error="mutateState.error || removeState.error"
        :success="mutateState.success || removeState.success"
      />
    </div>
  </div>

  <CatalogArticlesModalDetailView
    v-model:open="detailModalOpen"
    :mode="detailModalMode"
    :detail="activeDetail"
    :can-edit="canEdit"
    :loading="mutateState.loading"
    :error="mutateState.error"
    @submit="handleDetailSubmit"
  />
</template>
