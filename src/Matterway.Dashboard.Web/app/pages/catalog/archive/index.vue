<script setup lang="ts">
import { useCatalogArchivePage } from "~/composables/features/catalog/useCatalogArchivePage"

definePageMeta({
  title: "Arhiva kataloga",
  service: "catalog",
  permissions: ["operator", "manager"],
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
    :ui="{ body: 'py-3 sm:py-4 lg:py-6 min-h-0 overflow-hidden' }"
  >
    <template #header>
      <UDashboardNavbar title="Arhiva kataloga">
        <template #leading>
          <UDashboardSidebarCollapse />
        </template>
      </UDashboardNavbar>
    </template>

    <template #body>
      <div class="h-full min-h-0">
        <div class="flex h-full min-h-0 flex-col gap-4 overflow-y-auto">
          <div>
            <h2 class="text-foreground text-base font-semibold">
              Arhiva kataloga
            </h2>
            <p class="text-muted text-sm">
              Izvezite ili uvezite podatke kataloga i slike artikala kao ZIP
              arhivu.
            </p>
          </div>

          <div class="grid gap-4 lg:grid-cols-2 lg:items-stretch">
            <CatalogArchiveExportPanel
              :can-operate="canOperate"
              :loading="exportState.loading"
              :error="exportState.error"
              @download="exportArchive"
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
      </div>
    </template>
  </UDashboardPanel>
</template>
