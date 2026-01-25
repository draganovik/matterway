<script setup lang="ts">
import { useApiClient } from '~/composables/useApiClient'
import { formatMoney } from '~/utils/format'

definePageMeta({
  title: 'Articles',
  service: 'catalog',
  level: 'operator',
  action: 'update'
})
</script>

<template>
  <FeatureShell>
    <div class="space-y-6">
      <div
        class="space-y-6"
      >
        <UCard class="border border-default">
          <template #header>
            <div>
              <h2 class="text-lg font-semibold">
                Update article
              </h2>
              <p class="text-sm text-muted">
                Load an article, modify fields, then update.
              </p>
            </div>
          </template>
          <div class="grid gap-6 lg:grid-cols-[2fr_1fr]">
            <UForm
              class="space-y-4"
              @submit="updateArticle"
            >
              <div class="flex gap-3">
                <UFormField
                  label="Article Id"
                  required
                  class="flex-1"
                >
                  <UInput
                    v-model="updateForm.id"
                    placeholder="GUID"
                  />
                </UFormField>
                <UButton
                  color="neutral"
                  variant="outline"
                  class="self-end"
                  @click="loadArticle('update')"
                >
                  Load
                </UButton>
              </div>
              <div class="grid gap-4 md:grid-cols-2">
                <UFormField label="Article Code">
                  <UInput
                    v-model="updateForm.articleCode"
                    placeholder="ABC123"
                  />
                </UFormField>
                <UFormField label="Base Price (RSD)">
                  <UInput
                    v-model.number="updateForm.basePrice"
                    type="number"
                    min="0.01"
                    step="0.01"
                  />
                </UFormField>
              </div>
              <UFormField label="Title">
                <UInput
                  v-model="updateForm.title"
                  placeholder="Router Pro X"
                />
              </UFormField>
              <UFormField label="Description">
                <UTextarea
                  v-model="updateForm.description"
                  :rows="4"
                />
              </UFormField>
              <UFormField label="Available">
                <USwitch v-model="updateForm.isAvailable" />
              </UFormField>
              <UButton
                type="submit"
                color="primary"
                :loading="updateState.loading"
              >
                Update Article
              </UButton>
              <FormStatus
                :error="updateState.error"
                :success="updateState.success"
              />
            </UForm>
            <UCard class="border border-default bg-elevated/40">
              <template #header>
                <h3 class="text-sm font-semibold text-muted">
                  Loaded article
                </h3>
              </template>
              <div
                v-if="updatePreview"
                class="space-y-2 text-sm"
              >
                <p class="font-semibold">
                  {{ updatePreview.title }}
                </p>
                <p class="text-muted">
                  Code: {{ updatePreview.code }}
                </p>
                <p class="text-muted">
                  Price: {{ formatMoney(updatePreview.price) }}
                </p>
                <p class="text-muted">
                  Available: {{ updatePreview.isAvailable ? 'Yes' : 'No' }}
                </p>
              </div>
              <p
                v-else
                class="text-sm text-muted"
              >
                Load an article to preview.
              </p>
            </UCard>
          </div>
        </UCard>
      </div>
    </div>
  </FeatureShell>
</template>
