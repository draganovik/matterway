<script setup lang="ts">
import { useCatalogClient } from "~/composables/api/useCatalogClient"
import type { CreateArticleResponse } from "~/types/catalog"
import { useRequestState } from "~/composables/workflows/state/useRequestState"
import { useResetOnModalOpen } from "~/composables/workflows/modal/useResetOnModalOpen"

type ArticleBaseForm = {
  code: string
  title: string
  basePrice: number | null
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
  basePrice: null,
  description: "",
  isAvailable: true,
})

function resetForm() {
  form.value = {
    code: "",
    title: "",
    basePrice: null,
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
  const code = String(payload.code ?? "")
    .trim()
    .toUpperCase()
  const title = String(payload.title ?? "").trim()
  const description = String(payload.description ?? "").trim()
  const basePrice = payload.basePrice

  if (!code || !title || basePrice == null || !description) {
    createState.error =
      "Popunite sva obavezna polja pre kreiranja artikla."
    return
  }

  if (!/^[A-Z0-9]{8}$/.test(code)) {
    createState.error = "Šifra artikla mora imati tačno 8 slova ili cifara."
    return
  }

  form.value.code = code

  createState.loading = true
  const result = await api.createArticle({
    code,
    title,
    basePrice,
    description,
    isAvailable: payload.isAvailable,
  })
  createState.loading = false

  if (!result.ok || !result.data) {
    createState.error = result.error || "Kreiranje artikla nije uspelo."
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
          Novi artikal
        </h3>
        <p class="text-muted text-sm">
          Sačuvajte osnovne podatke artikla, pa nastavite uređivanje u glavnom
          prikazu.
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
          Otkaži
        </UButton>
        <UButton
          color="primary"
          :loading="createState.loading"
          :disabled="!canEdit"
          @click="createArticle"
        >
          {{ createState.loading ? "Kreiranje artikla" : "Kreiraj artikal" }}
        </UButton>
      </div>
    </template>
  </UModal>
</template>
