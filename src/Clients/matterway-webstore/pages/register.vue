<script lang="ts" setup>
useHead({
  title: "Registracija",
});
definePageMeta({
  middleware: "auth",
  authNoSession: true,
});
</script>

<template>
  <div>Page: foo</div>
</template>

<style scoped></style>
