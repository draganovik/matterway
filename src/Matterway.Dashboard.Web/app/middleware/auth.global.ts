import { useAuthSessionStore } from "~/composables/stores/useAuthSessionStore"
import { getFeatureByRoute } from "~/data/serviceRegistry"

export default defineNuxtRouteMiddleware(async (to) => {
  const auth = useAuthSessionStore()

  await auth.initialize()

  if (to.meta.public) return

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
