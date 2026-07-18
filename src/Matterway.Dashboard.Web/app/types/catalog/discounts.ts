import type { NumberInput } from "./shared"

export type CreateDiscountRequest = {
  code: string
  percentage: NumberInput
  validFrom: string
  validTo?: string | null
  articleCodes: string[]
}

export type UpdateDiscountRequest = {
  percentage: NumberInput
  validFrom: string
  validTo?: string | null
  articleCodes: string[]
}

export type UpdatedDiscountResponse = {
  code: string
  percentage: NumberInput
  validFrom: string
  validTo?: string | null
  articleCodes: string[]
}

export type QueryDiscountResponse = {
  code?: string | null
  percentage?: NumberInput | null
  validFrom?: string | null
  validTo?: string | null
  articleCode?: string | null
}

export type DeleteDiscountResponse = {
  code: string
  removedCount?: NumberInput
  message?: string | null
}
