export type CatalogArticleDiscount = {
  percentage?: number | null;
  validFrom?: string | null;
  validTo?: string | null;
};

export type CatalogArticleImage = {
  id?: string;
  orderIndex?: number;
  imageUrl?: string;
  imageAlt?: string;
};

export type CatalogArticleDetail = {
  detailSlug?: string | null;
  title?: string;
  unit?: string | null;
  textValue?: string | null;
  numericValue?: number | null;
};

export type CatalogArticle = {
  id: string;
  articleCode: string;
  title: string;
  basePrice: number;
  price: number;
  discount: CatalogArticleDiscount | null;
  description: string;
  isAvailable: boolean;
  thumbnailImage: CatalogArticleImage | null;
  articleImages: CatalogArticleImage[];
  articleDetails: CatalogArticleDetail[];
  createdAt?: string;
  updatedAt?: string;
};

type AnyRecord = Record<string, unknown>;

function asNumber(value: unknown, fallback = 0) {
  const parsed = Number(value);
  return Number.isFinite(parsed) ? parsed : fallback;
}

function asRecord(value: unknown): AnyRecord {
  return value && typeof value === "object" ? (value as AnyRecord) : {};
}

function asArray(value: unknown): unknown[] {
  return Array.isArray(value) ? value : [];
}

function readDiscount(value: unknown): CatalogArticleDiscount | null {
  const source = asRecord(value);
  if (!Object.keys(source).length) return null;
  return {
    percentage: asNumber(source.percentage, 0),
    validFrom: typeof source.validFrom === "string" ? source.validFrom : null,
    validTo: typeof source.validTo === "string" ? source.validTo : null,
  };
}

function readImage(value: unknown): CatalogArticleImage | null {
  const source = asRecord(value);
  if (!source.imageUrl) return null;
  return {
    id: typeof source.id === "string" ? source.id : undefined,
    orderIndex: asNumber(source.orderIndex, 0),
    imageUrl: String(source.imageUrl),
    imageAlt: source.imageAlt ? String(source.imageAlt) : undefined,
  };
}

export function mapCatalogArticleListItem(payload: unknown): CatalogArticle {
  const source = asRecord(payload);
  const thumbnail =
    readImage(source.thumbnailImage) ||
    (source.thumbnailUrl
      ? {
          imageUrl: String(source.thumbnailUrl),
          imageAlt: source.thumbnailAlt
            ? String(source.thumbnailAlt)
            : undefined,
        }
      : null);

  return {
    id: String(source.id ?? ""),
    articleCode: String(source.articleCode ?? source.code ?? ""),
    title: String(source.title ?? ""),
    basePrice: asNumber(source.basePrice ?? source.price),
    price: asNumber(source.price ?? source.basePrice),
    discount: readDiscount(source.discount),
    description: String(source.description ?? ""),
    isAvailable: Boolean(source.isAvailable),
    thumbnailImage: thumbnail,
    articleImages: [],
    articleDetails: [],
    createdAt:
      typeof source.createdAt === "string" ? source.createdAt : undefined,
    updatedAt:
      typeof source.updatedAt === "string" ? source.updatedAt : undefined,
  };
}

export function mapCatalogArticleDetail(payload: unknown): CatalogArticle {
  const source = asRecord(payload);

  const images = [...asArray(source.images), ...asArray(source.articleImages)]
    .map(readImage)
    .filter((image): image is CatalogArticleImage => Boolean(image?.imageUrl))
    .sort(
      (a, b) =>
        Number(a.orderIndex ?? Number.MAX_SAFE_INTEGER) -
        Number(b.orderIndex ?? Number.MAX_SAFE_INTEGER),
    );

  const thumbnail = readImage(source.thumbnailImage) || images[0] || null;

  const details = asArray(source.details).map((detailValue) => {
    const detail = asRecord(detailValue);
    const numericValueRaw = detail.numericValue;
    const numericValue =
      typeof numericValueRaw === "number"
        ? numericValueRaw
        : numericValueRaw !== undefined && numericValueRaw !== null
          ? Number(numericValueRaw)
          : null;

    return {
      detailSlug: detail.detailSlug ? String(detail.detailSlug) : null,
      title: detail.title ? String(detail.title) : undefined,
      unit: detail.unit ? String(detail.unit) : null,
      textValue: detail.textValue ? String(detail.textValue) : null,
      numericValue: Number.isFinite(numericValue ?? NaN) ? numericValue : null,
    };
  });

  return {
    id: String(source.id ?? ""),
    articleCode: String(source.articleCode ?? source.code ?? ""),
    title: String(source.title ?? ""),
    basePrice: asNumber(source.basePrice ?? source.price),
    price: asNumber(source.price ?? source.basePrice),
    discount: readDiscount(source.discount),
    description: String(source.description ?? ""),
    isAvailable: Boolean(source.isAvailable),
    thumbnailImage: thumbnail,
    articleImages: images,
    articleDetails: details,
    createdAt:
      typeof source.createdAt === "string" ? source.createdAt : undefined,
    updatedAt:
      typeof source.updatedAt === "string" ? source.updatedAt : undefined,
  };
}
