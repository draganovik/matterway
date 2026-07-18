<script setup lang="ts">
import type { QueryArticleResponse } from "~/types/catalog"

const props = withDefaults(
  defineProps<{
    items?: QueryArticleResponse[]
    selectedIds?: string[]
    loading?: boolean
    error?: string
    emptyMessage?: string
  }>(),
  {
    items: () => [],
    selectedIds: () => [],
    loading: false,
    error: "",
    emptyMessage: "Nema artikala.",
  },
)

const emit = defineEmits<{
  toggle: [id: string]
}>()

function resolveId(article: QueryArticleResponse) {
  return article.code || ""
}

function isSelected(article: QueryArticleResponse) {
  const id = resolveId(article)
  return Boolean(id && props.selectedIds.includes(id))
}
</script>

<template>
  <div class="max-h-75 overflow-y-auto p-1">
    <StatusMessages
      v-if="props.error || !props.items.length"
      :error="props.error"
      :loading="props.loading ? 'Učitavanje artikala.' : false"
      :empty="
        !props.loading && !props.error && !props.items.length
          ? props.emptyMessage
          : false
      "
    />
    <div v-else class="border-muted border-y">
      <EntitiesListItem
        v-for="(article, index) in props.items"
        :key="resolveId(article) || `article-${index}`"
        :selected="isSelected(article)"
        @click="
          resolveId(article) ? emit('toggle', resolveId(article)) : undefined
        "
      >
        <CatalogDiscountsModalArticleSelectionListItem
          :item="article"
          :selected="isSelected(article)"
        />
      </EntitiesListItem>
    </div>
  </div>
</template>
