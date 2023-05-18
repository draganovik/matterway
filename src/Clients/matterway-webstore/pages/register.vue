<script lang="ts" setup>
definePageMeta({
  middleware: "auth",
  authNoSession: true,
});
</script>

<template>
  <div>Page: foo</div>
</template>

<style scoped></style>
