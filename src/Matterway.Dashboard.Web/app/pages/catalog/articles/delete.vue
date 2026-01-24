<script setup lang="ts">
import ArticlesFeature from '@components/catalog/ArticlesFeature.vue'

definePageMeta({
  title: 'Articles',
  service: 'catalog',
  level: 'operator',
  action: 'delete'
})
</script>

<template>
  <ArticlesFeature />
</template>
