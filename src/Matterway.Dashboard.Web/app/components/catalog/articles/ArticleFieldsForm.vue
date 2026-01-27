<script setup lang="ts">
type ArticleForm = {
  articleCode: string
  title: string
  basePrice: number | string
  description: string
  isAvailable: boolean
}

const props = withDefaults(defineProps<{
  modelValue: ArticleForm
  disabled?: boolean
}>(), {
  disabled: false
})

const emit = defineEmits<{
  (event: 'update:modelValue', value: ArticleForm): void
}>()

const form = reactive<ArticleForm>({
  articleCode: '',
  title: '',
  basePrice: '',
  description: '',
  isAvailable: true
})

watch(
  () => props.modelValue,
  (value) => {
    if (!value) return
    form.articleCode = value.articleCode
    form.title = value.title
    form.basePrice = value.basePrice
    form.description = value.description
    form.isAvailable = value.isAvailable
  },
  { immediate: true }
)

watch(
  form,
  () => {
    emit('update:modelValue', { ...form })
  },
  { deep: true }
)
</script>

<template>
  <div class="grid gap-4">
    <div class="grid gap-4 md:grid-cols-2">
      <UFormField
        label="Article Code"
        required
        help="5-10 uppercase letters or numbers"
      >
        <UInput
          v-model="form.articleCode"
          placeholder="ABCDE"
          size="md"
          :disabled="disabled"
        />
      </UFormField>
      <UFormField
        label="Base Price"
        required
      >
        <UInput
          v-model="form.basePrice"
          type="number"
          min="0.01"
          step="0.01"
          placeholder="0.00"
          size="md"
          :disabled="disabled"
        />
      </UFormField>
    </div>

    <UFormField
      label="Title"
      required
    >
      <UInput
        v-model="form.title"
        placeholder="Article title"
        size="md"
        :disabled="disabled"
      />
    </UFormField>

    <UFormField
      label="Description"
      required
    >
      <UTextarea
        v-model="form.description"
        :rows="5"
        placeholder="Describe the article"
        size="md"
        :disabled="disabled"
      />
    </UFormField>

    <UFormField label="Available">
      <USwitch
        v-model="form.isAvailable"
        size="md"
        :disabled="disabled"
      />
    </UFormField>
  </div>
</template>
