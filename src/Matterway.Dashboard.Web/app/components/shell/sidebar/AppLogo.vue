<script setup lang="ts">
import logoUrl from "~/assets/brand/matterway-logo-red.svg"
</script>

<template>
  <ImageWithFallback
    :src="logoUrl"
    alt="Matterway"
    img-class="block h-8 w-8 shrink-0"
    placeholder-class="h-8 w-8 shrink-0 rounded-md"
  />
</template>
