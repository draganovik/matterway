<script setup lang="ts">
definePageMeta({
  title: 'Article Images',
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
            Remove image
          </h2>
        </template>
        <UForm
          class="space-y-4"
          @submit="deleteImage"
        >
          <UFormField
            label="Article Id"
            required
          >
            <UInput
              v-model="deleteForm.articleId"
              placeholder="GUID"
              @blur="loadImages(deleteForm.articleId)"
            />
          </UFormField>
          <UFormField
            label="Order Index"
            required
          >
            <UInput
              v-model.number="deleteForm.orderIndex"
              type="number"
              min="0"
            />
          </UFormField>
          <UButton
            type="submit"
            color="error"
            variant="solid"
            :loading="deleteState.loading"
          >
            Remove Image
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
            Current images
          </h3>
        </template>
        <FormStatus
          :loading="previewState.loading"
          :error="previewState.error"
        />
        <div
          v-if="imagePreview.length"
          class="space-y-3"
        >
          <div
            v-for="image in imagePreview"
            :key="image.id"
            class="flex items-center gap-3 rounded-lg border border-default p-3 text-sm"
          >
            <img
              v-if="image.imageUrl"
              :src="image.imageUrl"
              :alt="image.imageAlt || ''"
              class="h-12 w-12 rounded object-cover"
            >
            <div>
              <p class="font-semibold">
                Order {{ image.orderIndex }}
              </p>
              <p class="text-muted">
                {{ image.imageAlt || 'No alt text' }}
              </p>
            </div>
          </div>
        </div>
        <p
          v-else
          class="text-sm text-muted"
        >
          Load an article to view images.
        </p>
      </UCard>
    </div>
  </FeatureShell>
</template>
