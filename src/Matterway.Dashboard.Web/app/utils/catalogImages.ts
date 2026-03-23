const imageIdPattern = /^[a-f0-9]{32}$/i
const supportedCatalogImageMimeTypes = [
  "image/jpeg",
  "image/png",
  "image/webp",
  "image/svg+xml",
] as const

const supportedCatalogImageExtensions = [
  ".jpg",
  ".jpeg",
  ".png",
  ".webp",
  ".svg",
] as const

const supportedCatalogImageFormatsLabel = "JPG, PNG, WEBP i SVG"

function normalizeImageId(imageId: string | null | undefined) {
  const normalizedId =
    typeof imageId === "string" ? imageId.trim().replace(/-/g, "") : ""

  return imageIdPattern.test(normalizedId) ? normalizedId.toLowerCase() : null
}

function extractImageId(sourceUrl: string) {
  try {
    const url = new URL(sourceUrl, "http://catalog-image.local")
    const candidate = url.pathname.split("/").pop()
    return normalizeImageId(candidate)
  } catch {
    return null
  }
}

export function buildCatalogImageUrl(
  sourceUrl: string | null | undefined,
  imageId?: string | null,
) {
  const normalizedId =
    normalizeImageId(imageId) || extractImageId(sourceUrl || "")
  if (!normalizedId) return null

  return `/api/dashboard/cdn/images/${encodeURIComponent(normalizedId)}`
}

export const catalogImageUploadAccept = [
  ...supportedCatalogImageMimeTypes,
  ...supportedCatalogImageExtensions,
].join(",")

export function getSupportedCatalogImageFormatsLabel() {
  return supportedCatalogImageFormatsLabel
}

export function isSupportedCatalogImageFile(file: File) {
  const normalizedType = file.type.trim().toLowerCase()
  if (
    normalizedType &&
    supportedCatalogImageMimeTypes.includes(
      normalizedType as (typeof supportedCatalogImageMimeTypes)[number],
    )
  ) {
    return true
  }

  const normalizedName = file.name.trim().toLowerCase()
  return supportedCatalogImageExtensions.some((extension) =>
    normalizedName.endsWith(extension),
  )
}
