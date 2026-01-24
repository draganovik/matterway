<script setup lang="ts">
import ArticleSpecificationsFeature from '@components/catalog/ArticleSpecificationsFeature.vue'

definePageMeta({
  title: 'Article Specifications',
  service: 'catalog',
  level: 'operator',
  action: 'create'
})
</script>

<template>
  <ArticleSpecificationsFeature />
</template>
