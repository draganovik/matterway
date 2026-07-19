import { buildCatalogImageUrl } from "~/utils/catalogImages"

export type CatalogArticleDiscount = {
  percentage: number
  validFrom: string
  validTo?: string | null
}

export type CatalogArticleImage = {
  id?: string
  orderIndex?: number
  imageUrl?: string | null
  imageAlt?: string | null
}

export type CatalogArticleDetail = {
  detailSlug?: string | null
  title?: string | null
  unit?: string | null
  textValue?: string | null
  numericValue?: number | null
}

export type CatalogArticle = {
  code: string
  title: string
  basePrice: number
  price: number
  discount: CatalogArticleDiscount | null
  description: string
  isAvailable: boolean
  thumbnailImage: CatalogArticleImage | null
  articleImages: CatalogArticleImage[]
  articleDetails: CatalogArticleDetail[]
  createdAt?: string | null
  updatedAt?: string | null
}

export type CatalogArticleListResponse = {
  code: string
  title?: string | null
  basePrice?: number | null
  price?: number | null
  discount?: CatalogArticleDiscount | null
  description?: string | null
  thumbnailUrl?: string | null
  thumbnailAlt?: string | null
  isAvailable: boolean
}

export type CatalogArticleDetailResponse = {
  code: string
  title?: string | null
  basePrice?: number | null
  price?: number | null
  discount?: CatalogArticleDiscount | null
  description?: string | null
  details?: CatalogArticleDetail[] | null
  images?: CatalogArticleImage[] | null
  createdAt?: string | null
  updatedAt?: string | null
  isAvailable: boolean
}

function mapImage(image: CatalogArticleImage): CatalogArticleImage {
  return {
    ...image,
    imageUrl: buildCatalogImageUrl(image.imageUrl, image.id),
  }
}

export function mapCatalogArticleListItem(
  article: CatalogArticleListResponse,
): CatalogArticle {
  const thumbnailUrl = buildCatalogImageUrl(article.thumbnailUrl)

  return {
    code: article.code,
    title: article.title ?? "",
    basePrice: article.basePrice ?? 0,
    price: article.price ?? article.basePrice ?? 0,
    discount: article.discount ?? null,
    description: article.description ?? "",
    isAvailable: article.isAvailable,
    thumbnailImage: thumbnailUrl
      ? { imageUrl: thumbnailUrl, imageAlt: article.thumbnailAlt }
      : null,
    articleImages: [],
    articleDetails: [],
  }
}

export function mapCatalogArticleDetail(
  article: CatalogArticleDetailResponse,
): CatalogArticle {
  const images = (article.images ?? [])
    .map(mapImage)
    .sort(
      (left, right) =>
        (left.orderIndex ?? Number.MAX_SAFE_INTEGER) -
        (right.orderIndex ?? Number.MAX_SAFE_INTEGER),
    )

  return {
    code: article.code,
    title: article.title ?? "",
    basePrice: article.basePrice ?? 0,
    price: article.price ?? article.basePrice ?? 0,
    discount: article.discount ?? null,
    description: article.description ?? "",
    isAvailable: article.isAvailable,
    thumbnailImage: images[0] ?? null,
    articleImages: images,
    articleDetails: article.details ?? [],
    createdAt: article.createdAt,
    updatedAt: article.updatedAt,
  }
}
