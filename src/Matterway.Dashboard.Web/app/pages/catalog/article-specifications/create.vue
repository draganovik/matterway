<script setup lang="ts">
import { useApiClient } from '~/composables/useApiClient'

definePageMeta({
  title: 'Article Specifications',
  service: 'catalog',
  level: 'operator',
  action: 'create'
})
</script>

<template>
  <FeatureShell>
    <div class="grid gap-6 lg:grid-cols-[2fr_1fr]">
      <UCard
        class="border border-default"
      >
        <template #header>
          <h2 class="text-lg font-semibold">
            Add specification
          </h2>
        </template>
        <UForm
          class="space-y-4"
          @submit="createSpec"
        >
          <UFormField
            label="Article Id"
            required
          >
            <UInput
              v-model="createForm.articleId"
              placeholder="GUID"
              @blur="loadArticleSpecs(createForm.articleId)"
            />
          </UFormField>
          <UFormField
            label="Specification"
            required
          >
            <USelectMenu
              v-model="createForm.specificationSlug"
              :items="specOptions"
              value-attribute="slug"
              option-attribute="title"
              placeholder="Select specification"
            />
          </UFormField>
          <UFormField
            label="Value"
            required
          >
            <UInput
              v-model.number="createForm.value"
              type="number"
              step="0.01"
            />
          </UFormField>
          <div class="flex flex-wrap gap-3">
            <UButton
              type="submit"
              color="primary"
              :loading="createState.loading"
            >
              Add Specification
            </UButton>
            <UButton
              color="neutral"
              variant="outline"
              @click="loadSpecs"
            >
              Load Specifications
            </UButton>
          </div>
          <FormStatus
            :error="createState.error"
            :success="createState.success"
          />
          <FormStatus
            :loading="specState.loading"
            :error="specState.error"
          />
        </UForm>
      </UCard>

      <UCard class="border border-default bg-elevated/40">
        <template #header>
          <h3 class="text-sm font-semibold text-muted">
            Article specifications
          </h3>
        </template>
        <FormStatus
          :loading="previewState.loading"
          :error="previewState.error"
        />
        <div
          v-if="articlePreview?.specifications?.length"
          class="space-y-2 text-sm"
        >
          <div
            v-for="spec in articlePreview.specifications"
            :key="spec.specificationSlug"
            class="rounded-lg border border-default px-3 py-2"
          >
            <p class="font-semibold">
              {{ spec.title || spec.specificationSlug }}
            </p>
            <p class="text-muted">
              {{ spec.value }} {{ spec.unit || '' }}
            </p>
          </div>
        </div>
        <p
          v-else
          class="text-sm text-muted"
        >
          Load an article to see specifications.
        </p>
      </UCard>
    </div>
  </FeatureShell>
</template>
