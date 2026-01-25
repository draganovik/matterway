<script setup lang="ts">
definePageMeta({
  title: 'Orders',
  service: 'sales',
  level: 'observer',
  action: 'create'
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
            Add order status
          </h2>
        </template>
        <UForm
          class="space-y-4"
          @submit="addStatus"
        >
          <UFormField
            label="Order Id"
            required
          >
            <UInput
              v-model="statusForm.orderId"
              placeholder="GUID"
            />
          </UFormField>
          <UFormField
            label="Status"
            required
          >
            <USelectMenu
              v-model="statusForm.status"
              :items="statusOptions"
            />
          </UFormField>
          <UFormField label="Note">
            <UTextarea
              v-model="statusForm.note"
              :rows="3"
            />
          </UFormField>
          <UButton
            type="submit"
            color="primary"
            :loading="statusState.loading"
          >
            Add Status
          </UButton>
          <FormStatus
            :error="statusState.error"
            :success="statusState.success"
          />
        </UForm>
      </UCard>
      <UCard class="border border-default bg-elevated/40">
        <template #header>
          <h3 class="text-sm font-semibold text-muted">
            Workflow
          </h3>
        </template>
        <p class="text-sm text-muted">
          Status updates are appended to the order history and affect downstream fulfillment.
        </p>
      </UCard>
    </div>
  </FeatureShell>
</template>
