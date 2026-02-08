<script setup lang="ts">
withDefaults(defineProps<{
  title?: string
  description?: string
}>(), {
  title: 'Nothing selected',
  description: 'Select an item from the list to start editing.'
})
</script>

<template>
  <div class="rounded-lg border border-default bg-background px-4 py-6 text-center">
    <p class="text-sm font-medium text-foreground">
      {{ title }}
    </p>
    <p class="mt-1 text-sm text-muted">
      {{ description }}
    </p>
  </div>
</template>
