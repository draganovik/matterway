<script setup lang="ts">
defineProps<{
  error?: string | null
  info?: string | null
  success?: string | null
}>()
</script>

<template>
  <div class="space-y-3">
    <UAlert
      v-if="error"
      color="error"
      variant="soft"
      icon="i-lucide-circle-alert"
      :title="error"
    />
    <UAlert
      v-if="info"
      color="info"
      variant="soft"
      icon="i-lucide-info"
      :title="info"
    />
    <UAlert
      v-if="success"
      color="success"
      variant="soft"
      icon="i-lucide-check-circle-2"
      :title="success"
    />
  </div>
</template>
