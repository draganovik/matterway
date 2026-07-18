<script setup lang="ts">
const props = defineProps<{ text: string }>()
</script>

<template>
  <UTooltip
    :text="props.text"
    :content="{ side: 'bottom', align: 'start' }"
    :ui="{
      content: 'h-auto max-w-80 py-2',
      text: 'whitespace-normal',
    }"
  >
    <UButton
      aria-label="Informacije o stranici"
      icon="i-lucide-info"
      color="neutral"
      variant="ghost"
      size="xs"
      class="text-muted"
    />
  </UTooltip>
</template>
