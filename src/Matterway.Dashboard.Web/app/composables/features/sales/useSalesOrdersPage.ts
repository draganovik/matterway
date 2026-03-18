import { useAuthSessionStore } from "~/composables/stores/useAuthSessionStore"
import { useSalesClient } from "~/composables/api/useSalesClient"
import { usePaginationState } from "~/composables/workflows/pagination/usePaginationState"
import { useRequestState } from "~/composables/workflows/state/useRequestState"
import type { OrderResponse } from "~/types/sales"

export function useSalesOrdersPage() {
  const auth = useAuthSessionStore()
  const api = useSalesClient()
  const canManageStatuses = computed(() =>
    auth.hasPermission("sales", ["operator", "manager"]),
  )

  const listState = useRequestState({ empty: "Nema porudžbina." })
  const detailState = useRequestState()

  const orders = ref<OrderResponse[]>([])
  const filter = ref("")
  const {
    pagination,
    resetTotals,
    applyMeta,
    changePage,
    changePageSize,
    searchWithPageReset,
    watchPagination,
  } = usePaginationState({ pageSize: 20 })

  const selectedId = ref<string | null>(null)
  const selectedOrder = ref<OrderResponse | null>(null)

  const revealDetailsModalOpen = ref(false)
  const revealStatusHistoryModalOpen = ref(false)
  const createStatusModalOpen = ref(false)
  const revealPaymentsModalOpen = ref(false)
  const revealItemsModalOpen = ref(false)

  function clearSelection() {
    selectedId.value = null
    selectedOrder.value = null
    revealDetailsModalOpen.value = false
    revealStatusHistoryModalOpen.value = false
    revealPaymentsModalOpen.value = false
    revealItemsModalOpen.value = false
    detailState.error = ""
  }

  async function loadOrders() {
    listState.loading = true
    listState.error = ""

    const result = await api.queryOrders({
      page: pagination.page,
      pageSize: pagination.pageSize,
      customerId: filter.value.trim() || undefined,
    })

    listState.loading = false

    if (!result.ok) {
      listState.error = result.error || "Učitavanje porudžbina nije uspelo."
      orders.value = []
      resetTotals()
      clearSelection()
      return
    }

    if (result.status === 204 || !result.data) {
      orders.value = []
      resetTotals()
      clearSelection()
      return
    }

    orders.value = result.data.data || []
    applyMeta(result.data.meta, orders.value.length)

    if (!selectedId.value) return

    const match =
      orders.value.find((item) => item.id === selectedId.value) || null
    if (!match) clearSelection()
  }

  async function loadOrder(orderId: string) {
    detailState.loading = true
    detailState.error = ""

    const result = await api.getOrderById(orderId)
    detailState.loading = false

    if (!result.ok || !result.data) {
      detailState.error = result.error || "Učitavanje detalja porudžbine nije uspelo."
      selectedOrder.value = null
      return
    }

    selectedOrder.value = result.data
    orders.value = orders.value.map((item) =>
      item.id === result.data?.id ? result.data : item,
    )
  }

  function searchOrders() {
    searchWithPageReset(loadOrders)
  }

  function selectOrder(orderId: string) {
    selectedId.value = orderId
    revealDetailsModalOpen.value = false
    revealStatusHistoryModalOpen.value = false
    createStatusModalOpen.value = false
    revealPaymentsModalOpen.value = false
    revealItemsModalOpen.value = false
    void loadOrder(orderId)
  }

  function revealDetails() {
    if (!selectedOrder.value?.id) return
    revealDetailsModalOpen.value = true
  }

  function revealStatusHistory() {
    if (!selectedOrder.value?.id) return
    revealStatusHistoryModalOpen.value = true
  }

  function openCreateStatus() {
    if (!selectedOrder.value?.id || !canManageStatuses.value) return
    createStatusModalOpen.value = true
  }

  function handleStatusCreated() {
    if (!selectedOrder.value?.id) return
    void loadOrder(selectedOrder.value.id)
  }

  function revealPayments() {
    if (!selectedOrder.value?.id) return
    revealPaymentsModalOpen.value = true
  }

  function revealItems() {
    if (!selectedOrder.value?.id) return
    revealItemsModalOpen.value = true
  }

  const selectedOrderLabel = computed(
    () => selectedOrder.value?.id || "Izabrana porudžbina",
  )

  watchPagination(loadOrders)

  onMounted(() => {
    void loadOrders()
  })

  return {
    canManageStatuses,
    listState,
    detailState,
    orders,
    filter,
    pagination,
    selectedId,
    selectedOrder,
    revealDetailsModalOpen,
    revealStatusHistoryModalOpen,
    createStatusModalOpen,
    revealPaymentsModalOpen,
    revealItemsModalOpen,
    selectedOrderLabel,
    changePage,
    changePageSize,
    searchOrders,
    selectOrder,
    revealDetails,
    revealStatusHistory,
    openCreateStatus,
    handleStatusCreated,
    revealPayments,
    revealItems,
  }
}
