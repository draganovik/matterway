<script setup lang="ts">
const props = defineProps<{
  description?: string | null
}>()
</script>

<template>
  <UCard class="border-default border">
    <template #header>
      <h2 class="text-lg font-semibold">Opis</h2>
    </template>
    <p class="text-muted text-sm leading-relaxed whitespace-pre-wrap">
      {{ props.description || "Opis nije dostupan." }}
    </p>
  </UCard>
</template>
