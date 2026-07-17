<script setup lang="ts">
import { useCatalogArchivePage } from "~/composables/features/catalog/useCatalogArchivePage"

definePageMeta({
  title: "Arhiva kataloga",
})

const {
  canOperate,
  exportState,
  importState,
  hasSelectedImportFile,
  selectedImportFileName,
  setImportFile,
  exportArchive,
  importArchive,
  clearImportFileSelection,
} = useCatalogArchivePage()
</script>

<template>
  <UDashboardPanel
    id="catalog-archive"
    :ui="{
      body: 'min-h-0 overflow-hidden py-3',
    }"
  >
    <template #header>
      <UDashboardNavbar title="Arhiva kataloga">
        <template #leading>
          <UDashboardSidebarCollapse />
        </template>

        <template #trailing>
          <PageInfoTooltip
            text="Otpremite prethodno izvezenu ZIP arhivu da zamenite trenutne podatke kataloga i slike."
          />
        </template>

        <template #right>
          <UButton
            color="primary"
            icon="i-lucide-download"
            :loading="exportState.loading"
            :disabled="!canOperate"
            @click="exportArchive"
          >
            {{ exportState.loading ? "Priprema arhive" : "Preuzmi arhivu" }}
          </UButton>
        </template>
      </UDashboardNavbar>
    </template>

    <template #body>
      <div class="h-full min-h-0 overflow-y-auto">
        <div class="w-full max-w-3xl space-y-4">
          <StatusMessages
            v-if="exportState.error || exportState.success"
            :error="exportState.error"
            :success="exportState.success"
          />

          <CatalogArchiveImportPanel
            :can-operate="canOperate"
            :loading="importState.loading"
            :error="importState.error"
            :success="importState.success"
            :has-selected-file="hasSelectedImportFile"
            :selected-file-name="selectedImportFileName"
            @select-file="setImportFile"
            @upload="importArchive"
            @clear="clearImportFileSelection"
          />
        </div>
      </div>
    </template>
  </UDashboardPanel>
</template>
