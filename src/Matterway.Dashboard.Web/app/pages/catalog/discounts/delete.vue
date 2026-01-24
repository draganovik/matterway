<script setup lang="ts">
import DiscountsFeature from '@components/catalog/DiscountsFeature.vue'

definePageMeta({
  title: 'Discounts',
  service: 'catalog',
  level: 'operator',
  action: 'delete'
})
</script>

<template>
  <DiscountsFeature />
</template>
