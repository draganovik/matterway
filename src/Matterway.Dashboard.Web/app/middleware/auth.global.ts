import { useAuthSessionStore } from "~/composables/stores/useAuthSessionStore"
import { getFeatureByRoute } from "~/data/serviceRegistry"

export default defineNuxtRouteMiddleware(async (to) => {
  const auth = useAuthSessionStore()

  if (to.meta.public) {
    await auth.initialize()
    return
  }

  await auth.initialize()

  if (!auth.isLoggedIn.value || !auth.isEmployee.value) {
    return navigateTo("/login")
  }

  const feature = getFeatureByRoute(to.path)
  const requiredService = to.meta.service ?? feature?.service
  const requiredPermissions = to.meta.permissions ?? feature?.allowed

  if (requiredService && requiredPermissions?.length) {
    if (!auth.hasPermission(requiredService, requiredPermissions)) {
      await auth.logout()
      return navigateTo("/login")
    }
  }
})
