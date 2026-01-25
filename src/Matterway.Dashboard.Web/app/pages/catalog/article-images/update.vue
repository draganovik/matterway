<script setup lang="ts">
import { useApiClient } from '~/composables/useApiClient'

definePageMeta({
  title: 'Article Images',
  service: 'catalog',
  level: 'operator',
  action: 'update'
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
            Update image
          </h2>
        </template>
        <UForm
          class="space-y-4"
          @submit="updateImage"
        >
          <UFormField
            label="Article Id"
            required
          >
            <UInput
              v-model="updateForm.articleId"
              placeholder="GUID"
              @blur="loadImages(updateForm.articleId)"
            />
          </UFormField>
          <UFormField
            label="Current Order Index"
            required
          >
            <UInput
              v-model.number="updateForm.orderIndex"
              type="number"
              min="0"
            />
          </UFormField>
          <UFormField label="New Order Index">
            <UInput
              v-model.number="updateForm.newOrderIndex"
              type="number"
              min="0"
            />
          </UFormField>
          <UFormField label="Image Alt">
            <UInput v-model="updateForm.imageAlt" />
          </UFormField>
          <UButton
            type="submit"
            color="primary"
            :loading="updateState.loading"
          >
            Update Image
          </UButton>
          <FormStatus
            :error="updateState.error"
            :success="updateState.success"
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
