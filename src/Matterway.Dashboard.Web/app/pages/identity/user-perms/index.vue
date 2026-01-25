<script setup lang="ts">
import { useApiClient } from '~/composables/useApiClient'
import { permissionLevels, permissionServices } from '~/data/permissions'

definePageMeta({
  title: 'User Permissions',
  service: 'identity',
  level: 'operator'
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
            View permissions
          </h2>
        </template>
        <UForm
          class="space-y-4"
          @submit="loadPerms"
        >
          <UFormField
            label="User Id"
            required
          >
            <UInput
              v-model="queryForm.userId"
              placeholder="GUID"
            />
          </UFormField>
          <UButton
            type="submit"
            color="primary"
            :loading="queryState.loading"
          >
            Load Permissions
          </UButton>
          <FormStatus :error="queryState.error" />
        </UForm>
      </UCard>
      <UCard class="border border-default bg-elevated/40">
        <template #header>
          <h3 class="text-sm font-semibold text-muted">
            Current permissions
          </h3>
        </template>
        <div
          v-if="queryResults.length"
          class="space-y-2 text-sm"
        >
          <div
            v-for="perm in queryResults"
            :key="`${perm.service}-${perm.level}`"
            class="rounded-lg border border-default px-3 py-2"
          >
            <p class="font-semibold">
              {{ perm.service }}
            </p>
            <p class="text-muted">
              {{ perm.level }}
            </p>
          </div>
        </div>
        <p
          v-else
          class="text-sm text-muted"
        >
          No permissions loaded.
        </p>
      </UCard>
    </div>
  </FeatureShell>
</template>
