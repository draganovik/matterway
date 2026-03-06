<script setup lang="ts">
type ArticleForm = {
  code: string
  title: string
  basePrice: number | string
  description: string
  isAvailable: boolean
}

const { disabled = false } = defineProps<{
  disabled?: boolean
}>()

const form = defineModel<ArticleForm>({ required: true })

const basePriceValue = computed({
  get: () => {
    const parsed = Number(form.value.basePrice)
    return Number.isFinite(parsed) ? parsed : null
  },
  set: (value: number | null | undefined) => {
    form.value.basePrice = value == null ? "" : value
  },
})
</script>

<template>
  <div class="grid gap-4">
    <UFormField label="Title" required class="md:col-span-3">
      <UInput
        v-model="form.title"
        placeholder="Article title"
        :disabled="disabled"
        size="xl"
        class="w-full"
      />
    </UFormField>

    <UFormField
      label="Article Code"
      required
      help="Exactly 8 uppercase letters or numbers"
    >
      <UInput
        v-model="form.code"
        placeholder="PHUE0002"
        maxlength="8"
        :disabled="disabled"
        class="w-full"
      />
    </UFormField>

    <UFormField label="Base Price" required>
      <UInputNumber
        v-model="basePriceValue"
        orientation="vertical"
        :min="0.01"
        :step="0.01"
        variant="outline"
        placeholder="0.00"
        :disabled="disabled"
        class="w-full"
        :ui="{ root: 'w-full', base: 'w-full text-left' }"
      />
    </UFormField>

    <UFormField label="Available">
      <USwitch
        v-model="form.isAvailable"
        :disabled="disabled"
        size="xl"
        class="pt-0.5"
      />
    </UFormField>

    <UFormField label="Description" required class="md:col-span-3">
      <UTextarea
        v-model="form.description"
        :rows="5"
        placeholder="Describe the article"
        size="xl"
        :disabled="disabled"
        class="w-full"
      />
    </UFormField>
  </div>
</template>
