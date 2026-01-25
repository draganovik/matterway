<script setup lang="ts">
import { useApiClient } from '~/composables/useApiClient'
import { formatDateTime } from '~/utils/format'

definePageMeta({
  title: 'System Users',
  service: 'identity',
  level: 'operator',
  action: 'update'
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
            Update user
          </h2>
        </template>
        <UForm
          class="space-y-4"
          @submit="updateUser"
        >
          <UFormField
            label="User Id"
            required
          >
            <UInput
              v-model="updateForm.userId"
              placeholder="GUID"
            />
          </UFormField>
          <UFormField label="Email">
            <UInput
              v-model="updateForm.email"
              type="email"
              placeholder="user@matterway.local"
            />
          </UFormField>
          <UFormField label="Password">
            <UInput
              v-model="updateForm.password"
              type="password"
              placeholder="New password"
            />
          </UFormField>
          <UButton
            type="submit"
            color="primary"
            :loading="updateState.loading"
          >
            Update User
          </UButton>
          <FormStatus
            :error="updateState.error"
            :success="updateState.success"
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
          Operators can only update Customer users unless they are Administrators.
        </p>
      </UCard>
    </div>
  </FeatureShell>
</template>
