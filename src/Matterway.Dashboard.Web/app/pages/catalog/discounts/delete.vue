<script setup lang="ts">
definePageMeta({
  title: 'Discounts',
  service: 'catalog',
  level: 'operator',
  action: 'delete'
})
</script>

<template>
  <FeatureShell>
    <div class="space-y-6">
      <div
        class="grid gap-6 lg:grid-cols-[2fr_1fr]"
      >
        <UCard class="border border-default">
          <template #header>
            <h2 class="text-lg font-semibold">
              Delete discounts
            </h2>
          </template>
          <UForm
            class="space-y-4"
            @submit="deleteDiscount"
          >
            <UFormField
              label="Code"
              required
            >
              <UInput
                v-model="deleteForm.code"
                placeholder="SUMMER24"
              />
            </UFormField>
            <UButton
              type="submit"
              color="error"
              variant="solid"
              :loading="deleteState.loading"
            >
              Delete Discounts
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
            Deleting a code removes all associated discounts across article IDs.
          </p>
        </UCard>
      </div>
    </div>
  </FeatureShell>
</template>
