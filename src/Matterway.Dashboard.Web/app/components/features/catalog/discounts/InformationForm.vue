<script setup lang="ts">
type DiscountForm = {
  code: string
  percentage: number | null
  validFrom: string
  validTo: string
}

const { disabled = false } = defineProps<{
  disabled?: boolean
}>()

const form = defineModel<DiscountForm>({ required: true })
</script>

<template>
  <div class="grid gap-4 md:grid-cols-2">
    <UFormField
      label="Kod"
      required
      help="3-50 karaktera, velika slova, cifre, donja crta ili crtica."
    >
      <UInput
        v-model="form.code"
        placeholder="SPRING25"
        :disabled="disabled"
        class="w-full"
      />
    </UFormField>

    <UFormField
      label="Procenat"
      required
      help="Opseg decimalne vrednosti: 0,01 do 1."
    >
      <UInputNumber
        v-model="form.percentage"
        orientation="vertical"
        :min="0.01"
        :max="1"
        :step="0.01"
        variant="outline"
        placeholder="0.15"
        :disabled="disabled"
        class="w-full"
        :ui="{ root: 'w-full', base: 'w-full text-left' }"
      />
    </UFormField>

    <UFormField label="Važi od" required>
      <UInput
        v-model="form.validFrom"
        type="datetime-local"
        placeholder="YYYY-MM-DDTHH:mm"
        :disabled="disabled"
        class="w-full"
      />
    </UFormField>

    <UFormField label="Važi do">
      <UInput
        v-model="form.validTo"
        type="datetime-local"
        placeholder="YYYY-MM-DDTHH:mm"
        :disabled="disabled"
        class="w-full"
      />
    </UFormField>
  </div>
</template>
