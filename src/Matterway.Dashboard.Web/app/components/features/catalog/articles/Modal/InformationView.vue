<script setup lang="ts">
import { useCatalogClient } from "~/composables/api/useCatalogClient"
import type { CreateArticleResponse } from "~/types/catalog"
import { useRequestState } from "~/composables/workflows/state/useRequestState"
import { useResetOnModalOpen } from "~/composables/workflows/modal/useResetOnModalOpen"

type ArticleBaseForm = {
  code: string
  title: string
  basePrice: number | string
  description: string
  isAvailable: boolean
}

const { canEdit = false } = defineProps<{
  canEdit?: boolean
}>()

const emit = defineEmits<{
  created: [article: CreateArticleResponse]
}>()

const isOpen = defineModel<boolean>("open", { required: true })

const api = useCatalogClient()
const createState = useRequestState()

const form = ref<ArticleBaseForm>({
  code: "",
  title: "",
  basePrice: "",
  description: "",
  isAvailable: true,
})

function resetForm() {
  form.value = {
    code: "",
    title: "",
    basePrice: "",
    description: "",
    isAvailable: true,
  }
  createState.error = ""
}

useResetOnModalOpen(isOpen, resetForm)

async function createArticle() {
  createState.error = ""
  if (!canEdit) return

  const payload = form.value
  const code = String(payload.code ?? "").trim().toUpperCase()
  const title = String(payload.title ?? "").trim()
  const description = String(payload.description ?? "").trim()

  if (!code || !title || !payload.basePrice || !description) {
    createState.error =
      "Fill in all required fields before creating the article."
    return
  }

  if (!/^[A-Z0-9]{8}$/.test(code)) {
    createState.error = "Article code must be exactly 8 letters or numbers."
    return
  }

  form.value.code = code

  createState.loading = true
  const result = await api.createArticle({
    code,
    title,
    basePrice: payload.basePrice,
    description,
    isAvailable: payload.isAvailable,
  })
  createState.loading = false

  if (!result.ok || !result.data) {
    createState.error = result.error || "Unable to create article."
    return
  }

  emit("created", result.data)
  isOpen.value = false
}
</script>

<template>
  <UModal v-model:open="isOpen">
    <template #header>
      <div class="space-y-1">
        <h3 class="text-foreground text-base font-semibold">
          Create New Article
        </h3>
        <p class="text-muted text-sm">
          Save core article data, then continue editing it in the browse view.
        </p>
      </div>
    </template>

    <template #body>
      <div class="grid gap-4">
        <CatalogArticlesInformationForm
          v-model="form"
          :disabled="!canEdit || createState.loading"
        />
        <StatusMessages :error="createState.error" />
      </div>
    </template>

    <template #footer>
      <div class="flex w-full justify-between">
        <UButton
          variant="ghost"
          :disabled="createState.loading"
          @click="isOpen = false"
        >
          Cancel
        </UButton>
        <UButton
          color="primary"
          :loading="createState.loading"
          :disabled="!canEdit"
          @click="createArticle"
        >
          Create Article
        </UButton>
      </div>
    </template>
  </UModal>
</template>
