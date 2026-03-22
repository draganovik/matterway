const imageIdPattern = /^[a-f0-9]{32}$/i

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

  return `/api/storefront/cdn/images/${encodeURIComponent(normalizedId)}`
}
