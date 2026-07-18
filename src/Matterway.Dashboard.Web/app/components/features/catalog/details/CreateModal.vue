<script setup lang="ts">
import { useCatalogClient } from "~/composables/api/useCatalogClient"
import type { QueryDetailResponse } from "~/types/catalog"
import { useRequestState } from "~/composables/workflows/state/useRequestState"
import { useResetOnModalOpen } from "~/composables/workflows/modal/useResetOnModalOpen"
import { normalizeSlug } from "~/utils/normalization"

type DetailCreateForm = {
  slug: string
  title: string
  unit: string
}

const { canEdit = false } = defineProps<{
  canEdit?: boolean
}>()

const emit = defineEmits<{
  created: [detail: QueryDetailResponse]
}>()

const isOpen = defineModel<boolean>("open", { required: true })

const api = useCatalogClient()
const createState = useRequestState()

const form = ref<DetailCreateForm>({
  slug: "",
  title: "",
  unit: "",
})

function resetForm() {
  form.value = {
    slug: "",
    title: "",
    unit: "",
  }
  createState.error = ""
}

useResetOnModalOpen(isOpen, resetForm)

async function createDetail() {
  createState.error = ""
  if (!canEdit) return

  const slug = normalizeSlug(form.value.slug)
  const title = String(form.value.title ?? "").trim()
  const unit = String(form.value.unit ?? "").trim()

  if (!slug || !title) {
    createState.error = "Slug i naziv su obavezni."
    return
  }

  createState.loading = true
  const result = await api.putDetail(slug, {
    title,
    unit: unit || null,
  })
  createState.loading = false

  if (!result.ok) {
    createState.error = result.error || "Kreiranje detalja nije uspelo."
    return
  }

  emit("created", {
    slug: result.data?.slug ?? slug,
    title: result.data?.title ?? title,
    unit: result.data?.unit ?? (unit || null),
  })

  isOpen.value = false
}
</script>

<template>
  <UModal v-model:open="isOpen">
    <template #header>
      <div class="space-y-1">
        <h3 class="text-highlighted text-base font-semibold">Novi detalj</h3>
        <p class="text-muted text-sm">
          Kreirajte definiciju detalja, a zatim je uređujte iz panela za izmenu.
        </p>
      </div>
    </template>

    <template #body>
      <div class="grid gap-4">
        <UFormField
          label="Slug"
          required
          help="Ključ malim slovima koji se koristi u detaljima artikla."
        >
          <UInput
            v-model="form.slug"
            placeholder="screen-size"
            :disabled="!canEdit || createState.loading"
            class="w-full"
          />
        </UFormField>

        <UFormField label="Naziv" required>
          <UInput
            v-model="form.title"
            placeholder="Veličina ekrana"
            :disabled="!canEdit || createState.loading"
            class="w-full"
          />
        </UFormField>

        <UFormField
          label="Jedinica"
          help="Opciona jedinica za numeričke vrednosti (npr. cm, kg)."
        >
          <UInput
            v-model="form.unit"
            placeholder="inč"
            :disabled="!canEdit || createState.loading"
            class="w-full"
          />
        </UFormField>

        <StatusMessages :error="createState.error" />
      </div>
    </template>

    <template #footer>
      <div class="flex w-full justify-between">
        <UButton
          variant="ghost"
          :disabled="createState.loading"
          @click="
            () => {
              isOpen = false
            }
          "
        >
          Otkaži
        </UButton>
        <UButton
          color="primary"
          :loading="createState.loading"
          :disabled="!canEdit"
          @click="createDetail"
        >
          {{ createState.loading ? "Kreiranje detalja" : "Kreiraj detalj" }}
        </UButton>
      </div>
    </template>
  </UModal>
</template>
