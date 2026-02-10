<script setup lang="ts">
import { useCatalogApi } from '~/composables/useCatalogApi'
import type { QueryDetailResponse } from '~/types/catalog'
import { useRequestState } from '~/composables/useRequestState'
import { normalizeSlug } from '~/utils/normalization'

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

const isOpen = defineModel<boolean>('open', { required: true })

const api = useCatalogApi()
const createState = useRequestState()

const form = ref<DetailCreateForm>({
  slug: '',
  title: '',
  unit: ''
})

function resetForm() {
  form.value = {
    slug: '',
    title: '',
    unit: ''
  }
  createState.error = ''
}

watch(isOpen, (open) => {
  if (open) resetForm()
})

async function createDetail() {
  createState.error = ''
  if (!canEdit) return

  const slug = normalizeSlug(form.value.slug)
  const title = form.value.title.trim()
  const unit = form.value.unit.trim()

  if (!slug || !title) {
    createState.error = 'Slug and title are required.'
    return
  }

  createState.loading = true
  const result = await api.putDetail(slug, {
    title,
    unit: unit || null
  })
  createState.loading = false

  if (!result.ok) {
    createState.error = result.error || 'Unable to create detail.'
    return
  }

  emit('created', {
    slug: result.data?.slug ?? slug,
    title: result.data?.title ?? title,
    unit: result.data?.unit ?? (unit || null)
  })

  isOpen.value = false
}
</script>

<template>
  <UModal v-model:open="isOpen">
    <template #header>
      <div class="space-y-1">
        <h3 class="text-foreground text-base font-semibold">
          Create New Detail
        </h3>
        <p class="text-muted text-sm">
          Create a detail definition, then manage it from the editor panel.
        </p>
      </div>
    </template>

    <template #body>
      <div class="grid gap-4">
        <UFormField
          label="Slug"
          required
          help="Lowercase key used in article details."
        >
          <UInput
            v-model="form.slug"
            placeholder="screen-size"
            :disabled="!canEdit || createState.loading"
            class="w-full"
          />
        </UFormField>

        <UFormField label="Title" required>
          <UInput
            v-model="form.title"
            placeholder="Screen Size"
            :disabled="!canEdit || createState.loading"
            class="w-full"
          />
        </UFormField>

        <UFormField
          label="Unit"
          help="Optional unit for numeric values (e.g. cm, kg)."
        >
          <UInput
            v-model="form.unit"
            placeholder="inch"
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
          @click="isOpen = false"
        >
          Cancel
        </UButton>
        <UButton
          color="primary"
          :loading="createState.loading"
          :disabled="!canEdit"
          @click="createDetail"
        >
          Create Detail
        </UButton>
      </div>
    </template>
  </UModal>
</template>
