<script setup lang="ts">
import { adminServices } from '~/data/adminFeatures'
import { useAuthSession } from '~/composables/useAuthSession'

definePageMeta({
  title: 'Customers Overview',
  service: 'customers',
  level: 'observer'
})
</script>

<template>
  <ServiceOverview
    title="Customers"
    description="Lookup customers, manage profiles, and inspect cart items."
    :features="features"
  />
</template>
