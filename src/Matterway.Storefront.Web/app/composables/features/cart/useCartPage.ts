import { useAuthSessionStore } from "~/composables/stores/useAuthSessionStore"
import { useCartStore } from "~/composables/stores/useCartStore"

export function useCartPage() {
  const auth = useAuthSessionStore()
  const cart = useCartStore()

  const isEmpty = computed(() => cart.totalItems.value === 0)

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
    goToCheckout,
    clearCart,
  }
}
