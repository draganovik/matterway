export default class ProductModel {
  id: string;
  productCode: string;
  title: string;
  basePrice: number;
  price: number;
  discount: ProductDiscount | null;
  description: string;
  productDetails: ProductDetail[];
  productSpecifications: ProductSpecification[];
  productImages: ProductImages[];
  thumbnailImage: ProductThumbnail | null;
  createdAt?: string;
  updatedAt?: string;
  isAvailable: boolean;

  constructor(initial?: Partial<ProductModel>) {
    this.id = initial?.id ?? "";
    this.productCode = initial?.productCode ?? initial?.code ?? "";
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
    this.productDetails =
      initial?.productDetails?.map((detail) => ({
        detailSlug: detail.detailSlug ?? detail.typeSlug ?? null,
        title: detail.title,
        value: detail.value,
      })) ?? [];
    this.productSpecifications =
      initial?.productSpecifications?.map((spec) => ({
        specificationSlug: spec.specificationSlug ?? null,
        title: spec.title,
        value: typeof spec.value === "string" ? Number(spec.value) : spec.value,
        unit: spec.unit ?? null,
      })) ?? [];
    this.productImages =
      initial?.productImages?.map((image) => ({ ...image })) ?? [];
    this.thumbnailImage = initial?.thumbnailImage
      ? { ...initial.thumbnailImage }
      : null;
    this.createdAt = initial?.createdAt;
    this.updatedAt = initial?.updatedAt;
    this.isAvailable = initial?.isAvailable ?? false;
  }

  static fromCatalogResponse(response: any): ProductModel {
    return new ProductModel({
      id: response.id,
      productCode: response.productCode ?? response.code,
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
      productDetails: [],
      productSpecifications: [],
      productImages: [],
      createdAt: response.createdAt,
      updatedAt: response.updatedAt,
    });
  }

  static fromDetailResponse(response: any): ProductModel {
    const orderedImages = [
      ...(response.images ?? response.productImages ?? []),
    ].sort(
      (a, b) =>
        (a.orderIndex ?? Number.MAX_SAFE_INTEGER) -
        (b.orderIndex ?? Number.MAX_SAFE_INTEGER),
    );
    const primaryImage = orderedImages[0];
    return new ProductModel({
      id: response.id,
      productCode: response.productCode ?? response.code,
      title: response.title,
      basePrice: response.basePrice ?? response.price,
      price: response.price ?? response.basePrice,
      discount: response.discount ?? null,
      description: response.description,
      productDetails: (response.details ?? response.productDetails ?? []).map(
        (detail: any) => ({
          detailSlug:
            detail.detailSlug ?? detail.typeSlug ?? detail.slug ?? null,
          title: detail.title,
          value: detail.value,
        }),
      ),
      productSpecifications: (response.specifications ?? []).map(
        (spec: any) => ({
          specificationSlug:
            spec.specificationSlug ?? spec.slug ?? spec.typeSlug ?? null,
          title: spec.title,
          value:
            typeof spec.value === "string" ? Number(spec.value) : spec.value,
          unit: spec.unit ?? null,
        }),
      ),
      productImages: orderedImages,
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

export class ProductDetail {
  detailSlug?: string | null;
  title?: string;
  value?: string;
}

export class ProductSpecification {
  specificationSlug?: string | null;
  title?: string;
  value?: number | null;
  unit?: string | null;
}

export class ProductImages {
  id?: string;
  orderIndex?: number;
  imageUrl?: string;
  imageAlt?: string;
}

export class ProductThumbnail {
  imageUrl?: string;
  imageAlt?: string;
}

export class ProductDiscount {
  percentage?: number;
  validFrom?: string | null;
  validTo?: string | null;
}
