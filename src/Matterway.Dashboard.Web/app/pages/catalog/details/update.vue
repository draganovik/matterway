<script setup lang="ts">
import DetailsFeature from '@components/catalog/DetailsFeature.vue'

definePageMeta({
  title: 'Details',
  service: 'catalog',
  level: 'observer',
  action: 'update'
})
</script>

<template>
  <DetailsFeature />
</template>
