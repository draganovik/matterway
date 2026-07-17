<script setup lang="ts">
const isOpen = defineModel<boolean>("open", { required: true })

const props = withDefaults(
  defineProps<{
    title: string
    description?: string
    subject?: string
    confirmLabel?: string
    cancelLabel?: string
    loading?: boolean
    error?: string
  }>(),
  {
    description: "",
    subject: "",
    confirmLabel: "Obriši",
    cancelLabel: "Otkaži",
    loading: false,
    error: "",
  },
)

const emit = defineEmits<{
  confirm: []
}>()

function close() {
  isOpen.value = false
}
</script>

<template>
  <UModal
    v-model:open="isOpen"
    :ui="{
      content: 'sm:max-w-lg',
    }"
  >
    <template #header>
      <div class="flex items-center gap-3">
        <div
          class="bg-error/10 text-error ring-error/15 flex size-10 shrink-0 items-center justify-center rounded-full ring-1 ring-inset"
        >
          <UIcon name="i-lucide-trash-2" class="size-5" />
        </div>

        <div class="min-w-0 space-y-0.5">
          <h3 class="text-highlighted text-base leading-tight font-semibold">
            {{ props.title }}
          </h3>
          <p v-if="props.description" class="text-muted text-sm">
            {{ props.description }}
          </p>
        </div>
      </div>
    </template>

    <template #body>
      <div class="space-y-4">
        <div class="space-y-3">
          <p class="text-highlighted text-sm font-medium">
            Ova radnja je trajna i ne može da se poništi.
          </p>
          <p class="text-muted text-sm">
            Potvrdite brisanje samo ako ste sigurni da zapis više ne treba da
            postoji.
          </p>
          <div
            v-if="props.subject"
            class="border-default/70 bg-default text-highlighted inline-flex max-w-full items-center gap-2 rounded-full border px-3 py-1 text-sm font-medium shadow-sm"
          >
            <UIcon name="i-lucide-trash-2" class="text-error size-4 shrink-0" />
            <span class="min-w-0 truncate">{{ props.subject }}</span>
          </div>
        </div>

        <StatusMessages v-if="props.error" :error="props.error" />
      </div>
    </template>

    <template #footer>
      <div
        class="flex w-full flex-col-reverse items-stretch gap-3 sm:flex-row sm:items-center sm:justify-end"
      >
        <UButton variant="ghost" :disabled="props.loading" @click="close">
          {{ props.cancelLabel }}
        </UButton>
        <UButton
          color="error"
          :loading="props.loading"
          :disabled="props.loading"
          @click="emit('confirm')"
        >
          {{ props.confirmLabel }}
        </UButton>
      </div>
    </template>
  </UModal>
</template>
