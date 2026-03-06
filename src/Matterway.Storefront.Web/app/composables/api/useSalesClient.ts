import { useApiClient } from "~/composables/api/useApiClient"
import type { PaginatedPayload } from "~/types/common/api"
import type { PlaceOrderPayload, SalesOrder } from "~/types/sales/orders"

export function useSalesClient() {
  const api = useApiClient()

  async function placeSelfOrder(payload: PlaceOrderPayload) {
    return api.request<SalesOrder>("sales", "self/orders", {
      method: "POST",
      body: JSON.stringify(payload),
    })
  }

  async function listSelfOrders(page = 1, pageSize = 20) {
    return api.request<PaginatedPayload<SalesOrder>>(
      "sales",
      `self/orders?page=${page}&pageSize=${pageSize}`,
      {
        method: "GET",
      },
    )
  }

  return {
    placeSelfOrder,
    listSelfOrders,
  }
}
