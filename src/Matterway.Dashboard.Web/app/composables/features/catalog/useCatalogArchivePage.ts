import { useAuthSessionStore } from "~/composables/stores/useAuthSessionStore"
import { useCatalogClient } from "~/composables/api/useCatalogClient"
import { useRequestState } from "~/composables/workflows/state/useRequestState"
import type { ImportCatalogArchiveResponse } from "~/types/catalog"

export function useCatalogArchivePage() {
  const auth = useAuthSessionStore()
  const api = useCatalogClient()

  const canOperate = computed(() => auth.hasPermission("catalog", ["manager"]))

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
      importState.error = "Podržane su samo .zip arhive."
      return
    }

    importFile.value = file
  }

  async function exportArchive() {
    resetExportMessages()

    if (!canOperate.value) {
      exportState.error = "Potrebna je dozvola menadžera."
      return
    }

    exportState.loading = true
    const result = await api.exportCatalogArchive()
    exportState.loading = false

    if (!result.ok || !result.data) {
      exportState.error = result.error || "Izvoz arhive nije uspeo."
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

    exportState.success = `Arhiva je uspešno izvezena: ${link.download}`
  }

  async function importArchive() {
    resetImportMessages()

    if (!canOperate.value) {
      importState.error = "Potrebna je dozvola menadžera."
      return
    }

    if (!importFile.value) {
      importState.error = "Najpre izaberite .zip arhivu."
      return
    }

    importState.loading = true
    const result = await api.importCatalogArchive(importFile.value)
    importState.loading = false

    if (!result.ok || !result.data) {
      importState.error = result.error || "Uvoz arhive nije uspeo."
      return
    }

    importState.success = formatImportSummary(result.data)
  }

  function formatImportSummary(summary: ImportCatalogArchiveResponse) {
    return `Uvoz je uspešno završen. Artikli: ${summary.articleCount}, detalji: ${summary.detailCount}, popusti: ${summary.discountCount}, tekstualni detalji: ${summary.articleDetailTextCount}, numerički detalji: ${summary.articleDetailNumericCount}, slike: ${summary.articleImageCount}.`
  }

  return {
    canOperate,
    exportState,
    importState,
    hasSelectedImportFile,
    selectedImportFileName,
    setImportFile,
    exportArchive,
    importArchive,
    clearImportFileSelection,
  }
}
