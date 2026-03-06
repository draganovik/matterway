export interface DetailFilterDefinition {
  slug: string
  label: string
  unit?: string
}

export interface DetailFilterState {
  slug: string
  value?: string
  min?: number
  max?: number
}

export interface ArticleFilterState {
  search?: string
  minPrice?: number
  maxPrice?: number
  detailFilters: DetailFilterState[]
}

const toNumeric = (value?: number) =>
  typeof value === "number" && Number.isFinite(value) ? value : undefined

function bySlug(definitions: DetailFilterDefinition[]) {
  return new Map(definitions.map((definition) => [definition.slug, definition]))
}

export const normalizeDetailDefinitions = (
  definitions: Array<{
    slug?: string | null
    title?: string | null
    unit?: string | null
  }>,
): DetailFilterDefinition[] => {
  const map = new Map<string, DetailFilterDefinition>()
  for (const definition of definitions) {
    const slug = definition.slug?.trim()
    if (!slug) continue
    map.set(slug, {
      slug,
      label: definition.title?.trim() || slug,
      unit: definition.unit?.trim() || undefined,
    })
  }
  return Array.from(map.values()).sort((a, b) => a.label.localeCompare(b.label))
}

export const resolveDetailDefinition = (
  slug: string | undefined,
  definitions: DetailFilterDefinition[],
) => (slug ? bySlug(definitions).get(slug) : undefined)

export const isNumericDetailDefinition = (
  definition?: DetailFilterDefinition,
) => Boolean(definition?.unit)

export const createEmptyDetailFilter = (
  definitions: DetailFilterDefinition[],
): DetailFilterState => {
  const fallback = definitions[0]
  return {
    slug: fallback?.slug ?? "",
    value: "",
    min: undefined,
    max: undefined,
  }
}

export const buildArticlesRsqlFilter = (
  filters: ArticleFilterState,
  definitions: DetailFilterDefinition[],
): string | undefined => {
  const clauses: string[] = []

  const search = filters.search?.trim()
  if (search) {
    clauses.push(`title==${search}`)
  }

  const minPrice = toNumeric(filters.minPrice)
  if (minPrice !== undefined) {
    clauses.push(`price=ge=${minPrice}`)
  }

  const maxPrice = toNumeric(filters.maxPrice)
  if (maxPrice !== undefined) {
    clauses.push(`price=le=${maxPrice}`)
  }

  for (const detail of filters.detailFilters ?? []) {
    const slug = detail.slug?.trim()
    if (!slug) continue
    const definition = resolveDetailDefinition(slug, definitions)
    const hasNumericBounds =
      detail.min !== undefined || detail.max !== undefined

    if (isNumericDetailDefinition(definition) || hasNumericBounds) {
      const min = toNumeric(detail.min)
      const max = toNumeric(detail.max)

      if (min !== undefined) {
        clauses.push(`${slug}=ge=${min}`)
      }
      if (max !== undefined) {
        clauses.push(`${slug}=le=${max}`)
      }
    } else {
      const value = detail.value?.trim()
      if (!value) continue
      clauses.push(`${slug}==${value}`)
    }
  }

  return clauses.length ? clauses.join(";") : undefined
}
