<script setup lang="ts">
const props = defineProps<{
  totalCount: number;
  pageSize: number;
  pageOptions: number[];
}>();

const emit = defineEmits<{
  "update:pageSize": [value: number];
}>();
</script>

<template>
  <div class="flex flex-wrap items-center justify-between gap-3">
    <div class="text-sm text-slate-500">{{ props.totalCount }} rezultata</div>
    <div class="flex items-center gap-2">
      <span class="text-sm text-slate-500">Po stranici</span>
      <USelect
        :model-value="props.pageSize"
        :items="
          props.pageOptions.map((size) => ({
            label: String(size),
            value: size,
          }))
        "
        @update:model-value="emit('update:pageSize', Number($event))"
      />
    </div>
  </div>
</template>
