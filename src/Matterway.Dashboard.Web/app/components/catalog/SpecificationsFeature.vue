<script setup lang="ts">
import { useApiClient } from '~/composables/useApiClient'
import { useFeatureTabs } from '~/composables/useFeatureTabs'

const api = useApiClient()
const { active } = useFeatureTabs()

const queryForm = reactive({
  limit: 10,
  titleLike: ''
})
const queryState = reactive({ loading: false, error: '', empty: '' })
const queryResults = ref<any[]>([])

const upsertForm = reactive({
  slug: '',
  title: '',
  unit: ''
})
const upsertState = reactive({ loading: false, error: '', success: '' })

const deleteForm = reactive({
  slug: ''
})
const deleteState = reactive({ loading: false, error: '', success: '' })

async function querySpecs() {
  queryState.loading = true
  queryState.error = ''
  queryState.empty = ''
  queryResults.value = []
  const params = new URLSearchParams({
    limit: queryForm.limit.toString()
  })
  if (queryForm.titleLike) params.set('titleLike', queryForm.titleLike)
  const result = await api.request<any>('catalog', `admin/specifications?${params.toString()}`)
  queryState.loading = false
  if (!result.ok) {
    queryState.error = result.error || 'Failed to query specifications.'
    return
  }
  queryResults.value = result.data || []
  if (!queryResults.value.length) queryState.empty = 'No specifications found.'
}

async function upsertSpec() {
  upsertState.loading = true
  upsertState.error = ''
  upsertState.success = ''
  const result = await api.request<any>(
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
  upsertState.loading = false
  if (!result.ok) {
    upsertState.error = result.error || 'Failed to upsert specification.'
    return
  }
  upsertState.success = 'Specification saved.'
}

async function deleteSpec() {
  deleteState.loading = true
  deleteState.error = ''
  deleteState.success = ''
  const result = await api.request<any>('catalog', `admin/specifications/${deleteForm.slug}`, {
    method: 'DELETE'
  })
  deleteState.loading = false
  if (!result.ok) {
    deleteState.error = result.error || 'Failed to delete specification.'
    return
  }
  deleteState.success = 'Specification removed.'
}
</script>

<template>
  <div class="space-y-6">
    <div
      v-if="active === 'query'"
      class="space-y-4"
    >
      <UCard class="border border-default">
        <template #header>
          <h2 class="text-lg font-semibold">
            Query specifications
          </h2>
        </template>
        <div class="grid gap-4 md:grid-cols-[2fr_1fr_auto]">
          <UFormField label="Title Like">
            <UInput
              v-model="queryForm.titleLike"
              placeholder="Bandwidth"
            />
          </UFormField>
          <UFormField label="Limit">
            <UInput
              v-model.number="queryForm.limit"
              type="number"
              min="1"
              max="50"
            />
          </UFormField>
          <UButton
            color="primary"
            class="self-end"
            @click="querySpecs"
          >
            Query
          </UButton>
        </div>
        <div class="mt-4">
          <FormStatus
            :loading="queryState.loading"
            :error="queryState.error"
            :empty="queryState.empty"
          />
          <UTable
            v-if="queryResults.length"
            :rows="queryResults"
            :columns="[
              { key: 'slug', label: 'Slug' },
              { key: 'title', label: 'Title' },
              { key: 'unit', label: 'Unit' }
            ]"
          />
        </div>
      </UCard>
    </div>

    <div
      v-else-if="active === 'update'"
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

    <div
      v-else-if="active === 'delete'"
      class="grid gap-6 lg:grid-cols-[2fr_1fr]"
    >
      <UCard class="border border-default">
        <template #header>
          <h2 class="text-lg font-semibold">
            Delete specification
          </h2>
        </template>
        <UForm
          class="space-y-4"
          @submit="deleteSpec"
        >
          <UFormField
            label="Slug"
            required
          >
            <UInput
              v-model="deleteForm.slug"
              placeholder="bandwidth"
            />
          </UFormField>
          <UButton
            type="submit"
            color="error"
            variant="solid"
            :loading="deleteState.loading"
          >
            Delete Specification
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
            Note
          </h3>
        </template>
        <p class="text-sm text-muted">
          Specifications referenced by articles cannot be deleted and will return a validation error.
        </p>
      </UCard>
    </div>
  </div>
</template>
