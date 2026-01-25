<script setup lang="ts">
import { useApiClient } from '~/composables/useApiClient'

definePageMeta({
  title: 'Article Specifications',
  service: 'catalog',
  level: 'operator',
  action: 'delete'
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
            Remove specification
          </h2>
        </template>
        <UForm
          class="space-y-4"
          @submit="deleteSpec"
        >
          <UFormField
            label="Article Id"
            required
          >
            <UInput
              v-model="deleteForm.articleId"
              placeholder="GUID"
              @blur="loadArticleSpecs(deleteForm.articleId)"
            />
          </UFormField>
          <UFormField
            label="Specification Slug"
            required
          >
            <UInput
              v-model="deleteForm.specificationSlug"
              placeholder="spec-slug"
            />
          </UFormField>
          <UButton
            type="submit"
            color="error"
            variant="solid"
            :loading="deleteState.loading"
          >
            Remove Specification
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
