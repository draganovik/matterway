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
  if (feature) {
    if (!auth.hasPermission(feature.service, feature.allowed)) {
      return navigateTo("/")
    }
  }
})
