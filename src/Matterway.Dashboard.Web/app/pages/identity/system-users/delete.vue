<script setup lang="ts">
definePageMeta({
  title: 'System Users',
  service: 'identity',
  level: 'operator',
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
            Delete user
          </h2>
        </template>
        <UForm
          class="space-y-4"
          @submit="deleteUser"
        >
          <UFormField
            label="User Id"
            required
          >
            <UInput
              v-model="deleteForm.userId"
              placeholder="GUID"
            />
          </UFormField>
          <UButton
            type="submit"
            color="error"
            variant="solid"
            :loading="deleteState.loading"
          >
            Delete User
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
            Permissions
          </h3>
        </template>
        <p class="text-sm text-muted">
          Deleting a user requires Administrator permissions.
        </p>
      </UCard>
    </div>
  </FeatureShell>
</template>
