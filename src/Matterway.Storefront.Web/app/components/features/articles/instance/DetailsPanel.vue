<script setup lang="ts">
import type { CatalogArticleDetail } from "~/types/catalog/articles"

const props = withDefaults(
  defineProps<{
    details?: CatalogArticleDetail[] | null
  }>(),
  {
    details: () => [],
  },
)

const detailRows = computed(() =>
  [...(props.details ?? [])]
    .map((detail, index) => ({
      rowKey: `${detail.detailSlug || detail.title || "detail"}-${index}`,
      key: detail.title?.trim() || detail.detailSlug || "Detalj",
      value: detail.textValue ?? detail.numericValue ?? "-",
      unit: detail.unit ?? "",
    }))
    .sort((a, b) =>
      a.key.localeCompare(b.key, undefined, {
        sensitivity: "base",
      }),
    ),
)
</script>

<template>
  <UCard :ui="{ body: 'p-0 sm:p-0' }" class="border-default border">
    <template #header>
      <h2 class="text-lg font-semibold">Detalji</h2>
    </template>

    <table class="w-full border-collapse text-sm">
      <thead>
        <tr class="storefront-subtle-surface">
          <th class="border-default border-b px-3 py-2 text-left font-semibold">
            Naziv
          </th>
          <th class="border-default border-b px-3 py-2 text-left font-semibold">
            Vrednost
          </th>
        </tr>
      </thead>
      <tbody>
        <tr
          v-for="detail in detailRows"
          :key="detail.rowKey"
          class="border-default border-b last:border-b-0"
        >
          <td class="text-muted px-3 py-2">{{ detail.key }}</td>
          <td class="px-3 py-2 font-medium">
            {{ detail.value }}
            <span v-if="detail.unit" class="text-muted ml-1">{{
              detail.unit
            }}</span>
          </td>
        </tr>
        <tr v-if="!detailRows.length">
          <td colspan="2" class="text-muted px-3 py-2">
            Detalji nisu dostupni.
          </td>
        </tr>
      </tbody>
    </table>
  </UCard>
</template>
