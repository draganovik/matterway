<script setup lang="ts">
import UserPermsFeature from '@components/identity/UserPermsFeature.vue'

definePageMeta({
  title: 'User Permissions',
  service: 'identity',
  level: 'operator',
  action: 'delete'
})
</script>

<template>
  <UserPermsFeature />
</template>
