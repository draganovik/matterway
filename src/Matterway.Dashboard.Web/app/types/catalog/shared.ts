export type NumberInput = number | string

export type ArticleDiscountProperty = {
  code?: string | null
  percentage: NumberInput
  validFrom: string
  validTo?: string | null
}

export type ArticleImageProperty = {
  id: string
  orderIndex: NumberInput
  imageUrl?: string | null
  imageAlt?: string | null
}

export type ArticleDetailProperty = {
  detailSlug?: string | null
  title?: string | null
  unit?: string | null
  textValue?: string | null
  numericValue?: NumberInput | null
}
