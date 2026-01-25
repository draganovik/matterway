<script setup lang="ts">
import { adminServices } from '~/data/adminFeatures'
import { useAuthSession } from '~/composables/useAuthSession'

definePageMeta({
  title: 'Sales Overview',
  service: 'sales',
  level: 'observer'
})
</script>

<template>
  <ServiceOverview
    title="Sales"
    description="Track orders, inspect payments, and monitor sales activity."
    :features="features"
  />
</template>
