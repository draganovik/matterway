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
    emptyMessage: "No articles found.",
  },
)

const emit = defineEmits<{
  toggle: [id: string]
}>()

function resolveId(article: QueryArticleResponse) {
  return article.id || ""
}

function isSelected(article: QueryArticleResponse) {
  const id = resolveId(article)
  return Boolean(id && props.selectedIds.includes(id))
}
</script>

<template>
  <div class="max-h-75 space-y-2 overflow-y-auto">
    <StatusMessages
      v-if="props.error || props.loading || !props.items.length"
      :error="props.error"
      :loading="props.loading ? 'Loading articles.' : false"
      :empty="
        !props.loading && !props.error && !props.items.length
          ? props.emptyMessage
          : false
      "
    />
    <div v-else class="space-y-2">
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
