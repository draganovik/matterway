<script setup lang="ts">
import { useApiClient } from '~/composables/useApiClient'
import { formatMoney } from '~/utils/format'

definePageMeta({
  title: 'Articles',
  service: 'catalog',
  level: 'operator',
  action: 'create'
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
                Create article
              </h2>
              <p class="text-sm text-muted">
                Operator permission required.
              </p>
            </div>
          </template>
          <UForm
            class="space-y-4"
            @submit="createArticle"
          >
            <div class="grid gap-4 md:grid-cols-2">
              <UFormField
                label="Article Code"
                required
              >
                <UInput
                  v-model="createForm.articleCode"
                  placeholder="ABC123"
                />
              </UFormField>
              <UFormField
                label="Base Price (RSD)"
                required
              >
                <UInput
                  v-model.number="createForm.basePrice"
                  type="number"
                  min="0.01"
                  step="0.01"
                />
              </UFormField>
            </div>
            <UFormField
              label="Title"
              required
            >
              <UInput
                v-model="createForm.title"
                placeholder="Router Pro X"
              />
            </UFormField>
            <UFormField
              label="Description"
              required
            >
              <UTextarea
                v-model="createForm.description"
                :rows="4"
              />
            </UFormField>
            <UFormField label="Available">
              <USwitch v-model="createForm.isAvailable" />
            </UFormField>
            <UButton
              type="submit"
              color="primary"
              :loading="createState.loading"
            >
              Create Article
            </UButton>
            <FormStatus
              :error="createState.error"
              :success="createState.success"
            />
          </UForm>
        </UCard>
        <UCard class="border border-default bg-elevated/40">
          <template #header>
            <h3 class="text-sm font-semibold text-muted">
              Tips
            </h3>
          </template>
          <ul class="space-y-2 text-sm text-muted">
            <li>Article codes must be 5-10 uppercase letters or numbers.</li>
            <li>Base price must be greater than 0.</li>
            <li>Availability controls public storefront visibility.</li>
          </ul>
        </UCard>
      </div>
    </div>
  </FeatureShell>
</template>
