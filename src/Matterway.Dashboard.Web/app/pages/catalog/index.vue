<script setup lang="ts">
import { serviceSections } from '~/data/serviceRegistry'
import { useAuthSession } from '~/composables/useAuthSession'

definePageMeta({
  title: 'Catalog Overview',
  service: 'catalog',
  level: 'observer'
})

const auth = useAuthSession()
const features = computed(() => {
  const service = serviceSections.find(item => item.key === 'catalog')
  if (!service) return []
  return service.features.filter(feature =>
    auth.hasPermission(feature.service, feature.minimum)
  )
})
</script>

<template>
  <ServiceOverview
    title="Catalog"
    description="Manage articles, images, details, specifications, and discounts across the catalog."
    :features="features"
  />
</template>
