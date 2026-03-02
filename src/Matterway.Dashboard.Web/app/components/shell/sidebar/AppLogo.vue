<script setup lang="ts">
import logoUrl from "~/assets/brand/matterway-logo-red.svg"
</script>

<template>
  <ImageWithFallback
    :src="logoUrl"
    alt="Matterway"
    img-class="block h-6 w-6 shrink-0"
    placeholder-class="h-6 w-6 shrink-0 rounded-sm"
  />
</template>
