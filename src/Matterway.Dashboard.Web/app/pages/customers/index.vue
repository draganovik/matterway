<script setup lang="ts">
import { serviceSections } from '~/data/serviceRegistry'

definePageMeta({ title: 'Customers', service: 'customers', level: 'observer' })

const customersSection = serviceSections.find(
  (section) => section.key === 'customers'
)
const firstFeatureRoute = customersSection?.features[0]?.route || '/'

await navigateTo(firstFeatureRoute, { replace: true })
</script>

<template>
  <div />
</template>
