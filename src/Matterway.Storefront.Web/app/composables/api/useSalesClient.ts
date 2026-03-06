import { useApiClient } from "~/composables/api/useApiClient"
import type { PaginatedPayload } from "~/types/common/api"
import type { SalesOrder } from "~/types/sales/orders"

export function useSalesClient() {
  const api = useApiClient()

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
    listSelfOrders,
  }
}
