<script setup lang="ts">
import { useApiClient } from '~/composables/useApiClient'
import { useRequestState } from '~/composables/useRequestState'
import { normalizeList } from '~/utils/http'

definePageMeta({
  title: 'User Permissions',
  service: 'identity',
  level: 'operator'
})

const api = useApiClient()

const queryForm = reactive({
  userId: ''
})
const queryState = useRequestState()
const queryResults = ref<Array<{ service: string, level: string }>>([])

async function loadPerms() {
  queryState.error = ''
  queryState.loading = true
  try {
    const result = await api.request<unknown>(
      'identity',
      `admin/system-users/${queryForm.userId}/perms`
    )
    if (!result.ok) {
      queryState.error = result.error || 'Failed to load permissions.'
      queryResults.value = []
      return
    }
    queryResults.value = normalizeList(result.data)
  } catch (err) {
    queryState.error = err instanceof Error ? err.message : 'Failed to load permissions.'
    queryResults.value = []
  } finally {
    queryState.loading = false
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
