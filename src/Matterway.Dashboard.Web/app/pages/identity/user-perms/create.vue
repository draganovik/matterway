<script setup lang="ts">
import { useApiClient } from '~/composables/useApiClient'
import { permissionLevels, permissionServices } from '~/data/permissions'

definePageMeta({
  title: 'User Permissions',
  service: 'identity',
  level: 'operator',
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
            Add permission
          </h2>
        </template>
        <UForm
          class="space-y-4"
          @submit="addPerm"
        >
          <UFormField
            label="User Id"
            required
          >
            <UInput
              v-model="addForm.userId"
              placeholder="GUID"
            />
          </UFormField>
          <div class="grid gap-4 md:grid-cols-2">
            <UFormField
              label="Service"
              required
            >
              <USelectMenu
                v-model="addForm.service"
                :items="permissionServices"
                value-attribute="value"
                option-attribute="label"
              />
            </UFormField>
            <UFormField
              label="Level"
              required
            >
              <USelectMenu
                v-model="addForm.level"
                :items="permissionLevels"
                value-attribute="value"
                option-attribute="label"
              />
            </UFormField>
          </div>
          <UButton
            type="submit"
            color="primary"
            :loading="addState.loading"
          >
            Add Permission
          </UButton>
          <FormStatus
            :error="addState.error"
            :success="addState.success"
          />
        </UForm>
      </UCard>
      <UCard class="border border-default bg-elevated/40">
        <template #header>
          <h3 class="text-sm font-semibold text-muted">
            Policy
          </h3>
        </template>
        <p class="text-sm text-muted">
          Only Administrators can add or remove permissions for employee accounts.
        </p>
      </UCard>
    </div>
  </FeatureShell>
</template>
