<script setup lang="ts">
import { useApiClient } from '~/composables/useApiClient'
import { useRequestState } from '~/composables/useRequestState'

definePageMeta({
  title: 'Specifications',
  service: 'catalog',
  level: 'observer',
  action: 'update'
})

const api = useApiClient()

const upsertForm = reactive({
  slug: '',
  title: '',
  unit: ''
})
const upsertState = useRequestState()

async function upsertSpec() {
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
      `admin/specifications/${upsertForm.slug}`,
      {
        method: 'PUT',
        body: JSON.stringify({
          title: upsertForm.title,
          unit: upsertForm.unit || null
        })
      }
    )
    if (!result.ok) {
      upsertState.error = result.error || 'Failed to save specification.'
      return
    }
    upsertState.success = 'Specification saved.'
  } catch (err) {
    upsertState.error = err instanceof Error ? err.message : 'Failed to save specification.'
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
            Create/Replace specification
          </h2>
        </template>
        <UForm
          class="space-y-4"
          @submit="upsertSpec"
        >
          <UFormField
            label="Slug"
            required
          >
            <UInput
              v-model="upsertForm.slug"
              placeholder="bandwidth"
            />
          </UFormField>
          <UFormField
            label="Title"
            required
          >
            <UInput
              v-model="upsertForm.title"
              placeholder="Bandwidth"
            />
          </UFormField>
          <UFormField label="Unit">
            <UInput
              v-model="upsertForm.unit"
              placeholder="Mbps"
            />
          </UFormField>
          <UButton
            type="submit"
            color="primary"
            :loading="upsertState.loading"
          >
            Save Specification
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
          PUT creates the slug if missing or replaces the definition if it already exists.
        </p>
      </UCard>
    </div>
  </div>
</template>
