<script setup lang="ts">
const props = defineProps<{
  page: number
  pageSize: number
  pageOptions: number[]
  totalCount: number
  totalPages: number
}>()

const emit = defineEmits<{
  "go-to-page": [page: number]
  "update:page-size": [value: number]
}>()
</script>

<template>
  <div v-if="props.totalCount > 0" class="space-y-4">
    <UFormField label="Po stranici">
      <USelect
        :model-value="props.pageSize"
        :items="
          props.pageOptions.map((size) => ({
            label: String(size),
            value: size,
          }))
        "
        placeholder="Izaberi broj"
        class="w-full"
        @update:model-value="emit('update:page-size', Number($event))"
      />
    </UFormField>

    <div
      class="text-muted flex flex-wrap items-center justify-between gap-2 text-sm"
    >
      <span
        >Strana {{ props.page }} od {{ Math.max(props.totalPages, 1) }}</span
      >
      <span>{{ props.totalCount }} rezultata</span>
    </div>

    <div v-if="props.totalPages > 1" class="flex justify-center">
      <UPagination
        :page="props.page"
        :items-per-page="props.pageSize"
        :total="props.totalCount"
        :sibling-count="1"
        show-controls
        @update:page="emit('go-to-page', $event)"
      />
    </div>
  </div>
</template>
