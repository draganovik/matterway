<script setup lang="ts">
import { useAuthSession } from "~/composables/useAuthSession"
import { useCatalogApi } from "~/composables/useCatalogApi"
import { useRequestState } from "~/composables/useRequestState"
import type { ImportCatalogArchiveResponse } from "~/types/catalog"

const auth = useAuthSession()
const api = useCatalogApi()

const canOperate = computed(() =>
  auth.hasPermission("catalog", ["operator", "manager"]),
)

const exportState = useRequestState()
const importState = useRequestState()

const importFile = ref<File | null>(null)
const hasSelectedImportFile = computed(() => Boolean(importFile.value))
const selectedImportFileName = computed(() => importFile.value?.name || "")

function resetExportMessages() {
  exportState.error = ""
  exportState.success = ""
}

function resetImportMessages() {
  importState.error = ""
  importState.success = ""
}

function clearImportFileSelection() {
  importFile.value = null
  resetImportMessages()
}

function setImportFile(file: File | null) {
  resetImportMessages()

  if (!file) {
    importFile.value = null
    return
  }

  if (!file.name.toLowerCase().endsWith(".zip")) {
    importFile.value = null
    importState.error = "Only .zip archives are supported."
    return
  }

  importFile.value = file
}

async function exportArchive() {
  resetExportMessages()

  if (!canOperate.value) {
    exportState.error = "Operator permission is required."
    return
  }

  exportState.loading = true
  const result = await api.exportCatalogArchive()
  exportState.loading = false

  if (!result.ok || !result.data) {
    exportState.error = result.error || "Unable to export archive."
    return
  }

  const downloadUrl = URL.createObjectURL(result.data.blob)
  const link = document.createElement("a")
  link.href = downloadUrl
  link.download = result.data.fileName || "catalog-archive.zip"
  document.body.appendChild(link)
  link.click()
  link.remove()
  URL.revokeObjectURL(downloadUrl)

  exportState.success = `Archive exported: ${link.download}`
}

async function importArchive() {
  resetImportMessages()

  if (!canOperate.value) {
    importState.error = "Operator permission is required."
    return
  }

  if (!importFile.value) {
    importState.error = "Select a .zip archive file first."
    return
  }

  importState.loading = true
  const result = await api.importCatalogArchive(importFile.value)
  importState.loading = false

  if (!result.ok || !result.data) {
    importState.error = result.error || "Unable to import archive."
    return
  }

  importState.success = formatImportSummary(result.data)
}

function formatImportSummary(summary: ImportCatalogArchiveResponse) {
  return `Imported successfully. Articles: ${summary.articleCount}, Details: ${summary.detailCount}, Discounts: ${summary.discountCount}, Text details: ${summary.articleDetailTextCount}, Numeric details: ${summary.articleDetailNumericCount}, Images: ${summary.articleImageCount}.`
}
</script>

<template>
  <div class="flex h-full min-h-0 flex-col gap-4 overflow-y-auto">
    <div>
      <h2 class="text-foreground text-base font-semibold">Catalog Data</h2>
      <p class="text-muted text-sm">
        Export or import catalog records and article images as a zip archive.
      </p>
    </div>

    <div class="grid gap-4 lg:grid-cols-2 lg:items-stretch">
      <CatalogArchivePanelExportView
        :can-operate="canOperate"
        :loading="exportState.loading"
        :error="exportState.error"
        @download="exportArchive"
      />

      <CatalogArchivePanelImportView
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
