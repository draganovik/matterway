<script setup lang="ts">
import { permissionLevels, permissionServices } from '~/data/permissions'

definePageMeta({
  title: 'User Permissions',
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
            Remove permission
          </h2>
        </template>
        <UForm
          class="space-y-4"
          @submit="deletePerm"
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
          <div class="grid gap-4 md:grid-cols-2">
            <UFormField
              label="Service"
              required
            >
              <USelectMenu
                v-model="deleteForm.service"
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
                v-model="deleteForm.level"
                :items="permissionLevels"
                value-attribute="value"
                option-attribute="label"
              />
            </UFormField>
          </div>
          <UButton
            type="submit"
            color="error"
            variant="solid"
            :loading="deleteState.loading"
          >
            Remove Permission
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
          Removing permissions returns the updated permission list for the user.
        </p>
      </UCard>
    </div>
  </FeatureShell>
</template>
