<script setup lang="ts">
import { useCatalogClient } from "~/composables/api/useCatalogClient"
import type { GetArticleResponse } from "~/types/catalog"
import { useRequestState } from "~/composables/workflows/state/useRequestState"

const props = withDefaults(
  defineProps<{
    article?: GetArticleResponse | null
    error?: string
    canEdit?: boolean
  }>(),
  {
    article: null,
    error: "",
    canEdit: false,
  },
)

const emit = defineEmits<{
  (event: "update:article", value: GetArticleResponse | null): void
  (event: "updated", value: GetArticleResponse): void
}>()

const api = useCatalogClient()

const updateState = useRequestState()

type ArticleForm = {
  code: string
  title: string
  basePrice: number | string
  description: string
  isAvailable: boolean
}

const form = ref<ArticleForm>({
  code: "",
  title: "",
  basePrice: "",
  description: "",
  isAvailable: true,
})

watch(
  () => props.article,
  (article) => {
    if (!article) return
    form.value = {
      code: article.code || "",
      title: article.title || "",
      basePrice: article.basePrice ?? "",
      description: article.description || "",
      isAvailable: article.isAvailable,
    }
    updateState.error = ""
    updateState.success = ""
  },
  { immediate: true },
)

function updateArticleData(patch: Partial<GetArticleResponse>) {
  if (!props.article) return
  const next = { ...props.article, ...patch }
  emit("update:article", next)
  emit("updated", next)
}

async function saveArticle() {
  updateState.error = ""
  updateState.success = ""
  if (!props.article) return
  if (!props.canEdit) return

  const code = String(form.value.code ?? "").trim().toUpperCase()
  const title = String(form.value.title ?? "").trim()
  const description = String(form.value.description ?? "").trim()
  if (!/^[A-Z0-9]{8}$/.test(code)) {
    updateState.error = "Article code must be exactly 8 letters or numbers."
    return
  }

  form.value.code = code
  form.value.title = title
  form.value.description = description
  updateState.loading = true
  const result = await api.updateArticle(props.article.code, {
    code,
    title: title || null,
    basePrice: form.value.basePrice || null,
    description: description || null,
    isAvailable: form.value.isAvailable,
  })
  updateState.loading = false
  if (!result.ok) {
    updateState.error = result.error || "Unable to update article."
    return
  }
  updateArticleData({
    code: result.data?.code ?? form.value.code,
    title: result.data?.title ?? form.value.title,
    basePrice: result.data?.basePrice ?? form.value.basePrice,
    description: result.data?.description ?? form.value.description,
    isAvailable: result.data?.isAvailable ?? form.value.isAvailable,
    updatedAt: result.data?.updatedAt ?? props.article.updatedAt,
  })
  updateState.success = "Article updated."
}

function updateDetails(details: GetArticleResponse["details"]) {
  updateArticleData({ details })
}

function updateImages(images: GetArticleResponse["images"]) {
  updateArticleData({ images })
}
</script>

<template>
  <div class="flex flex-col gap-5">
    <StatusMessages v-if="error" :error="error" />

    <div v-else-if="!article" class="space-y-4">
      <div class="space-y-1">
        <h3 class="text-foreground text-base font-semibold">Article Editor</h3>
        <p class="text-muted text-sm">
          {{
            canEdit
              ? "Operator permission is required for create, update, and delete."
              : "Read-only mode: operator permission required for changes."
          }}
        </p>
      </div>

      <EntitiesEmptyState
        title="Nothing selected"
        description="Select an item from the list to start editing."
      />
    </div>

    <div v-else class="grid gap-5">
      <section>
        <div class="flex flex-wrap items-start justify-between gap-3">
          <div>
            <h3 class="text-foreground text-base font-semibold">
              Article Fields
            </h3>
            <p class="text-muted text-sm">
              Update primary details and availability.
            </p>
          </div>
          <div class="text-muted text-sm">
            Article Code: {{ article?.code }}
          </div>
        </div>

        <div class="mt-4">
          <CatalogArticlesInformationForm v-model="form" :disabled="!canEdit" />
        </div>

        <div class="mt-4 flex flex-wrap items-center gap-3">
          <UButton
            size="lg"
            color="primary"
            :loading="updateState.loading"
            :disabled="!canEdit"
            @click="saveArticle"
          >
            Save Changes
          </UButton>
          <StatusMessages
            :error="updateState.error"
            :success="updateState.success"
          />
        </div>
      </section>

      <section class="space-y-3">
        <CatalogArticlesImagesPanel
          :code="article?.code ?? null"
          :model-value="article?.images || []"
          :can-edit="canEdit"
          @update:model-value="updateImages"
        />
      </section>

      <section class="space-y-3">
        <CatalogArticlesDetailsPanel
          :code="article?.code ?? null"
          :details="article?.details || []"
          :can-edit="canEdit"
          @update:details="updateDetails"
        />
      </section>
    </div>
  </div>
</template>
