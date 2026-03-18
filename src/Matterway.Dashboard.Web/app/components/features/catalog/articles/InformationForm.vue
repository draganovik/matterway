<script setup lang="ts">
type ArticleForm = {
  code: string
  title: string
  basePrice: number | null
  description: string
  isAvailable: boolean
}

const { disabled = false } = defineProps<{
  disabled?: boolean
}>()

const form = defineModel<ArticleForm>({ required: true })

const basePriceValue = computed({
  get: () => form.value.basePrice,
  set: (value: number | null | undefined) => {
    form.value.basePrice = value == null ? null : value
  },
})
</script>

<template>
  <div class="grid gap-4">
    <UFormField label="Naziv" required class="md:col-span-3">
      <UInput
        v-model="form.title"
        placeholder="Naziv artikla"
        :disabled="disabled"
        size="xl"
        class="w-full"
      />
    </UFormField>

    <UFormField
      label="Šifra artikla"
      required
      help="Tačno 8 velikih slova ili cifara"
    >
      <UInput
        v-model="form.code"
        placeholder="PHUE0002"
        maxlength="8"
        :disabled="disabled"
        class="w-full"
      />
    </UFormField>

    <UFormField label="Osnovna cena" required>
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

    <UFormField label="Dostupan">
      <USwitch
        v-model="form.isAvailable"
        :disabled="disabled"
        size="xl"
        class="pt-0.5"
      />
    </UFormField>

    <UFormField label="Opis" required class="md:col-span-3">
      <UTextarea
        v-model="form.description"
        :rows="5"
        placeholder="Opišite artikal"
        size="xl"
        :disabled="disabled"
        class="w-full"
      />
    </UFormField>
  </div>
</template>
