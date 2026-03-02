<script setup lang="ts">
import { serviceSections } from "~/data/serviceRegistry"

definePageMeta({
  title: "Identity",
  service: "identity",
  permissions: ["operator", "manager"],
})

const identitySection = serviceSections.find(
  (section) => section.key === "identity",
)
const firstFeatureRoute = identitySection?.features[0]?.route || "/"

await navigateTo(firstFeatureRoute, { replace: true })
</script>

<template>
  <div />
</template>
