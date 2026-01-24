<script setup lang="ts">
import CustomersFeature from '@components/customers/CustomersFeature.vue'

definePageMeta({
  title: 'Customers',
  service: 'customers',
  level: 'observer'
})
</script>

<template>
  <CustomersFeature />
</template>
