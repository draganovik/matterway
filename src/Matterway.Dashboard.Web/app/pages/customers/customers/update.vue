<script setup lang="ts">
import CustomersFeature from '@components/customers/CustomersFeature.vue'

definePageMeta({
  title: 'Customers',
  service: 'customers',
  level: 'observer',
  action: 'update'
})
</script>

<template>
  <CustomersFeature />
</template>
