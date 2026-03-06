import { useAuthSessionStore } from "~/composables/stores/useAuthSessionStore"
import { useCartStore } from "~/composables/stores/useCartStore"
import { useCustomerSessionSync } from "~/composables/features/auth/useCustomerSessionSync"

export function useCartPage() {
  const auth = useAuthSessionStore()
  const cart = useCartStore()
  const { syncCustomerSession } = useCustomerSessionSync()

  const isEmpty = computed(() => cart.totalItems.value === 0)

  async function initialize() {
    await syncCustomerSession({ force: true })
  }

  function goToCheckout() {
    if (isEmpty.value) return
    if (!auth.isLoggedIn.value) {
      void navigateTo("/login?next=%2Fcheckout")
      return
    }
    void navigateTo("/checkout")
  }

  async function clearCart() {
    await cart.clear()
  }

  return {
    cart,
    isEmpty,
    initialize,
    goToCheckout,
    clearCart,
  }
}
