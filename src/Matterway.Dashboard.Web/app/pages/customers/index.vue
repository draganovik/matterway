<script setup lang="ts">
import { serviceSections } from "~/data/serviceRegistry"

definePageMeta({
  title: "Customers",
  service: "customers",
  permissions: ["observer", "operator", "manager"],
})

const customersSection = serviceSections.find(
  (section) => section.key === "customers",
)
const firstFeatureRoute = customersSection?.features[0]?.route || "/"

await navigateTo(firstFeatureRoute, { replace: true })
</script>

<template>
  <div />
</template>
