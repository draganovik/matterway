<script setup lang="ts">
import { permissionLevels, permissionServices } from '~/data/serviceRegistry'
import { useApiClient } from '~/composables/useApiClient'
import { useRequestState } from '~/composables/useRequestState'

definePageMeta({
  title: 'User Permissions',
  service: 'identity',
  level: 'operator',
  action: 'create'
})

const api = useApiClient()

const addForm = reactive({
  userId: '',
  service: '',
  level: ''
})
const addState = useRequestState()

async function addPerm() {
  addState.error = ''
  addState.success = ''
  if (!addForm.userId || !addForm.service || !addForm.level) {
    addState.error = 'User, service, and level are required.'
    return
  }
  addState.loading = true
  try {
    const result = await api.request(
      'identity',
      `admin/system-users/${addForm.userId}/perms`,
      {
        method: 'POST',
        body: JSON.stringify({
          service: addForm.service,
          level: addForm.level
        })
      }
    )
    if (!result.ok) {
      addState.error = result.error || 'Failed to add permission.'
      return
    }
    addState.success = 'Permission added.'
  } catch (err) {
    addState.error = err instanceof Error ? err.message : 'Failed to add permission.'
  } finally {
    addState.loading = false
  }
}
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
