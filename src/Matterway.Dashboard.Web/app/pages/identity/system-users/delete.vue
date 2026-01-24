<script setup lang="ts">
import SystemUsersFeature from '@components/identity/SystemUsersFeature.vue'

definePageMeta({
  title: 'System Users',
  service: 'identity',
  level: 'operator',
  action: 'delete'
})
</script>

<template>
  <SystemUsersFeature />
</template>
