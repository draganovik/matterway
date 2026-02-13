<script setup lang="ts">
import { serviceSections } from "~/data/serviceRegistry"

definePageMeta({ title: "Catalog", service: "catalog", level: "observer" })

const catalogSection = serviceSections.find(
  (section) => section.key === "catalog",
)
const firstFeatureRoute = catalogSection?.features[0]?.route || "/"

await navigateTo(firstFeatureRoute, { replace: true })
</script>

<template>
  <div />
</template>
