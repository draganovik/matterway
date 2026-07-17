<script setup lang="ts">
type DiscountForm = {
  code: string
  percentage: number | null
  validFrom: string
  validTo: string
}

const form = defineModel<DiscountForm>({ required: true })
const articleCodes = defineModel<string[]>("articleCodes", { required: true })

withDefaults(
  defineProps<{
    selected?: boolean
    canEdit?: boolean
    saveLoading?: boolean
    deleteLoading?: boolean
    error?: string
    success?: string
  }>(),
  {
    selected: false,
    canEdit: false,
    saveLoading: false,
    deleteLoading: false,
    error: "",
    success: "",
  },
)

const emit = defineEmits<{
  (event: "save" | "remove"): void
}>()
</script>

<template>
  <div class="space-y-4">
    <div class="space-y-1">
      <h3 class="text-foreground text-base font-semibold">
        {{ selected ? "Izmena popusta" : "Uređivanje popusta" }}
      </h3>
      <p class="text-muted text-sm">
        Izaberite popust sa liste da biste uredili povezane artikle i period
        važenja.
      </p>
    </div>

    <EntitiesEmptyState
      v-if="!selected"
      title="Ništa nije izabrano"
      description="Izaberite popust sa liste da biste započeli izmenu."
    />

    <div v-else class="grid gap-4">
      <CatalogDiscountsInformationForm
        v-model="form"
        :disabled="!canEdit || saveLoading || deleteLoading"
      />

      <CatalogDiscountsArticleSelectionPanel
        v-model="articleCodes"
        :can-edit="canEdit && !saveLoading && !deleteLoading"
      />

      <div class="flex flex-wrap items-center gap-3">
        <UButton
          color="primary"
          :loading="saveLoading"
          :disabled="!canEdit || deleteLoading"
          @click="emit('save')"
        >
          {{ saveLoading ? "Čuvanje popusta" : "Sačuvaj popust" }}
        </UButton>
        <UButton
          color="error"
          variant="outline"
          :loading="deleteLoading"
          :disabled="!canEdit || saveLoading"
          @click="emit('remove')"
        >
          {{ deleteLoading ? "Brisanje popusta" : "Obriši popust" }}
        </UButton>
      </div>

      <StatusMessages :error="error" :success="success" />
    </div>
  </div>
</template>
