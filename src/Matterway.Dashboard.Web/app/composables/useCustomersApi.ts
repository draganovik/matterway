import { buildQuery } from '~/utils/http'
import { useApiClient } from '~/composables/useApiClient'
import type {
  CustomerRequest,
  CustomerResponse,
  DeleteCustomerResponse,
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
    createCustomer,
    updateCustomer,
    deleteCustomer
  }
}
