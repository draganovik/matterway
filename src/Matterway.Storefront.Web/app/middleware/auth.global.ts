import { useAuthSessionStore } from "~/composables/stores/useAuthSessionStore"
import { useCustomerSessionSync } from "~/composables/features/auth/useCustomerSessionSync"

export default defineNuxtRouteMiddleware(async (to) => {
  const auth = useAuthSessionStore()
  const { syncCustomerSession } = useCustomerSessionSync()

  await auth.initialize()
  await syncCustomerSession()

  if (to.meta.public) return

  if (!auth.isLoggedIn.value || !auth.isCustomer.value) {
    const nextPath = encodeURIComponent(to.fullPath)
    return navigateTo(`/login?next=${nextPath}`)
  }
})
