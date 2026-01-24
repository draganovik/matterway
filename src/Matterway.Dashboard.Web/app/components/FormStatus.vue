<script setup lang="ts">
defineProps<{
  error?: string
  success?: string
  loading?: boolean
  empty?: string
}>()
</script>

<template>
  <div class="space-y-3">
    <UAlert
      v-if="loading"
      icon="i-lucide-loader"
      color="neutral"
      variant="soft"
    >
      Loading...
    </UAlert>
    <UAlert
      v-if="error"
      icon="i-lucide-alert-triangle"
      color="red"
      variant="soft"
    >
      {{ error }}
    </UAlert>
    <UAlert
      v-if="success"
      icon="i-lucide-check-circle"
      color="green"
      variant="soft"
    >
      {{ success }}
    </UAlert>
    <UAlert
      v-if="empty"
      icon="i-lucide-inbox"
      color="neutral"
      variant="soft"
    >
      {{ empty }}
    </UAlert>
  </div>
</template>
