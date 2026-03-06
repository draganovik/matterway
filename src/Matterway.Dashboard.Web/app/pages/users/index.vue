<script setup lang="ts">
import { useAuthSessionStore } from "~/composables/stores/useAuthSessionStore"
import { serviceSections } from "~/data/serviceRegistry"

definePageMeta({
  title: "Users",
})

const auth = useAuthSessionStore()
const usersSection = serviceSections.find((section) => section.key === "users")
const firstFeatureRoute =
  usersSection?.features.find((feature) =>
    auth.hasPermission(feature.service, feature.allowed),
  )?.route || "/"

await navigateTo(firstFeatureRoute, { replace: true })
</script>

<template>
  <div />
</template>
