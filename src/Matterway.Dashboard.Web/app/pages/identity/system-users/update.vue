<script setup lang="ts">
import SystemUsersFeature from '@components/identity/SystemUsersFeature.vue'

definePageMeta({
  title: 'System Users',
  service: 'identity',
  level: 'operator',
  action: 'update'
})
</script>

<template>
  <SystemUsersFeature />
</template>
