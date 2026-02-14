<script setup lang="ts">
import logoUrl from "~/assets/brand/matterway-logo-red.svg"
</script>

<template>
  <ImageWithFallback
    :src="logoUrl"
    alt="Matterway"
    img-class="block h-full w-full"
    placeholder-class="h-full w-full rounded-sm"
  />
</template>
