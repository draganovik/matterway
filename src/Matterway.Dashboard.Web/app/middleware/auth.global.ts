import { useAuthSession } from "~/composables/useAuthSession"
import { getFeatureByRoute } from "~/data/serviceRegistry"

export default defineNuxtRouteMiddleware(async (to) => {
  const auth = useAuthSession()

  if (to.meta.public) {
    void auth.initialize()
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
      return navigateTo(feature?.route || "/")
    }
  }
})
