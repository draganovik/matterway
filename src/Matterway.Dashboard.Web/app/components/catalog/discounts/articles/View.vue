<script setup lang="ts">
const props = withDefaults(
  defineProps<{
    modelValue?: string[]
    canEdit?: boolean
  }>(),
  {
    modelValue: () => [],
    canEdit: false
  }
)

const emit = defineEmits<{
  (event: 'update:modelValue', value: string[]): void
}>()

const articleModalOpen = ref(false)

const selectedIds = computed(() => props.modelValue || [])
const previewIds = computed(() => selectedIds.value.slice(0, 8))

function applySelection(next: string[]) {
  emit('update:modelValue', next)
}
</script>

<template>
  <div class="grid gap-4">
    <div class="flex items-center justify-between gap-3">
      <h3 class="text-foreground text-base font-semibold">Articles</h3>
      <UButton
        color="primary"
        variant="outline"
        :disabled="!canEdit"
        @click="articleModalOpen = true"
      >
        Select Articles
      </UButton>
    </div>

    <div
      v-if="!selectedIds.length"
      class="border-default bg-background text-muted rounded-lg border px-4 py-4 text-sm"
    >
      No articles selected.
    </div>

    <div
      v-else
      class="border-default bg-background rounded-lg border px-4 py-3"
    >
      <div class="text-foreground text-sm font-medium">
        Selected Article IDs ({{ selectedIds.length }})
      </div>
      <div class="mt-2 flex flex-wrap gap-2">
        <UBadge
          v-for="id in previewIds"
          :key="id"
          color="neutral"
          variant="subtle"
        >
          {{ id }}
        </UBadge>
      </div>
      <p
        v-if="selectedIds.length > previewIds.length"
        class="text-muted mt-2 text-xs"
      >
        +{{ selectedIds.length - previewIds.length }} more
      </p>
    </div>
  </div>

  <CatalogDiscountsArticlesModal
    v-model:open="articleModalOpen"
    :selected-ids="selectedIds"
    :can-edit="canEdit"
    @submit="applySelection"
  />
</template>
