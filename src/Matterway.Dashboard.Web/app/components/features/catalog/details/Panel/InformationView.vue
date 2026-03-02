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
        {{ detail ? "Edit Detail" : "Detail Editor" }}
      </h3>
      <p class="text-muted text-sm">
        {{
          canEdit
            ? "Operator permission is required for create, update, and delete."
            : "Read-only mode: operator permission required for changes."
        }}
      </p>
    </div>

    <EntitiesEmptyState
      v-if="!detail"
      title="Nothing selected"
      description="Select an item from the list to start editing."
    />

    <div v-else class="grid gap-4">
      <UFormField
        label="Slug"
        required
        help="Lowercase key used in article details."
      >
        <UInput
          :model-value="slug"
          placeholder="screen-size"
          :disabled="!canEdit || canDelete"
          class="w-full"
          @update:model-value="emit('update:slug', $event)"
        />
      </UFormField>

      <UFormField label="Title" required>
        <UInput
          :model-value="title"
          placeholder="Screen Size"
          :disabled="!canEdit"
          class="w-full"
          @update:model-value="emit('update:title', $event)"
        />
      </UFormField>

      <UFormField
        label="Unit"
        help="Optional unit for numeric values (e.g. cm, kg)."
      >
        <UInput
          :model-value="unit"
          placeholder="inch"
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
          Update Detail
        </UButton>

        <UButton
          v-if="canDelete"
          color="error"
          variant="ghost"
          :loading="removeLoading"
          :disabled="!canEdit"
          @click="emit('remove')"
        >
          Delete Detail
        </UButton>
      </div>

      <StatusMessages :error="error" :success="success" />
    </div>
  </div>
</template>
