<script setup lang="ts">
import OrdersFeature from '@components/sales/OrdersFeature.vue'

definePageMeta({
  title: 'Orders',
  service: 'sales',
  level: 'observer'
})
</script>

<template>
  <OrdersFeature />
</template>
