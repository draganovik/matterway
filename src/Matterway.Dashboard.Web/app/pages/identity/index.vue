<script setup lang="ts">
import { serviceSections } from '~/data/serviceRegistry'
import { useAuthSession } from '~/composables/useAuthSession'

definePageMeta({
  title: 'Identity Overview',
  service: 'identity',
  level: 'operator'
})

const auth = useAuthSession()
const features = computed(() => {
  const service = serviceSections.find(item => item.key === 'identity')
  if (!service) return []
  return service.features.filter(feature =>
    auth.hasPermission(feature.service, feature.minimum)
  )
})
</script>

<template>
  <ServiceOverview
    title="Identity"
    description="Administer system users and employee permissions."
    :features="features"
  />
</template>
