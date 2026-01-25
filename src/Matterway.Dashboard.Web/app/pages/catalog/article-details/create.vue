<script setup lang="ts">
definePageMeta({
  title: 'Article Details',
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
            Add detail
          </h2>
        </template>
        <UForm
          class="space-y-4"
          @submit="createDetail"
        >
          <UFormField
            label="Article Id"
            required
          >
            <UInput
              v-model="createForm.articleId"
              placeholder="GUID"
              @blur="loadArticleDetails(createForm.articleId)"
            />
          </UFormField>
          <UFormField
            label="Detail Slug"
            required
          >
            <USelectMenu
              v-model="createForm.detailSlug"
              :items="detailOptions"
              value-attribute="slug"
              option-attribute="title"
              placeholder="Select detail"
            />
          </UFormField>
          <UFormField
            label="Value"
            required
          >
            <UInput v-model="createForm.value" />
          </UFormField>
          <div class="flex flex-wrap gap-3">
            <UButton
              type="submit"
              color="primary"
              :loading="createState.loading"
            >
              Add Detail
            </UButton>
            <UButton
              color="neutral"
              variant="outline"
              @click="loadDetails"
            >
              Load Detail Definitions
            </UButton>
          </div>
          <FormStatus
            :error="createState.error"
            :success="createState.success"
          />
          <FormStatus
            :loading="detailState.loading"
            :error="detailState.error"
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
