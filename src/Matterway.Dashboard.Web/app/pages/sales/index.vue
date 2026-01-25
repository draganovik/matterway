<script setup lang="ts">
import { adminServices } from '~/data/adminFeatures'
import { useAuthSession } from '~/composables/useAuthSession'

definePageMeta({
  title: 'Sales Overview',
  service: 'sales',
  level: 'observer'
})

const auth = useAuthSession()
const features = computed(() => {
  const service = adminServices.find(item => item.key === 'sales')
  if (!service) return []
  return service.features.filter(feature =>
    auth.hasPermission(feature.service, feature.minimum)
  )
})
</script>

<template>
  <ServiceOverview
    title="Sales"
    description="Track orders, inspect payments, and monitor sales activity."
    :features="features"
  />
</template>
