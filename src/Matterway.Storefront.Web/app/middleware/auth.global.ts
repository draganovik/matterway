import { useAuthSessionStore } from "~/composables/stores/useAuthSessionStore"
import { useCustomerSessionSync } from "~/composables/features/auth/useCustomerSessionSync"

export default defineNuxtRouteMiddleware(async (to) => {
  const auth = useAuthSessionStore()
  const { syncCustomerSession } = useCustomerSessionSync()

  if (to.meta.public) {
    await auth.initialize()
    await syncCustomerSession()
    return
  }

  await auth.initialize()
  await syncCustomerSession()

  if (!auth.isLoggedIn.value || !auth.isCustomer.value) {
    const nextPath = encodeURIComponent(to.fullPath)
    return navigateTo(`/login?next=${nextPath}`)
  }
})
