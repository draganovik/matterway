<script setup lang="ts">
import OrdersFeature from '@components/sales/OrdersFeature.vue'

definePageMeta({
  title: 'Orders',
  service: 'sales',
  level: 'observer',
  action: 'create'
})
</script>

<template>
  <OrdersFeature />
</template>
