export interface DetailFilterDefinition {
  slug: string;
  label: string;
}

export interface SpecificationFilterDefinition {
  slug: string;
  label: string;
  unit?: string;
}

export interface DetailFilterState {
  slug: string;
  value?: string;
}

export interface SpecificationFilterState {
  slug: string;
  min?: number;
  max?: number;
}

export interface ArticleFilterState {
  search?: string;
  minPrice?: number;
  maxPrice?: number;
  detailFilters: DetailFilterState[];
  specificationFilters: SpecificationFilterState[];
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
];

export const supportedSpecificationFilters: SpecificationFilterDefinition[] = [
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
const specificationDefinitionMap = new Map(
  supportedSpecificationFilters.map((definition) => [
    definition.slug,
    definition,
  ]),
);

const toNumeric = (value?: number) =>
  typeof value === "number" && Number.isFinite(value) ? value : undefined;

export const resolveDetailDefinition = (slug?: string) =>
  slug ? detailDefinitionMap.get(slug) : undefined;

export const resolveSpecificationDefinition = (slug?: string) =>
  slug ? specificationDefinitionMap.get(slug) : undefined;

export const createEmptyDetailFilter = (): DetailFilterState => {
  const fallback = supportedDetailFilters[0];
  return {
    slug: fallback?.slug ?? "",
    value: "",
  };
};

export const createEmptySpecificationFilter = (): SpecificationFilterState => {
  const fallback = supportedSpecificationFilters[0];
  return {
    slug: fallback?.slug ?? "",
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
      const value = filter.value?.trim();
      if (!definition || !value) return null;
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

        return {
          slug: definition.slug,
          value: typeof item.value === "string" ? item.value : "",
        };
      })
      .filter(Boolean) as DetailFilterState[];
  } catch {
    return [];
  }
};

export const serializeSpecificationFilters = (
  specificationFilters: SpecificationFilterState[],
): string | undefined => {
  const normalized = specificationFilters
    .map((filter) => {
      const definition = resolveSpecificationDefinition(filter.slug);
      const min = toNumeric(filter.min);
      const max = toNumeric(filter.max);
      if (!definition || (min === undefined && max === undefined)) {
        return null;
      }
      return { slug: definition.slug, min, max };
    })
    .filter(Boolean);

  if (!normalized.length) return undefined;

  try {
    return JSON.stringify(normalized);
  } catch {
    return undefined;
  }
};

export const parseSpecificationFilters = (
  serialized?: string | null,
): SpecificationFilterState[] => {
  if (!serialized || typeof serialized !== "string") return [];
  try {
    const parsed = JSON.parse(serialized);
    if (!Array.isArray(parsed)) return [];

    return parsed
      .map((item) => {
        if (!item || typeof item.slug !== "string") return null;
        const definition = resolveSpecificationDefinition(item.slug);
        if (!definition) return null;
        return {
          slug: definition.slug,
          min: toNumeric(item.min),
          max: toNumeric(item.max),
        };
      })
      .filter(Boolean) as SpecificationFilterState[];
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
    const value = detail.value?.trim();
    if (!definition || !value) continue;
    clauses.push(`${definition.slug}==${value}`);
  }

  for (const spec of filters.specificationFilters ?? []) {
    const definition = resolveSpecificationDefinition(spec.slug);
    if (!definition) continue;
    const min = toNumeric(spec.min);
    const max = toNumeric(spec.max);

    if (min !== undefined) {
      clauses.push(`${definition.slug}=ge=${min}`);
    }
    if (max !== undefined) {
      clauses.push(`${definition.slug}=le=${max}`);
    }
  }

  return clauses.length ? clauses.join(";") : undefined;
};
