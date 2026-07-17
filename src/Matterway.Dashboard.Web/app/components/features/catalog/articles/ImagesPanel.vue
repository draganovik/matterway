<script setup lang="ts">
import { useCatalogClient } from "~/composables/api/useCatalogClient"
import type { ArticleImageProperty } from "~/types/catalog"
import { useRequestState } from "~/composables/workflows/state/useRequestState"

const props = withDefaults(
  defineProps<{
    code?: string | null
    modelValue?: ArticleImageProperty[]
    canEdit?: boolean
  }>(),
  {
    code: null,
    modelValue: () => [],
    canEdit: false,
  },
)

const emit = defineEmits<{
  (event: "update:modelValue", value: ArticleImageProperty[]): void
}>()

const api = useCatalogClient()
const images = ref<ArticleImageProperty[]>([])
const addState = useRequestState()
const updateState = useRequestState()
const removeState = useRequestState()
const imageModalOpen = ref(false)
const imageModalMode = ref<"add" | "edit">("add")
const activeImage = ref<ArticleImageProperty | null>(null)

function sortImages(next: ArticleImageProperty[]) {
  return [...next].sort((a, b) => Number(a.orderIndex) - Number(b.orderIndex))
}

watch(
  () => props.modelValue,
  (value) => {
    images.value = Array.isArray(value) ? sortImages(value) : []
  },
  { immediate: true },
)

watch(
  () => props.code,
  () => {
    addState.error = ""
    addState.success = ""
  },
)

function updateImages(next: ArticleImageProperty[]) {
  images.value = sortImages(next)
  emit("update:modelValue", images.value)
}

function applyReturnedImages(nextImages?: ArticleImageProperty[] | null) {
  if (!Array.isArray(nextImages)) return false

  updateImages(nextImages)
  return true
}

function openAddImagesModal() {
  addState.error = ""
  addState.success = ""
  if (!props.code) {
    addState.error = "Najpre sačuvajte artikal da biste dodali slike."
    return
  }
  imageModalMode.value = "add"
  activeImage.value = null
  imageModalOpen.value = true
}

function openEditImagesModal(image: ArticleImageProperty) {
  updateState.error = ""
  updateState.success = ""
  imageModalMode.value = "edit"
  activeImage.value = image
  imageModalOpen.value = true
}

async function addImage(payload: {
  orderIndex: number
  imageAlt: string
  file?: File | null
}) {
  addState.error = ""
  addState.success = ""
  if (!props.code) {
    addState.error = "Najpre sačuvajte artikal da biste dodali slike."
    return
  }
  const code = props.code
  if (!payload.file) {
    addState.error = "Izaberite sliku za otpremanje."
    return
  }
  addState.loading = true
  const result = await api.addArticleImage(code, {
    orderIndex: payload.orderIndex,
    file: payload.file,
    imageAlt: payload.imageAlt,
  })
  if (!result.ok) {
    addState.loading = false
    addState.error = result.error || "Dodavanje slike nije uspelo."
    return
  }

  addState.loading = false
  if (!applyReturnedImages(result.data?.images)) {
    addState.error =
      "Slika je dodata, ali odgovor ne sadrži ažuriranu listu slika."
    imageModalOpen.value = false
    return
  }

  addState.success = "Slika je uspešno dodata."
  imageModalOpen.value = false
}

async function updateImage(payload: { orderIndex: number; imageAlt: string }) {
  updateState.error = ""
  updateState.success = ""
  if (!props.code) return
  const code = props.code
  if (!activeImage.value) {
    updateState.error = "Izaberite sliku za izmenu."
    return
  }
  const targetOrderIndex = activeImage.value.orderIndex
  if (targetOrderIndex === undefined || targetOrderIndex === null) {
    updateState.error = "Izabrana slika nema definisan redosled."
    return
  }
  updateState.loading = true
  const result = await api.updateArticleImage(code, targetOrderIndex, {
    imageAlt: payload.imageAlt,
    orderIndex: payload.orderIndex,
  })
  if (!result.ok) {
    updateState.loading = false
    updateState.error = result.error || "Ažuriranje slike nije uspelo."
    return
  }

  updateState.loading = false
  if (!applyReturnedImages(result.data?.images)) {
    updateState.error =
      "Slika je ažurirana, ali odgovor ne sadrži ažuriranu listu slika."
    imageModalOpen.value = false
    return
  }

  updateState.success = "Slika je uspešno ažurirana."
  imageModalOpen.value = false
}

async function removeImage(image: ArticleImageProperty) {
  removeState.error = ""
  removeState.success = ""
  if (!props.code) return
  const code = props.code
  removeState.loading = true
  const result = await api.removeArticleImage(code, image.orderIndex)
  if (!result.ok) {
    removeState.loading = false
    removeState.error = result.error || "Uklanjanje slike nije uspelo."
    return
  }

  removeState.loading = false
  if (!applyReturnedImages(result.data?.images)) {
    removeState.error =
      "Slika je uklonjena, ali odgovor ne sadrži ažuriranu listu slika."
    return
  }

  removeState.success = "Slika je uspešno uklonjena."
}

async function handleImageSubmit(payload: {
  orderIndex: number
  imageAlt: string
  file?: File | null
}) {
  if (imageModalMode.value === "add") {
    await addImage(payload)
    return
  }
  await updateImage(payload)
}
</script>

<template>
  <div class="flex flex-col gap-4">
    <div class="flex items-center justify-between">
      <h3 class="text-highlighted text-base font-semibold">Slike</h3>
      <UButton
        color="primary"
        variant="outline"
        :disabled="!canEdit || !code"
        @click="openAddImagesModal"
      >
        Dodaj sliku
      </UButton>
    </div>

    <div
      v-if="!code"
      class="border-default bg-default text-muted rounded-md border px-4 py-4 text-sm"
    >
      Najpre sačuvajte artikal da biste dodali slike.
    </div>

    <div v-else class="@container grid gap-4">
      <div
        class="border-default/40 grid rounded-md border @sm:grid-cols-2 @md:grid-cols-3 @lg:grid-cols-4"
      >
        <div
          v-for="image in images"
          :key="image.id"
          class="border-default/40 border-t px-3 py-3 first:border-t-0"
        >
          <div class="grid gap-3">
            <div class="bg-muted/60 aspect-4/3 overflow-hidden rounded-md">
              <ImageWithFallback
                :src="image.imageUrl"
                :alt="image.imageAlt || ''"
                img-class="h-full w-full object-cover"
                placeholder-class="h-full w-full"
              />
            </div>
            <div class="grid gap-3">
              <UButton
                variant="outline"
                :disabled="!canEdit"
                class="justify-center"
                @click="openEditImagesModal(image)"
              >
                Izmeni
              </UButton>
              <UButton
                color="error"
                variant="ghost"
                :disabled="!canEdit"
                class="justify-center"
                @click="removeImage(image)"
              >
                Ukloni
              </UButton>
            </div>
          </div>
        </div>
      </div>

      <StatusMessages
        :error="addState.error || updateState.error || removeState.error"
        :success="
          addState.success || updateState.success || removeState.success
        "
      />
    </div>
  </div>

  <CatalogArticlesModalImageView
    v-model:open="imageModalOpen"
    :mode="imageModalMode"
    :image="activeImage"
    :can-edit="canEdit"
    :loading="imageModalMode === 'add' ? addState.loading : updateState.loading"
    :error="imageModalMode === 'add' ? addState.error : updateState.error"
    @submit="handleImageSubmit"
  />
</template>
