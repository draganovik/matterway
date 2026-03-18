import type { Ref } from "vue"
import type { OrderResponse } from "~/types/sales"
import { useSalesClient } from "~/composables/api/useSalesClient"
import { useModalCloseReset } from "~/composables/workflows/modal/useModalCloseReset"
import { useRequestState } from "~/composables/workflows/state/useRequestState"

type NullableString = string | null | undefined

type UseOrderRevealOptions = {
  isOpen: Ref<boolean>
  orderId: Ref<NullableString>
  orderLabel?: Ref<NullableString>
  revealErrorMessage: string
}

export function useOrderRevealModal(options: UseOrderRevealOptions) {
  const api = useSalesClient()
  const loadState = useRequestState()

  const order = ref<OrderResponse | null>(null)
  const notFound = ref(false)

  const displayLabel = computed(
    () =>
      options.orderLabel?.value?.trim() ||
      options.orderId.value?.trim() ||
      "Izabrana porudžbina",
  )

  function resetModalState() {
    loadState.loading = false
    loadState.error = ""
    order.value = null
    notFound.value = false
  }

  async function loadOrder() {
    const orderId = options.orderId.value?.trim()

    if (!orderId) {
      loadState.error = "Najpre izaberite porudžbinu."
      order.value = null
      notFound.value = false
      return
    }

    loadState.loading = true
    loadState.error = ""
    order.value = null
    notFound.value = false

    const result = await api.getOrderById(orderId)

    loadState.loading = false

    if (!result.ok) {
      if (result.status === 404) {
        notFound.value = true
        return
      }

      loadState.error = result.error || options.revealErrorMessage
      return
    }

    if (!result.data) {
      notFound.value = true
      return
    }

    order.value = result.data
  }

  useModalCloseReset({
    isOpen: options.isOpen,
    watchSources: [options.orderId],
    onCloseReset: resetModalState,
    onOpen: async () => {
      await loadOrder()
    },
  })

  return {
    order,
    notFound,
    loadState,
    displayLabel,
    loadOrder,
    resetModalState,
  }
}
