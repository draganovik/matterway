<script setup lang="ts">
import { useApiClient } from '~/composables/useApiClient'

definePageMeta({
  title: 'Details',
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
            Delete detail
          </h2>
        </template>
        <UForm
          class="space-y-4"
          @submit="deleteDetail"
        >
          <UFormField
            label="Slug"
            required
          >
            <UInput
              v-model="deleteForm.slug"
              placeholder="features"
            />
          </UFormField>
          <UButton
            type="submit"
            color="error"
            variant="solid"
            :loading="deleteState.loading"
          >
            Delete Detail
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
          Details referenced by articles cannot be deleted and will return a validation error.
        </p>
      </UCard>
    </div>
  </div>
</template>
