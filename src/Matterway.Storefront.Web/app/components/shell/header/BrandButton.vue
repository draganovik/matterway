<script setup lang="ts">
import logoUrl from "~/assets/brand/matterway-logo-cyan.svg"
</script>

<template>
  <NuxtLink to="/" class="flex items-center">
    <img :src="logoUrl" alt="Matterway" class="h-9 w-9" />
  </NuxtLink>
</template>
