import type { PaginationResponse } from "../common/pagination"

export type QueryCustomersParams = {
  page: number
  pageSize: number
}

export type CustomerRequest = {
  systemUserId: string
  firstName: string
  lastName: string
  birthDate: string
  defaultAddressId?: string | null
}

export type CustomerResponse = {
  systemUserId: string
  firstName?: string | null
  lastName?: string | null
  birthDate: string
  defaultAddressId?: string | null
}

export type CustomerAddressResponse = {
  id: string
  customerId: string
  country?: string | null
  city?: string | null
  zipCode?: string | null
  addressLine1?: string | null
  addressLine2?: string | null
  contactPhone?: string | null
}

export type PutCustomerAddressRequest = {
  country: string
  city: string
  zipCode: string
  addressLine1: string
  addressLine2: string
  contactPhone: string
}

export type QueryCustomersResponse = PaginationResponse<CustomerResponse>

export type DeleteCustomerResponse = {
  id: string
  message?: string | null
}
