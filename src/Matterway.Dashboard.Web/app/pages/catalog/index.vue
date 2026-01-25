<script setup lang="ts">
import { adminServices } from '~/data/adminFeatures'
import { useAuthSession } from '~/composables/useAuthSession'

definePageMeta({
  title: 'Catalog Overview',
  service: 'catalog',
  level: 'observer'
})
</script>

<template>
  <ServiceOverview
    title="Catalog"
    description="Manage articles, images, details, specifications, and discounts across the catalog."
    :features="features"
  />
</template>
