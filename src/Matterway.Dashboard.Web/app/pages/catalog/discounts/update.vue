<script setup lang="ts">
import { useApiClient } from '~/composables/useApiClient'
import { useRequestState } from '~/composables/useRequestState'
import { useDiscountSelection } from '~/composables/useDiscountSelection'

definePageMeta({
  title: 'Discounts',
  service: 'catalog',
  level: 'operator',
  action: 'update'
})

const api = useApiClient()
const updateState = useRequestState()

const { updateIds, removeArticleId, setArticleIds } = useDiscountSelection()

const updateForm = reactive({
  code: '',
  percentage: 0.1,
  validFrom: '',
  validTo: '',
  articleIds: [] as string[]
})

watch(updateIds, (ids) => {
  updateForm.articleIds = [...ids]
}, { immediate: true })

function splitArticleIds(value: string) {
  const ids = value
    .split(/[\n,]+/)
    .map(item => item.trim())
    .filter(Boolean)
  setArticleIds('update', ids)
  return ids
}

async function updateDiscount() {
  updateState.error = ''
  updateState.success = ''
  updateState.loading = true
  try {
    const result = await api.request(
      'catalog',
      `admin/discounts/${updateForm.code}`,
      {
        method: 'PUT',
        body: JSON.stringify({
          percentage: updateForm.percentage,
          validFrom: updateForm.validFrom,
          validTo: updateForm.validTo || null,
          articleIds: updateForm.articleIds
        })
      }
    )
    if (!result.ok) {
      updateState.error = result.error || 'Failed to update discounts.'
      return
    }
    updateState.success = 'Discounts updated.'
  } catch (err) {
    updateState.error = err instanceof Error ? err.message : 'Failed to update discounts.'
  } finally {
    updateState.loading = false
  }
}
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
              Update discounts
            </h2>
          </template>
          <UForm
            class="space-y-4"
            @submit="updateDiscount"
          >
            <UFormField
              label="Code"
              required
            >
              <UInput
                v-model="updateForm.code"
                placeholder="SUMMER24"
              />
            </UFormField>
            <div class="grid gap-4 md:grid-cols-2">
              <UFormField
                label="Percentage (0-1)"
                required
              >
                <UInput
                  v-model.number="updateForm.percentage"
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
                  v-model="updateForm.validFrom"
                  type="datetime-local"
                />
              </UFormField>
            </div>
            <UFormField label="Valid To">
              <UInput
                v-model="updateForm.validTo"
                type="datetime-local"
              />
            </UFormField>
            <UFormField
              label="Article Ids"
              required
            >
              <UTextarea
                :rows="2"
                :model-value="updateForm.articleIds.join(', ')"
                placeholder="GUID, GUID"
                @update:model-value="(value: string) => (updateForm.articleIds = splitArticleIds(value))"
              />
            </UFormField>
            <UButton
              type="submit"
              color="primary"
              :loading="updateState.loading"
            >
              Update Discounts
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
              Selected Articles
            </h3>
          </template>
          <div
            v-if="updateForm.articleIds.length"
            class="space-y-2 text-sm"
          >
            <div
              v-for="articleId in updateForm.articleIds"
              :key="articleId"
              class="flex items-center justify-between rounded border border-default px-3 py-2"
            >
              <span class="font-mono text-xs">{{ articleId }}</span>
              <UButton
                size="xs"
                color="error"
                variant="ghost"
                @click="removeArticleId('update', articleId)"
              >
                Remove
              </UButton>
            </div>
          </div>
          <p
            v-else
            class="text-sm text-muted"
          >
            Add article IDs to update.
          </p>
        </UCard>
      </div>
    </div>
  </FeatureShell>
</template>
