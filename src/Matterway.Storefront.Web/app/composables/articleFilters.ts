export interface DetailFilterDefinition {
  slug: string;
  label: string;
  unit?: string;
}

export interface DetailFilterState {
  slug: string;
  value?: string;
  min?: number;
  max?: number;
}

export interface ArticleFilterState {
  search?: string;
  minPrice?: number;
  maxPrice?: number;
  detailFilters: DetailFilterState[];
}

export const supportedDetailFilters: DetailFilterDefinition[] = [
  { slug: "brand", label: "Brend" },
  { slug: "model", label: "Model" },
  { slug: "color", label: "Boja" },
  { slug: "battery", label: "Baterija" },
  { slug: "connectivity", label: "Povezivanje" },
  { slug: "compatibility", label: "Kompatibilnost" },
  { slug: "display", label: "Ekran" },
  { slug: "material", label: "Materijal" },
  { slug: "color-temperature", label: "Temperatura boje" },
  { slug: "camera", label: "Kamera" },
  { slug: "processor", label: "Procesor" },
  { slug: "operating-system", label: "Operativni sistem" },
  { slug: "resolution", label: "Rezolucija" },
  { slug: "video-quality", label: "Video kvalitet" },
  { slug: "audio-quality", label: "Audio kvalitet" },
  { slug: "audio", label: "Audio" },
  { slug: "ports", label: "Portovi" },
  { slug: "features", label: "Karakteristike" },
  { slug: "battery-size", label: "Kapacitet baterije", unit: "mAh" },
  { slug: "depth", label: "Dubina", unit: "mm" },
  { slug: "height", label: "Visina", unit: "mm" },
  { slug: "power", label: "Snaga", unit: "W" },
  { slug: "ram-size", label: "RAM", unit: "GB" },
  { slug: "refresh-rate", label: "Osvezavanje", unit: "Hz" },
  { slug: "screen-size", label: "Velicina ekrana", unit: "in" },
  { slug: "storage", label: "Memorija", unit: "GB" },
  { slug: "weight", label: "Tezina", unit: "g" },
  { slug: "width", label: "Sirina", unit: "mm" },
];

const detailDefinitionMap = new Map(
  supportedDetailFilters.map((definition) => [definition.slug, definition]),
);

const toNumeric = (value?: number) =>
  typeof value === "number" && Number.isFinite(value) ? value : undefined;

export const resolveDetailDefinition = (slug?: string) =>
  slug ? detailDefinitionMap.get(slug) : undefined;

export const isNumericDetailDefinition = (
  definition?: DetailFilterDefinition,
) => Boolean(definition?.unit);

export const createEmptyDetailFilter = (): DetailFilterState => {
  const fallback = supportedDetailFilters[0];
  return {
    slug: fallback?.slug ?? "",
    value: "",
    min: undefined,
    max: undefined,
  };
};

export const serializeDetailFilters = (
  detailFilters: DetailFilterState[],
): string | undefined => {
  const normalized = detailFilters
    .map((filter) => {
      const definition = resolveDetailDefinition(filter.slug);
      if (!definition) return null;

      if (isNumericDetailDefinition(definition)) {
        const min = toNumeric(filter.min);
        const max = toNumeric(filter.max);
        if (min === undefined && max === undefined) return null;
        return { slug: definition.slug, min, max };
      }

      const value = filter.value?.trim();
      if (!value) return null;
      return { slug: definition.slug, value };
    })
    .filter(Boolean);

  if (!normalized.length) return undefined;

  try {
    return JSON.stringify(normalized);
  } catch {
    return undefined;
  }
};

export const parseDetailFilters = (
  serialized?: string | null,
): DetailFilterState[] => {
  if (!serialized || typeof serialized !== "string") return [];
  try {
    const parsed = JSON.parse(serialized);
    if (!Array.isArray(parsed)) return [];

    return parsed
      .map((item) => {
        if (!item || typeof item.slug !== "string") return null;
        const definition = resolveDetailDefinition(item.slug);
        if (!definition) return null;

        if (isNumericDetailDefinition(definition)) {
          return {
            slug: definition.slug,
            min: toNumeric(item.min),
            max: toNumeric(item.max),
          };
        }

        return {
          slug: definition.slug,
          value: typeof item.value === "string" ? item.value : "",
          min: undefined,
          max: undefined,
        };
      })
      .filter(Boolean) as DetailFilterState[];
  } catch {
    return [];
  }
};

export const buildArticlesRsqlFilter = (
  filters: ArticleFilterState,
): string | undefined => {
  const clauses: string[] = [];

  const search = filters.search?.trim();
  if (search) {
    clauses.push(`title==${search}`);
  }

  const minPrice = toNumeric(filters.minPrice);
  if (minPrice !== undefined) {
    clauses.push(`price=ge=${minPrice}`);
  }

  const maxPrice = toNumeric(filters.maxPrice);
  if (maxPrice !== undefined) {
    clauses.push(`price=le=${maxPrice}`);
  }

  for (const detail of filters.detailFilters ?? []) {
    const definition = resolveDetailDefinition(detail.slug);
    if (!definition) continue;

    if (isNumericDetailDefinition(definition)) {
      const min = toNumeric(detail.min);
      const max = toNumeric(detail.max);

      if (min !== undefined) {
        clauses.push(`${definition.slug}=ge=${min}`);
      }
      if (max !== undefined) {
        clauses.push(`${definition.slug}=le=${max}`);
      }
    } else {
      const value = detail.value?.trim();
      if (!value) continue;
      clauses.push(`${definition.slug}==${value}`);
    }
  }

  return clauses.length ? clauses.join(";") : undefined;
};
