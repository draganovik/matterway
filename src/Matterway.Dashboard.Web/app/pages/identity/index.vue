<script setup lang="ts">
import { adminServices } from '~/data/adminFeatures'
import { useAuthSession } from '~/composables/useAuthSession'

definePageMeta({
  title: 'Identity Overview',
  service: 'identity',
  level: 'operator'
})
</script>

<template>
  <ServiceOverview
    title="Identity"
    description="Administer system users and employee permissions."
    :features="features"
  />
</template>
