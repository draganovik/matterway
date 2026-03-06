<script setup lang="ts">
const props = defineProps<{
  page: number
  pages: number[]
  totalPages: number
}>()

const emit = defineEmits<{
  "go-to-page": [page: number]
}>()
</script>

<template>
  <div v-if="props.totalPages > 1" class="flex flex-wrap items-center gap-2">
    <UButton
      color="neutral"
      variant="soft"
      :disabled="props.page <= 1"
      @click="emit('go-to-page', props.page - 1)"
    >
      Prethodna
    </UButton>

    <UButton
      v-for="pageNumber in props.pages"
      :key="`page-${pageNumber}`"
      :variant="pageNumber === props.page ? 'solid' : 'soft'"
      :color="pageNumber === props.page ? 'primary' : 'neutral'"
      @click="emit('go-to-page', pageNumber)"
    >
      {{ pageNumber }}
    </UButton>

    <UButton
      color="neutral"
      variant="soft"
      :disabled="props.page >= props.totalPages"
      @click="emit('go-to-page', props.page + 1)"
    >
      Sledeća
    </UButton>
  </div>
</template>
