<script setup lang="ts">
import { useApiClient } from '~/composables/useApiClient'
import { useFeatureTabs } from '~/composables/useFeatureTabs'
import { permissionLevels, permissionServices } from '~/data/permissions'

const api = useApiClient()
const { active } = useFeatureTabs()

const queryForm = reactive({
  userId: ''
})
const queryState = reactive({ loading: false, error: '' })
const queryResults = ref<any[]>([])

const addForm = reactive({
  userId: '',
  service: 'catalog',
  level: 'operator'
})
const addState = reactive({ loading: false, error: '', success: '' })

const deleteForm = reactive({
  userId: '',
  service: 'catalog',
  level: 'operator'
})
const deleteState = reactive({ loading: false, error: '', success: '' })

async function loadPerms() {
  queryState.loading = true
  queryState.error = ''
  queryResults.value = []
  const result = await api.request<any>(
    'identity',
    `admin/system-users/${queryForm.userId}/perms`
  )
  queryState.loading = false
  if (!result.ok) {
    queryState.error = result.error || 'Failed to load permissions.'
    return
  }
  queryResults.value = result.data || []
}

async function addPerm() {
  addState.loading = true
  addState.error = ''
  addState.success = ''
  const result = await api.request<any>(
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
  addState.loading = false
  if (!result.ok) {
    addState.error = result.error || 'Failed to add permission.'
    return
  }
  addState.success = 'Permission added.'
  queryResults.value = result.data || []
}

async function deletePerm() {
  deleteState.loading = true
  deleteState.error = ''
  deleteState.success = ''
  const result = await api.request<any>(
    'identity',
    `admin/system-users/${deleteForm.userId}/perms`,
    {
      method: 'DELETE',
      body: JSON.stringify({
        service: deleteForm.service,
        level: deleteForm.level
      })
    }
  )
  deleteState.loading = false
  if (!result.ok) {
    deleteState.error = result.error || 'Failed to remove permission.'
    return
  }
  deleteState.success = 'Permission removed.'
  queryResults.value = result.data || []
}
</script>

<template>
  <FeatureShell>
    <div
      v-if="active === 'query'"
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

    <div
      v-else-if="active === 'create'"
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

    <div
      v-else-if="active === 'delete'"
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
