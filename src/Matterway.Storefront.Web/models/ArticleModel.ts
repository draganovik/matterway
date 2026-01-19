export default class ArticleModel {
  id: string;
  articleCode: string;
  title: string;
  basePrice: number;
  price: number;
  discount: ArticleDiscount | null;
  description: string;
  articleDetails: ArticleDetail[];
  articleSpecifications: ArticleSpecification[];
  articleImages: ArticleImages[];
  thumbnailImage: ArticleThumbnail | null;
  createdAt?: string;
  updatedAt?: string;
  isAvailable: boolean;

  constructor(initial?: Partial<ArticleModel>) {
    this.id = initial?.id ?? "";
    this.articleCode = initial?.articleCode ?? initial?.code ?? "";
    this.title = initial?.title ?? "";
    this.basePrice = initial?.basePrice ?? initial?.price ?? 0;
    this.price = initial?.price ?? initial?.basePrice ?? 0;
    this.discount = initial?.discount
      ? {
          percentage:
            Number(
              initial.discount.percentage ??
                (initial.discount as any).Percentage ??
                0,
            ) ?? 0,
          validFrom:
            initial.discount.validFrom ??
            (initial.discount as any).ValidFrom ??
            null,
          validTo:
            initial.discount.validTo ??
            (initial.discount as any).ValidTo ??
            null,
        }
      : null;
    this.description = initial?.description ?? "";
    this.articleDetails =
      initial?.articleDetails?.map((detail) => ({
        detailSlug: detail.detailSlug ?? detail.typeSlug ?? null,
        title: detail.title,
        value: detail.value,
      })) ?? [];
    this.articleSpecifications =
      initial?.articleSpecifications?.map((spec) => ({
        specificationSlug: spec.specificationSlug ?? null,
        title: spec.title,
        value: typeof spec.value === "string" ? Number(spec.value) : spec.value,
        unit: spec.unit ?? null,
      })) ?? [];
    this.articleImages =
      initial?.articleImages?.map((image) => ({ ...image })) ?? [];
    this.thumbnailImage = initial?.thumbnailImage
      ? { ...initial.thumbnailImage }
      : null;
    this.createdAt = initial?.createdAt;
    this.updatedAt = initial?.updatedAt;
    this.isAvailable = initial?.isAvailable ?? false;
  }

  static fromCatalogResponse(response: any): ArticleModel {
    return new ArticleModel({
      id: response.id,
      articleCode: response.articleCode ?? response.code,
      title: response.title,
      basePrice: response.basePrice ?? response.price,
      price: response.price ?? response.basePrice,
      discount: response.discount ?? null,
      description: response.description,
      thumbnailImage: response.thumbnailImage
        ? { ...response.thumbnailImage }
        : response.thumbnailUrl
          ? {
              imageUrl: response.thumbnailUrl,
              imageAlt: response.thumbnailAlt,
            }
          : null,
      isAvailable: response.isAvailable ?? false,
      articleDetails: [],
      articleSpecifications: [],
      articleImages: [],
      createdAt: response.createdAt,
      updatedAt: response.updatedAt,
    });
  }

  static fromDetailResponse(response: any): ArticleModel {
    const orderedImages = [
      ...(response.images ?? response.articleImages ?? []),
    ].sort(
      (a, b) =>
        (a.orderIndex ?? Number.MAX_SAFE_INTEGER) -
        (b.orderIndex ?? Number.MAX_SAFE_INTEGER),
    );
    const primaryImage = orderedImages[0];
    return new ArticleModel({
      id: response.id,
      articleCode: response.articleCode ?? response.code,
      title: response.title,
      basePrice: response.basePrice ?? response.price,
      price: response.price ?? response.basePrice,
      discount: response.discount ?? null,
      description: response.description,
      articleDetails: (response.details ?? response.articleDetails ?? []).map(
        (detail: any) => ({
          detailSlug:
            detail.detailSlug ?? detail.typeSlug ?? detail.slug ?? null,
          title: detail.title,
          value: detail.value,
        }),
      ),
      articleSpecifications: (response.specifications ?? []).map(
        (spec: any) => ({
          specificationSlug:
            spec.specificationSlug ?? spec.slug ?? spec.typeSlug ?? null,
          title: spec.title,
          value:
            typeof spec.value === "string" ? Number(spec.value) : spec.value,
          unit: spec.unit ?? null,
        }),
      ),
      articleImages: orderedImages,
      thumbnailImage: response.thumbnailImage
        ? { ...response.thumbnailImage }
        : primaryImage
          ? {
              imageUrl: primaryImage.imageUrl,
              imageAlt: primaryImage.imageAlt,
            }
          : null,
      createdAt: response.createdAt,
      updatedAt: response.updatedAt,
      isAvailable: response.isAvailable ?? false,
    });
  }
}

export class ArticleDetail {
  detailSlug?: string | null;
  title?: string;
  value?: string;
}

export class ArticleSpecification {
  specificationSlug?: string | null;
  title?: string;
  value?: number | null;
  unit?: string | null;
}

export class ArticleImages {
  id?: string;
  orderIndex?: number;
  imageUrl?: string;
  imageAlt?: string;
}

export class ArticleThumbnail {
  imageUrl?: string;
  imageAlt?: string;
}

export class ArticleDiscount {
  percentage?: number;
  validFrom?: string | null;
  validTo?: string | null;
}
