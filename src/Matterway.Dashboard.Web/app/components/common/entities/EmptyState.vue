<script setup lang="ts">
defineProps<{
  title: string
  description: string
}>()
</script>

<template>
  <div
    class="border-default bg-default rounded-md border px-4 py-5 text-center"
  >
    <p class="text-highlighted text-sm font-medium">
      {{ title }}
    </p>
    <p class="text-muted mt-1 text-sm">
      {{ description }}
    </p>
  </div>
</template>
