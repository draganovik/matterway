import type { NumberInput } from "./shared"

export type CreateDiscountRequest = {
  code: string
  percentage: NumberInput
  validFrom: string
  validTo?: string | null
  articleIds: string[]
}

export type CreatedDiscountResponse = {
  code: string
  percentage: NumberInput
  validFrom: string
  validTo?: string | null
  articleId: string
}

export type UpdateDiscountRequest = {
  percentage: NumberInput
  validFrom: string
  validTo?: string | null
  articleIds: string[]
}

export type UpdatedDiscountResponse = {
  code: string
  percentage: NumberInput
  validFrom: string
  validTo?: string | null
  articleId: string
}

export type QueryDiscountResponse = {
  code?: string | null
  percentage?: NumberInput | null
  validFrom?: string | null
  validTo?: string | null
  articleId?: string | null
}

export type DeleteDiscountResponse = {
  code: string
  removedCount?: NumberInput
  message?: string | null
}
