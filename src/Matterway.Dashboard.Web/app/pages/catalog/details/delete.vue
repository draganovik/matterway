<script setup lang="ts">
import DetailsFeature from '@components/catalog/DetailsFeature.vue'

definePageMeta({
  title: 'Details',
  service: 'catalog',
  level: 'observer',
  action: 'delete'
})
</script>

<template>
  <DetailsFeature />
</template>
