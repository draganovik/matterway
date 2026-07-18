import { DEFAULT_PAGINATION_PAGE_SIZE } from "~/constants/pagination"
import { useSalesClient } from "~/composables/api/useSalesClient"
import type { PaginationMeta } from "~/types/common/api"
import type { SalesOrder } from "~/types/sales"

const DEFAULT_PAGE = 1
const PAGE_SIZE = DEFAULT_PAGINATION_PAGE_SIZE

export function useOrdersPage() {
  const salesApi = useSalesClient()

  const loading = ref(true)
  const error = ref("")
  const orders = ref<SalesOrder[]>([])
  const meta = ref<PaginationMeta | null>(null)
  const pagination = reactive({ page: DEFAULT_PAGE })
  const totalCount = computed(
    () => meta.value?.totalCount ?? orders.value.length,
  )
  const totalPages = computed(() => {
    const totalFromApi = meta.value?.totalPages
    if (typeof totalFromApi === "number" && totalFromApi > 0) {
      return totalFromApi
    }
    const count = totalCount.value
    return count > 0 ? Math.ceil(count / PAGE_SIZE) : 0
  })
  const currentPage = computed(() => meta.value?.currentPage ?? pagination.page)
  const statusHistoryOpen = ref(false)
  const itemsOpen = ref(false)
  const selectedOrder = ref<SalesOrder | null>(null)

  watch(
    [statusHistoryOpen, itemsOpen],
    ([isStatusHistoryOpen, isItemsOpen]) => {
      if (!isStatusHistoryOpen && !isItemsOpen) {
        selectedOrder.value = null
      }
    },
  )

  function openStatusHistory(order: SalesOrder) {
    selectedOrder.value = order
    statusHistoryOpen.value = true
  }

  function openItems(order: SalesOrder) {
    selectedOrder.value = order
    itemsOpen.value = true
  }

  async function loadOrders() {
    loading.value = true
    error.value = ""

    const response = await salesApi.listSelfOrders(pagination.page, PAGE_SIZE)
    if (!response.ok) {
      error.value = response.error || "Učitavanje porudžbina nije uspelo."
      orders.value = []
      meta.value = null
      loading.value = false
      return
    }

    orders.value = response.data?.data ?? []
    meta.value = response.data?.meta ?? null
    if (
      typeof meta.value?.currentPage === "number" &&
      meta.value.currentPage > 0
    ) {
      pagination.page = meta.value.currentPage
    }
    loading.value = false
  }

  async function changePage(page: number) {
    if (loading.value || page < 1 || page === pagination.page) return
    if (totalPages.value && page > totalPages.value) return
    pagination.page = page
    await loadOrders()
  }

  return {
    loading,
    error,
    orders,
    pageSize: PAGE_SIZE,
    totalPages,
    totalCount,
    currentPage,
    statusHistoryOpen,
    itemsOpen,
    selectedOrder,
    openStatusHistory,
    openItems,
    loadOrders,
    changePage,
  }
}
