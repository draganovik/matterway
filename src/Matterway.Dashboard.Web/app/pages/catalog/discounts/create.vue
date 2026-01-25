<script setup lang="ts">
definePageMeta({
  title: 'Discounts',
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
            <h2 class="text-lg font-semibold">
              Create discounts
            </h2>
          </template>
          <UForm
            class="space-y-4"
            @submit="createDiscount"
          >
            <UFormField
              label="Code"
              required
            >
              <UInput
                v-model="createForm.code"
                placeholder="SUMMER24"
              />
            </UFormField>
            <div class="grid gap-4 md:grid-cols-2">
              <UFormField
                label="Percentage (0-1)"
                required
              >
                <UInput
                  v-model.number="createForm.percentage"
                  type="number"
                  min="0.01"
                  max="1"
                  step="0.01"
                />
              </UFormField>
              <UFormField
                label="Valid From"
                required
              >
                <UInput
                  v-model="createForm.validFrom"
                  type="datetime-local"
                />
              </UFormField>
            </div>
            <UFormField label="Valid To">
              <UInput
                v-model="createForm.validTo"
                type="datetime-local"
              />
            </UFormField>
            <UFormField
              label="Article Ids"
              required
            >
              <UTextarea
                :rows="2"
                :model-value="createForm.articleIds.join(', ')"
                placeholder="GUID, GUID"
                @update:model-value="(value: string) => (createForm.articleIds = splitArticleIds(value))"
              />
            </UFormField>
            <UButton
              type="submit"
              color="primary"
              :loading="createState.loading"
            >
              Create Discounts
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
              Selected Articles
            </h3>
          </template>
          <div
            v-if="createForm.articleIds.length"
            class="space-y-2 text-sm"
          >
            <div
              v-for="articleId in createForm.articleIds"
              :key="articleId"
              class="flex items-center justify-between rounded border border-default px-3 py-2"
            >
              <span class="font-mono text-xs">{{ articleId }}</span>
              <UButton
                size="xs"
                color="error"
                variant="ghost"
                @click="removeArticleId('create', articleId)"
              >
                Remove
              </UButton>
            </div>
          </div>
          <p
            v-else
            class="text-sm text-muted"
          >
            Add article IDs to apply the discount.
          </p>
        </UCard>
      </div>
    </div>
  </FeatureShell>
</template>
