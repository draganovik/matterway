export default class ProductModel {
  id: string;
  productCode: string;
  title: string;
  price: number;
  description: string;
  productDetails: ProductDetails[];
  productImages: ProductImages[];
  thumbnailImage: ProductThumbnail | null;
  createdAt?: string;
  updatedAt?: string;
  isAvailable: boolean;

  constructor(initial?: Partial<ProductModel>) {
    this.id = initial?.id ?? "";
    this.productCode = initial?.productCode ?? "";
    this.title = initial?.title ?? "";
    this.price = initial?.price ?? 0;
    this.description = initial?.description ?? "";
    this.productDetails =
      initial?.productDetails?.map((detail) => ({
        typeId: detail.typeId ?? null,
        title: detail.title,
        value: detail.value,
        unit: detail.unit ?? null,
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
      productCode: response.productCode,
      title: response.title,
      price: response.price,
      description: response.description,
      thumbnailImage: response.thumbnailImage
        ? { ...response.thumbnailImage }
        : null,
      isAvailable: response.isAvailable ?? false,
      productDetails: [],
      productImages: [],
      createdAt: response.createdAt,
      updatedAt: response.updatedAt,
    });
  }

  static fromDetailResponse(response: any): ProductModel {
    const orderedImages = [...(response.productImages ?? [])].sort(
      (a, b) =>
        (a.orderIndex ?? Number.MAX_SAFE_INTEGER) -
        (b.orderIndex ?? Number.MAX_SAFE_INTEGER),
    );
    const primaryImage = orderedImages[0];
    return new ProductModel({
      id: response.id,
      productCode: response.productCode,
      title: response.title,
      price: response.price,
      description: response.description,
      productDetails: (response.productDetails ?? []).map(
        (detail: any) => ({
          typeId: detail.typeId ?? null,
          title: detail.title,
          value: detail.value,
          unit: detail.unit ?? null,
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

export class ProductDetails {
  typeId?: number | null;
  title?: string;
  value?: string;
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
