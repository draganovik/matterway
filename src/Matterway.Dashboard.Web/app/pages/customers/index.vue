<script setup lang="ts">
import { serviceSections } from '~/data/serviceRegistry'
import { useAuthSession } from '~/composables/useAuthSession'

definePageMeta({
  title: 'Customers Overview',
  service: 'customers',
  level: 'observer'
})

const auth = useAuthSession()
const features = computed(() => {
  const service = serviceSections.find(item => item.key === 'customers')
  if (!service) return []
  return service.features.filter(feature =>
    auth.hasPermission(feature.service, feature.minimum)
  )
})
</script>

<template>
  <ServiceOverview
    title="Customers"
    description="Lookup customers, manage profiles, and inspect cart items."
    :features="features"
  />
</template>
