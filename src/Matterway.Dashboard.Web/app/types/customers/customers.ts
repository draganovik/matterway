import type { PaginationResponse } from '../common/pagination'

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

export type QueryCustomersResponse = PaginationResponse<CustomerResponse>

export type DeleteCustomerResponse = {
  id: string
  message?: string | null
}
