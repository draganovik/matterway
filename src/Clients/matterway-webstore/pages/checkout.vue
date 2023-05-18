<script lang="ts" setup>
definePageMeta({
  middleware: "auth",
  authOnlyRoles: [UserRole.Customer],
});
</script>

<template>
  <div>Page: foo</div>
</template>

<style scoped></style>
