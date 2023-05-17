<script lang="ts" setup>
definePageMeta({
  authorize: [UserRole.Customer],
  middleware: "auth",
});
</script>

<template>
  <div>Page: foo</div>
</template>

<style scoped></style>
