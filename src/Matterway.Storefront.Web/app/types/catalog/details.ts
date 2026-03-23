export type CatalogDetailDefinition = {
  slug?: string | null
  title?: string | null
  unit?: string | null
}

type AnyRecord = Record<string, unknown>

function asRecord(value: unknown): AnyRecord {
  return value && typeof value === "object" ? (value as AnyRecord) : {}
}

export function mapCatalogDetailDefinition(
  payload: unknown,
): CatalogDetailDefinition {
  const source = asRecord(payload)

  return {
    slug: typeof source.slug === "string" ? source.slug : null,
    title: typeof source.title === "string" ? source.title : null,
    unit: typeof source.unit === "string" ? source.unit : null,
  }
}
