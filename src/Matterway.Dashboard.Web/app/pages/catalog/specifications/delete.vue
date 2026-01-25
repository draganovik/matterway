<script setup lang="ts">
definePageMeta({
  title: 'Specifications',
  service: 'catalog',
  level: 'observer',
  action: 'delete'
})
</script>

<template>
  <div class="space-y-6">
    <div
      class="grid gap-6 lg:grid-cols-[2fr_1fr]"
    >
      <UCard class="border border-default">
        <template #header>
          <h2 class="text-lg font-semibold">
            Delete specification
          </h2>
        </template>
        <UForm
          class="space-y-4"
          @submit="deleteSpec"
        >
          <UFormField
            label="Slug"
            required
          >
            <UInput
              v-model="deleteForm.slug"
              placeholder="bandwidth"
            />
          </UFormField>
          <UButton
            type="submit"
            color="error"
            variant="solid"
            :loading="deleteState.loading"
          >
            Delete Specification
          </UButton>
          <FormStatus
            :error="deleteState.error"
            :success="deleteState.success"
          />
        </UForm>
      </UCard>
      <UCard class="border border-default bg-elevated/40">
        <template #header>
          <h3 class="text-sm font-semibold text-muted">
            Note
          </h3>
        </template>
        <p class="text-sm text-muted">
          Specifications referenced by articles cannot be deleted and will return a validation error.
        </p>
      </UCard>
    </div>
  </div>
</template>
