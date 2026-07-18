import { buildQuery } from "~/utils/http"
import { useApiClient } from "~/composables/api/useApiClient"
import type {
  AddOrderStatusRequest,
  OrderResponse,
  OrderStatusResponse,
  QueryOrdersParams,
  QueryOrdersResponse,
} from "~/types/sales"

const ADMIN_ORDERS_PATH = "admin/orders"

export function useSalesClient() {
  const api = useApiClient()

  async function queryOrders(params: QueryOrdersParams) {
    const query = buildQuery({
      Page: params.page,
      PageSize: params.pageSize,
      CustomerId: params.customerId || undefined,
    })

    return api.request<QueryOrdersResponse>(
      "sales",
      `${ADMIN_ORDERS_PATH}${query}`,
    )
  }

  async function getOrderById(orderId: string) {
    return api.request<OrderResponse>(
      "sales",
      `${ADMIN_ORDERS_PATH}/${orderId}`,
    )
  }

  async function addOrderStatus(
    orderId: string,
    payload: AddOrderStatusRequest,
  ) {
    return api.request<OrderStatusResponse>(
      "sales",
      `${ADMIN_ORDERS_PATH}/${orderId}/statuses`,
      {
        method: "POST",
        body: payload,
      },
    )
  }

  return {
    queryOrders,
    getOrderById,
    addOrderStatus,
  }
}
