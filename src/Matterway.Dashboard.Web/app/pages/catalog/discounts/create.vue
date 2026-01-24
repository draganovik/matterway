<script setup lang="ts">
import DiscountsFeature from '@components/catalog/DiscountsFeature.vue'

definePageMeta({
  title: 'Discounts',
  service: 'catalog',
  level: 'operator',
  action: 'create'
})
</script>

<template>
  <DiscountsFeature />
</template>
