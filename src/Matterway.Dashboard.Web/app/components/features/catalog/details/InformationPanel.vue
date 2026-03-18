<script setup lang="ts">
import type { QueryDetailResponse } from "~/types/catalog"

withDefaults(
  defineProps<{
    detail?: QueryDetailResponse | null
    canEdit?: boolean
    canDelete?: boolean
    slug?: string
    title?: string
    unit?: string
    saveLoading?: boolean
    removeLoading?: boolean
    error?: string
    success?: string
  }>(),
  {
    detail: null,
    canEdit: false,
    canDelete: false,
    slug: "",
    title: "",
    unit: "",
    saveLoading: false,
    removeLoading: false,
    error: "",
    success: "",
  },
)

const emit = defineEmits<{
  (event: "update:slug" | "update:title" | "update:unit", value: string): void
  (event: "save" | "remove"): void
}>()
</script>

<template>
  <div class="space-y-4">
    <div class="space-y-1">
      <h3 class="text-foreground text-base font-semibold">
        {{ detail ? "Izmena detalja" : "Uređivanje detalja" }}
      </h3>
      <p class="text-muted text-sm">
        {{
          canEdit
            ? "Za kreiranje, izmenu i brisanje potrebna je dozvola operatera."
            : "Režim samo za čitanje: za izmene je potrebna dozvola operatera."
        }}
      </p>
    </div>

    <EntitiesEmptyState
      v-if="!detail"
      title="Ništa nije izabrano"
      description="Izaberite stavku sa liste da biste započeli izmenu."
    />

    <div v-else class="grid gap-4">
      <UFormField
        label="Slug"
        required
        help="Ključ malim slovima koji se koristi u detaljima artikla."
      >
        <UInput
          :model-value="slug"
          placeholder="screen-size"
          :disabled="!canEdit || canDelete"
          class="w-full"
          @update:model-value="emit('update:slug', $event)"
        />
      </UFormField>

      <UFormField label="Naziv" required>
        <UInput
          :model-value="title"
          placeholder="Veličina ekrana"
          :disabled="!canEdit"
          class="w-full"
          @update:model-value="emit('update:title', $event)"
        />
      </UFormField>

      <UFormField
        label="Jedinica"
        help="Opciona jedinica za numeričke vrednosti (npr. cm, kg)."
      >
        <UInput
          :model-value="unit"
          placeholder="inč"
          :disabled="!canEdit"
          class="w-full"
          @update:model-value="emit('update:unit', $event)"
        />
      </UFormField>

      <div class="flex flex-wrap items-center gap-3">
        <UButton
          color="primary"
          :loading="saveLoading"
          :disabled="!canEdit"
          @click="emit('save')"
        >
          {{ saveLoading ? "Čuvanje detalja" : "Sačuvaj detalj" }}
        </UButton>

        <UButton
          v-if="canDelete"
          color="error"
          variant="ghost"
          :loading="removeLoading"
          :disabled="!canEdit"
          @click="emit('remove')"
        >
          {{ removeLoading ? "Brisanje detalja" : "Obriši detalj" }}
        </UButton>
      </div>

      <StatusMessages :error="error" :success="success" />
    </div>
  </div>
</template>
