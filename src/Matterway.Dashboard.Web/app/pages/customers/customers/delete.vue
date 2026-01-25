<script setup lang="ts">
definePageMeta({
  title: 'Customers',
  service: 'customers',
  level: 'observer',
  action: 'delete'
})
</script>

<template>
  <FeatureShell>
    <div
      class="grid gap-6 lg:grid-cols-[2fr_1fr]"
    >
      <UCard class="border border-default">
        <template #header>
          <h2 class="text-lg font-semibold">
            Delete customer
          </h2>
        </template>
        <UForm
          class="space-y-4"
          @submit="deleteCustomer"
        >
          <UFormField
            label="System User Id"
            required
          >
            <UInput
              v-model="deleteForm.systemUserId"
              placeholder="GUID"
            />
          </UFormField>
          <UButton
            type="submit"
            color="error"
            variant="solid"
            :loading="deleteState.loading"
          >
            Delete Customer
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
            Result
          </h3>
        </template>
        <p class="text-sm text-muted">
          Customer deletion responds with a confirmation message if successful.
        </p>
      </UCard>
    </div>
  </FeatureShell>
</template>
