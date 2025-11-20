export type ProductDetailFilterType = "text" | "number";

export interface ProductDetailFilterDefinition {
  slug: string;
  label: string;
  type: ProductDetailFilterType;
  unit?: string;
}

export interface ProductDetailFilterState {
  slug: string;
  type: ProductDetailFilterType;
  value?: string;
  min?: number;
  max?: number;
}

export interface ProductFilterState {
  search?: string;
  minPrice?: number;
  maxPrice?: number;
  detailFilters: ProductDetailFilterState[];
}

export const supportedProductDetailFilters: ProductDetailFilterDefinition[] = [
  { slug: "brand", label: "Brend", type: "text" },
  { slug: "model", label: "Model", type: "text" },
  { slug: "color", label: "Boja", type: "text" },
  { slug: "weight", label: "Tezina (g)", type: "number", unit: "g" },
  { slug: "width", label: "Sirina (cm)", type: "number", unit: "cm" },
  { slug: "height", label: "Visina (cm)", type: "number", unit: "cm" },
  { slug: "depth", label: "Dubina (cm)", type: "number", unit: "cm" },
  { slug: "power", label: "Snaga (W)", type: "number", unit: "W" },
  { slug: "battery", label: "Baterija", type: "text" },
  { slug: "connectivity", label: "Povezivanje", type: "text" },
  { slug: "compatibility", label: "Kompatibilnost", type: "text" },
  { slug: "display", label: "Ekran", type: "text" },
  { slug: "material", label: "Materijal", type: "text" },
  { slug: "color-temperature", label: "Temperatura boje", type: "text" },
  { slug: "camera", label: "Kamera", type: "text" },
  {
    slug: "battery-size",
    label: "Kapacitet baterije (mAh)",
    type: "number",
    unit: "mAh",
  },
  { slug: "storage", label: "Memorija (GB)", type: "number", unit: "GB" },
  { slug: "ram-size", label: "RAM (GB)", type: "number", unit: "GB" },
  { slug: "processor", label: "Procesor", type: "text" },
  { slug: "operating-system", label: "Operativni sistem", type: "text" },
  {
    slug: "screen-size",
    label: "Velicina ekrana (in)",
    type: "number",
    unit: "in",
  },
  { slug: "resolution", label: "Rezolucija", type: "text" },
  { slug: "video-quality", label: "Video kvalitet", type: "text" },
  { slug: "audio-quality", label: "Audio kvalitet", type: "text" },
  {
    slug: "refresh-rate",
    label: "Osvezavanje (Hz)",
    type: "number",
    unit: "Hz",
  },
  { slug: "audio", label: "Audio", type: "text" },
  { slug: "ports", label: "Portovi", type: "text" },
  { slug: "features", label: "Karakteristike", type: "text" },
];

const detailDefinitionMap = new Map(
  supportedProductDetailFilters.map((definition) => [
    definition.slug,
    definition,
  ]),
);

const toNumeric = (value?: number) =>
  typeof value === "number" && Number.isFinite(value) ? value : undefined;

export const resolveDetailDefinition = (slug?: string) =>
  slug ? detailDefinitionMap.get(slug) : undefined;

export const createEmptyDetailFilter = (): ProductDetailFilterState => {
  const fallback = supportedProductDetailFilters[0];
  return {
    slug: fallback?.slug ?? "",
    type: fallback?.type ?? "text",
    value: "",
    min: undefined,
    max: undefined,
  };
};

export const serializeDetailFilters = (
  detailFilters: ProductDetailFilterState[],
): string | undefined => {
  if (!detailFilters.length) {
    return undefined;
  }

  const normalized = detailFilters
    .map((filter) => {
      const definition = resolveDetailDefinition(filter.slug);
      if (!definition) {
        return null;
      }

      if (definition.type === "text") {
        return {
          slug: definition.slug,
          type: definition.type,
          value: filter.value ?? "",
        };
      }

      return {
        slug: definition.slug,
        type: definition.type,
        min: toNumeric(filter.min),
        max: toNumeric(filter.max),
      };
    })
    .filter(Boolean);

  if (normalized.length === 0) {
    return undefined;
  }

  try {
    return JSON.stringify(normalized);
  } catch {
    return undefined;
  }
};

export const parseDetailFilters = (
  serialized?: string | null,
): ProductDetailFilterState[] => {
  if (!serialized || typeof serialized !== "string") {
    return [];
  }

  try {
    const parsed = JSON.parse(serialized);
    if (!Array.isArray(parsed)) {
      return [];
    }

    return parsed
      .map((item) => {
        if (!item || typeof item.slug !== "string") {
          return null;
        }
        const definition = resolveDetailDefinition(item.slug);
        if (!definition) {
          return null;
        }

        if (definition.type === "text") {
          return {
            slug: definition.slug,
            type: definition.type,
            value: typeof item.value === "string" ? item.value : "",
          };
        }

        return {
          slug: definition.slug,
          type: definition.type,
          min: toNumeric(item.min),
          max: toNumeric(item.max),
        };
      })
      .filter(Boolean) as ProductDetailFilterState[];
  } catch {
    return [];
  }
};

export const buildProductsRsqlFilter = (
  filters: ProductFilterState,
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
    if (!definition) {
      continue;
    }

    if (definition.type === "text") {
      const value = detail.value?.trim();
      if (value) {
        clauses.push(`${definition.slug}==${value}`);
      }
      continue;
    }

    const min = toNumeric(detail.min);
    const max = toNumeric(detail.max);
    if (min !== undefined) {
      clauses.push(`${definition.slug}=ge=${min}`);
    }
    if (max !== undefined) {
      clauses.push(`${definition.slug}=le=${max}`);
    }
  }

  return clauses.length ? clauses.join(";") : undefined;
};
