<script setup lang="ts">
definePageMeta({
  title: 'Article Details',
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
            Remove detail
          </h2>
        </template>
        <UForm
          class="space-y-4"
          @submit="deleteDetail"
        >
          <UFormField
            label="Article Id"
            required
          >
            <UInput
              v-model="deleteForm.articleId"
              placeholder="GUID"
              @blur="loadArticleDetails(deleteForm.articleId)"
            />
          </UFormField>
          <UFormField
            label="Detail Slug"
            required
          >
            <UInput
              v-model="deleteForm.detailSlug"
              placeholder="detail-slug"
            />
          </UFormField>
          <UButton
            type="submit"
            color="error"
            variant="solid"
            :loading="deleteState.loading"
          >
            Remove Detail
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
            Article details
          </h3>
        </template>
        <FormStatus
          :loading="previewState.loading"
          :error="previewState.error"
        />
        <div
          v-if="articlePreview?.details?.length"
          class="space-y-2 text-sm"
        >
          <div
            v-for="detail in articlePreview.details"
            :key="detail.detailSlug"
            class="rounded-lg border border-default px-3 py-2"
          >
            <p class="font-semibold">
              {{ detail.title || detail.detailSlug }}
            </p>
            <p class="text-muted">
              {{ detail.value }}
            </p>
          </div>
        </div>
        <p
          v-else
          class="text-sm text-muted"
        >
          Load an article to see details.
        </p>
      </UCard>
    </div>
  </FeatureShell>
</template>
