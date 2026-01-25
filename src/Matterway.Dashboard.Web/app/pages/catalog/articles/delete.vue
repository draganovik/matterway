<script setup lang="ts">
import { formatMoney } from '~/utils/format'

definePageMeta({
  title: 'Articles',
  service: 'catalog',
  level: 'operator',
  action: 'delete'
})
</script>

<template>
  <FeatureShell>
    <div class="space-y-6">
      <div
        class="grid gap-6 lg:grid-cols-[2fr_1fr]"
      >
        <UCard class="border border-default">
          <template #header>
            <div>
              <h2 class="text-lg font-semibold">
                Delete article
              </h2>
              <p class="text-sm text-muted">
                Removes the article and associated images.
              </p>
            </div>
          </template>
          <UForm
            class="space-y-4"
            @submit="deleteArticle"
          >
            <div class="flex gap-3">
              <UFormField
                label="Article Id"
                required
                class="flex-1"
              >
                <UInput
                  v-model="deleteForm.id"
                  placeholder="GUID"
                />
              </UFormField>
              <UButton
                color="neutral"
                variant="outline"
                class="self-end"
                @click="loadArticle('delete')"
              >
                Load
              </UButton>
            </div>
            <UButton
              type="submit"
              color="error"
              variant="solid"
              :loading="deleteState.loading"
            >
              Delete Article
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
              Article preview
            </h3>
          </template>
          <div
            v-if="deletePreview"
            class="space-y-2 text-sm"
          >
            <p class="font-semibold">
              {{ deletePreview.title }}
            </p>
            <p class="text-muted">
              Code: {{ deletePreview.code }}
            </p>
            <p class="text-muted">
              Price: {{ formatMoney(deletePreview.price) }}
            </p>
          </div>
          <p
            v-else
            class="text-sm text-muted"
          >
            Load an article to confirm deletion.
          </p>
        </UCard>
      </div>
    </div>
  </FeatureShell>
</template>
