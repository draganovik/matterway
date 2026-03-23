import { DEFAULT_PAGINATION_PAGE_SIZE } from "~/constants/pagination"
import { useApiClient } from "~/composables/api/useApiClient"
import type { PaginatedPayload } from "~/types/common/api"
import type { SalesOrder } from "~/types/sales"

export function useSalesClient() {
  const api = useApiClient()

  async function listSelfOrders(
    page = 1,
    pageSize = DEFAULT_PAGINATION_PAGE_SIZE,
  ) {
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
