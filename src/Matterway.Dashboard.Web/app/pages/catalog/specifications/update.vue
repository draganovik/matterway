<script setup lang="ts">
import SpecificationsFeature from '@components/catalog/SpecificationsFeature.vue'

definePageMeta({
  title: 'Specifications',
  service: 'catalog',
  level: 'observer',
  action: 'update'
})
</script>

<template>
  <SpecificationsFeature />
</template>
