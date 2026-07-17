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
  (event: "remove"): void
}>()

const api = useCatalogClient()

const updateState = useRequestState()

type ArticleForm = {
  code: string
  title: string
  basePrice: number | null
  description: string
  isAvailable: boolean
}

const form = ref<ArticleForm>({
  code: "",
  title: "",
  basePrice: null,
  description: "",
  isAvailable: true,
})

watch(
  () => props.article,
  (article) => {
    if (!article) return
    const parsedBasePrice = Number(article.basePrice)
    form.value = {
      code: article.code || "",
      title: article.title || "",
      basePrice: Number.isFinite(parsedBasePrice) ? parsedBasePrice : null,
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
}

async function saveArticle() {
  updateState.error = ""
  updateState.success = ""
  if (!props.article) return
  if (!props.canEdit) return

  const code = String(form.value.code ?? "")
    .trim()
    .toUpperCase()
  const title = String(form.value.title ?? "").trim()
  const description = String(form.value.description ?? "").trim()
  if (!/^[A-Z0-9]{8}$/.test(code)) {
    updateState.error = "Šifra artikla mora imati tačno 8 slova ili cifara."
    return
  }

  form.value.code = code
  form.value.title = title
  form.value.description = description
  updateState.loading = true
  const result = await api.updateArticle(props.article.code, {
    code,
    title: title || null,
    basePrice: form.value.basePrice,
    description: description || null,
    isAvailable: form.value.isAvailable,
  })
  updateState.loading = false
  if (!result.ok) {
    updateState.error = result.error || "Ažuriranje artikla nije uspelo."
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
  updateState.success = "Artikal je uspešno ažuriran."
}

function updateDetails(details: GetArticleResponse["details"]) {
  updateArticleData({ details })
}

function updateImages(images: GetArticleResponse["images"]) {
  updateArticleData({ images })
}
</script>

<template>
  <div class="flex flex-col gap-4">
    <StatusMessages v-if="error" :error="error" />

    <div v-else-if="!article" class="space-y-4">
      <div class="space-y-1">
        <h3 class="text-highlighted text-base font-semibold">
          Uređivanje artikla
        </h3>
        <p class="text-muted text-sm">
          {{
            canEdit
              ? "Za kreiranje, izmenu i brisanje potrebna je dozvola operatera."
              : "Režim samo za čitanje: za izmene je potrebna dozvola operatera."
          }}
        </p>
      </div>

      <EntitiesEmptyState
        title="Ništa nije izabrano"
        description="Izaberite stavku sa liste da biste započeli izmenu."
      />
    </div>

    <div v-else class="grid gap-4">
      <section>
        <div class="flex flex-wrap items-start justify-between gap-3">
          <div>
            <h3 class="text-highlighted text-base font-semibold">
              Podaci artikla
            </h3>
            <p class="text-muted text-sm">
              Ažurirajte osnovne podatke i dostupnost.
            </p>
          </div>
          <div class="text-muted text-sm">
            Šifra artikla: {{ article?.code }}
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
            {{ updateState.loading ? "Čuvanje izmena" : "Sačuvaj izmene" }}
          </UButton>
          <UButton
            color="error"
            variant="ghost"
            :disabled="!canEdit"
            @click="emit('remove')"
          >
            Obriši artikal
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
