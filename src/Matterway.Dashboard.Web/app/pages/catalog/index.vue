<script setup lang="ts">
import { adminServices } from '~/data/adminFeatures'
import { useAuthSession } from '~/composables/useAuthSession'

definePageMeta({
  title: 'Catalog Overview',
  service: 'catalog',
  level: 'observer'
})

const auth = useAuthSession()
const section = computed(() => adminServices.find(item => item.key === 'catalog'))

const features = computed(() =>
  section.value?.features.filter(feature =>
    auth.hasPermission(feature.service, feature.minimum)
  ) || []
)
</script>

<template>
  <ServiceOverview
    title="Catalog"
    description="Manage articles, images, details, specifications, and discounts across the catalog."
    :features="features"
  />
</template>
