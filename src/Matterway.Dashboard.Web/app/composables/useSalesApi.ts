import { buildQuery } from '~/utils/http'
import { useApiClient } from '~/composables/useApiClient'
import type {
  OrderResponse,
  QueryOrdersParams,
  QueryOrdersResponse
} from '~/types/sales'

const ADMIN_ORDERS_PATH = 'admin/orders'

export function useSalesApi() {
  const api = useApiClient()

  async function queryOrders(params: QueryOrdersParams) {
    const query = buildQuery({
      Page: params.page,
      PageSize: params.pageSize,
      CustomerId: params.customerId || undefined
    })

    return api.request<QueryOrdersResponse>(
      'sales',
      `${ADMIN_ORDERS_PATH}${query}`
    )
  }

  async function getOrderById(orderId: string) {
    return api.request<OrderResponse>(
      'sales',
      `${ADMIN_ORDERS_PATH}/${orderId}`
    )
  }

  return {
    queryOrders,
    getOrderById
  }
}
