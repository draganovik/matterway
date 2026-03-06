import { useSalesClient } from "~/composables/api/useSalesClient"
import type { SalesOrder } from "~/types/sales/orders"

export function useOrdersPage() {
  const salesApi = useSalesClient()

  const loading = ref(true)
  const error = ref("")
  const orders = ref<SalesOrder[]>([])
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

    const response = await salesApi.listSelfOrders(1, 20)
    if (!response.ok) {
      error.value = response.error || "Učitavanje porudžbina nije uspelo."
      orders.value = []
      loading.value = false
      return
    }

    orders.value = response.data?.data ?? []
    loading.value = false
  }

  async function initialize() {
    await loadOrders()
  }

  return {
    loading,
    error,
    orders,
    statusHistoryOpen,
    itemsOpen,
    selectedOrder,
    openStatusHistory,
    openItems,
    loadOrders,
    initialize,
  }
}
