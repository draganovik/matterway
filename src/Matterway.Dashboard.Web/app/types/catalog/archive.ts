export type ImportCatalogArchiveResponse = {
  detailCount: number
  articleCount: number
  discountCount: number
  articleDetailTextCount: number
  articleDetailNumericCount: number
  articleImageCount: number
}

export type ExportCatalogArchiveResponse = {
  fileName: string
  blob: Blob
}
