<script setup lang="ts">
import { serviceSections } from "~/data/serviceRegistry"

definePageMeta({
  title: "Prodaja",
  service: "sales",
  permissions: ["observer", "operator", "manager"],
})

const salesSection = serviceSections.find((section) => section.key === "sales")
const firstFeatureRoute = salesSection?.features[0]?.route || "/"

await navigateTo(firstFeatureRoute, { replace: true })
</script>

<template>
  <div />
</template>
