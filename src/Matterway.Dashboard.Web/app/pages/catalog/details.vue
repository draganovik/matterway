<script setup lang="ts">
import { useApiClient } from '~/composables/useApiClient'
import { useFeatureTabs } from '~/composables/useFeatureTabs'

definePageMeta({
  title: 'Details',
  service: 'catalog',
  level: 'observer'
})

const api = useApiClient()
const { tabs, active } = useFeatureTabs()

const queryForm = reactive({
  limit: 10,
  titleLike: ''
})
const queryState = reactive({ loading: false, error: '', empty: '' })
const queryResults = ref<any[]>([])

const upsertForm = reactive({
  slug: '',
  title: ''
})
const upsertState = reactive({ loading: false, error: '', success: '' })

const deleteForm = reactive({
  slug: ''
})
const deleteState = reactive({ loading: false, error: '', success: '' })

async function queryDetails() {
  queryState.loading = true
  queryState.error = ''
  queryState.empty = ''
  queryResults.value = []
  const params = new URLSearchParams({
    limit: queryForm.limit.toString()
  })
  if (queryForm.titleLike) params.set('titleLike', queryForm.titleLike)
  const result = await api.request<any>('catalog', `admin/details?${params.toString()}`)
  queryState.loading = false
  if (!result.ok) {
    queryState.error = result.error || 'Failed to query details.'
    return
  }
  queryResults.value = result.data || []
  if (!queryResults.value.length) queryState.empty = 'No details found.'
}

async function upsertDetail() {
  upsertState.loading = true
  upsertState.error = ''
  upsertState.success = ''
  const result = await api.request<any>(
    'catalog',
    `admin/details/${upsertForm.slug}`,
    {
      method: 'PUT',
      body: JSON.stringify({ title: upsertForm.title })
    }
  )
  upsertState.loading = false
  if (!result.ok) {
    upsertState.error = result.error || 'Failed to upsert detail.'
    return
  }
  upsertState.success = 'Detail saved.'
}

async function deleteDetail() {
  deleteState.loading = true
  deleteState.error = ''
  deleteState.success = ''
  const result = await api.request<any>('catalog', `admin/details/${deleteForm.slug}`, {
    method: 'DELETE'
  })
  deleteState.loading = false
  if (!result.ok) {
    deleteState.error = result.error || 'Failed to delete detail.'
    return
  }
  deleteState.success = 'Detail removed.'
}
</script>

<template>
  <div class="space-y-6">
    <UCard class="border border-slate-200/60">
      <div>
        <p class="text-xs uppercase tracking-[0.3em] text-emerald-600">Catalog</p>
        <h1 class="text-2xl font-semibold text-slate-900">Details</h1>
        <p class="text-sm text-muted">Manage detail definitions.</p>
      </div>
    </UCard>

    <FeatureTabs v-model="active" :tabs="tabs" />

    <div v-if="active === 'query'" class="space-y-4">
      <UCard class="border border-slate-200/60">
        <template #header>
          <h2 class="text-lg font-semibold">Query details</h2>
        </template>
        <div class="grid gap-4 md:grid-cols-[2fr_1fr_auto]">
          <UFormField label="Title Like">
            <UInput v-model="queryForm.titleLike" placeholder="Connectivity" />
          </UFormField>
          <UFormField label="Limit">
            <UInput v-model.number="queryForm.limit" type="number" min="1" max="50" />
          </UFormField>
          <UButton color="primary" class="self-end" @click="queryDetails">
            Query
          </UButton>
        </div>
        <div class="mt-4">
          <FormStatus :loading="queryState.loading" :error="queryState.error" :empty="queryState.empty" />
          <UTable
            v-if="queryResults.length"
            :rows="queryResults"
            :columns="[
              { key: 'slug', label: 'Slug' },
              { key: 'title', label: 'Title' }
            ]"
          />
        </div>
      </UCard>
    </div>

    <div v-else-if="active === 'update'" class="grid gap-6 lg:grid-cols-[2fr_1fr]">
      <UCard class="border border-slate-200/60">
        <template #header>
          <h2 class="text-lg font-semibold">Create/Replace detail</h2>
        </template>
        <UForm @submit="upsertDetail" class="space-y-4">
          <UFormField label="Slug" required>
            <UInput v-model="upsertForm.slug" placeholder="features" />
          </UFormField>
          <UFormField label="Title" required>
            <UInput v-model="upsertForm.title" placeholder="Features" />
          </UFormField>
          <UButton type="submit" color="primary" :loading="upsertState.loading">
            Save Detail
          </UButton>
          <FormStatus :error="upsertState.error" :success="upsertState.success" />
        </UForm>
      </UCard>
      <UCard class="border border-slate-200/60 bg-white/80">
        <template #header>
          <h3 class="text-sm font-semibold text-muted">Behavior</h3>
        </template>
        <p class="text-sm text-muted">
          PUT creates the slug if missing or replaces the title if it already exists.
        </p>
      </UCard>
    </div>

    <div v-else-if="active === 'delete'" class="grid gap-6 lg:grid-cols-[2fr_1fr]">
      <UCard class="border border-slate-200/60">
        <template #header>
          <h2 class="text-lg font-semibold">Delete detail</h2>
        </template>
        <UForm @submit="deleteDetail" class="space-y-4">
          <UFormField label="Slug" required>
            <UInput v-model="deleteForm.slug" placeholder="features" />
          </UFormField>
          <UButton type="submit" color="red" variant="solid" :loading="deleteState.loading">
            Delete Detail
          </UButton>
          <FormStatus :error="deleteState.error" :success="deleteState.success" />
        </UForm>
      </UCard>
      <UCard class="border border-slate-200/60 bg-white/80">
        <template #header>
          <h3 class="text-sm font-semibold text-muted">Note</h3>
        </template>
        <p class="text-sm text-muted">
          Details referenced by articles cannot be deleted and will return a validation error.
        </p>
      </UCard>
    </div>
  </div>
</template>
