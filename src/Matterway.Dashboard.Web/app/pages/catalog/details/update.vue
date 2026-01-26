<script setup lang="ts">
import { useApiClient } from '~/composables/useApiClient'
import { useRequestState } from '~/composables/useRequestState'

definePageMeta({
  title: 'Details',
  service: 'catalog',
  level: 'observer',
  action: 'update'
})

const api = useApiClient()

const upsertForm = reactive({
  slug: '',
  title: ''
})
const upsertState = useRequestState()

async function upsertDetail() {
  upsertState.error = ''
  upsertState.success = ''
  if (!upsertForm.slug || !upsertForm.title) {
    upsertState.error = 'Slug and title are required.'
    return
  }
  upsertState.loading = true
  try {
    const result = await api.request(
      'catalog',
      `admin/details/${upsertForm.slug}`,
      {
        method: 'PUT',
        body: JSON.stringify({ title: upsertForm.title })
      }
    )
    if (!result.ok) {
      upsertState.error = result.error || 'Failed to save detail.'
      return
    }
    upsertState.success = 'Detail saved.'
  } catch (err) {
    upsertState.error = err instanceof Error ? err.message : 'Failed to save detail.'
  } finally {
    upsertState.loading = false
  }
}
</script>

<template>
  <div class="space-y-6">
    <div
      class="grid gap-6 lg:grid-cols-[2fr_1fr]"
    >
      <UCard class="border border-default">
        <template #header>
          <h2 class="text-lg font-semibold">
            Create/Replace detail
          </h2>
        </template>
        <UForm
          class="space-y-4"
          @submit="upsertDetail"
        >
          <UFormField
            label="Slug"
            required
          >
            <UInput
              v-model="upsertForm.slug"
              placeholder="features"
            />
          </UFormField>
          <UFormField
            label="Title"
            required
          >
            <UInput
              v-model="upsertForm.title"
              placeholder="Features"
            />
          </UFormField>
          <UButton
            type="submit"
            color="primary"
            :loading="upsertState.loading"
          >
            Save Detail
          </UButton>
          <FormStatus
            :error="upsertState.error"
            :success="upsertState.success"
          />
        </UForm>
      </UCard>
      <UCard class="border border-default bg-elevated/40">
        <template #header>
          <h3 class="text-sm font-semibold text-muted">
            Behavior
          </h3>
        </template>
        <p class="text-sm text-muted">
          PUT creates the slug if missing or replaces the title if it already exists.
        </p>
      </UCard>
    </div>
  </div>
</template>
