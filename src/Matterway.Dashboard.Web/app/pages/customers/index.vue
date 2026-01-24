<script setup lang="ts">
import { adminServices } from '~/data/adminFeatures'
import { useAuthSession } from '~/composables/useAuthSession'

definePageMeta({
  title: 'Customers Overview',
  service: 'customers',
  level: 'observer'
})

const auth = useAuthSession()
const section = computed(() => adminServices.find(item => item.key === 'customers'))

const features = computed(() =>
  section.value?.features.filter(feature =>
    auth.hasPermission(feature.service, feature.minimum)
  ) || []
)
</script>

<template>
  <ServiceOverview
    title="Customers"
    description="Lookup customers, manage profiles, and inspect cart items."
    :features="features"
  />
</template>
