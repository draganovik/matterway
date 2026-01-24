<script setup lang="ts">
import { adminServices } from '~/data/adminFeatures'
import { useAuthSession } from '~/composables/useAuthSession'

definePageMeta({
  title: 'Identity Overview',
  service: 'identity',
  level: 'operator'
})

const auth = useAuthSession()
const section = computed(() => adminServices.find(item => item.key === 'identity'))

const features = computed(() =>
  section.value?.features.filter(feature =>
    auth.hasPermission(feature.service, feature.minimum)
  ) || []
)
</script>

<template>
  <ServiceOverview
    title="Identity"
    description="Administer system users and employee permissions."
    :features="features"
  />
</template>
