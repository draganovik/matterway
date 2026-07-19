<script setup lang="ts">
import logoUrl from "~/assets/brand/matterway-logo-cyan.svg"
</script>

<template>
  <NuxtLink to="/" class="flex items-center">
    <ImageWithFallback
      :src="logoUrl"
      alt="Matterway"
      img-class="h-8 w-8"
      placeholder-class="h-8 w-8 rounded-md"
    />
  </NuxtLink>
</template>
