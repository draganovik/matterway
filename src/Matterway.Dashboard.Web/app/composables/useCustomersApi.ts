import { buildQuery } from '~/utils/http'
import { useApiClient } from '~/composables/useApiClient'
import type {
  CustomerAddressResponse,
  CustomerRequest,
  CustomerResponse,
  DeleteCustomerResponse,
  PutCustomerAddressRequest,
  QueryCustomersParams,
  QueryCustomersResponse
} from '~/types/customers'

const ADMIN_CUSTOMERS_PATH = 'admin/customers'

export function useCustomersApi() {
  const api = useApiClient()

  async function queryCustomers(params: QueryCustomersParams) {
    const query = buildQuery({
      Page: params.page,
      PageSize: params.pageSize
    })

    return api.request<QueryCustomersResponse>(
      'customers',
      `${ADMIN_CUSTOMERS_PATH}${query}`
    )
  }

  async function getCustomerById(customerId: string) {
    return api.request<CustomerResponse>(
      'customers',
      `${ADMIN_CUSTOMERS_PATH}/${customerId}`
    )
  }

  async function getAddressByCustomer(customerId: string) {
    return api.request<CustomerAddressResponse>(
      'customers',
      `${ADMIN_CUSTOMERS_PATH}/${customerId}/address`
    )
  }

  async function putAddressByCustomer(
    customerId: string,
    payload: PutCustomerAddressRequest
  ) {
    return api.request<CustomerAddressResponse>(
      'customers',
      `${ADMIN_CUSTOMERS_PATH}/${customerId}/address`,
      {
        method: 'PUT',
        body: JSON.stringify(payload)
      }
    )
  }

  async function createCustomer(payload: CustomerRequest) {
    return api.request<CustomerResponse>('customers', ADMIN_CUSTOMERS_PATH, {
      method: 'POST',
      body: JSON.stringify(payload)
    })
  }

  async function updateCustomer(
    systemUserId: string,
    payload: CustomerRequest
  ) {
    return api.request<CustomerResponse>(
      'customers',
      `${ADMIN_CUSTOMERS_PATH}/${systemUserId}`,
      {
        method: 'PATCH',
        body: JSON.stringify(payload)
      }
    )
  }

  async function deleteCustomer(systemUserId: string) {
    return api.request<DeleteCustomerResponse>(
      'customers',
      `${ADMIN_CUSTOMERS_PATH}/${systemUserId}`,
      {
        method: 'DELETE'
      }
    )
  }

  return {
    queryCustomers,
    getCustomerById,
    getAddressByCustomer,
    putAddressByCustomer,
    createCustomer,
    updateCustomer,
    deleteCustomer
  }
}
