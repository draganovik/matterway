import { useAuthSession } from '~/composables/useAuthSession'
import { getFeatureByRoute } from '~/data/serviceRegistry'

export default defineNuxtRouteMiddleware(async (to) => {
  const auth = useAuthSession()

  if (to.meta.public) {
    void auth.initialize()
    return
  }

  await auth.initialize()

  if (!auth.isLoggedIn.value || !auth.isEmployee.value) {
    return navigateTo('/login')
  }

  const feature = getFeatureByRoute(to.path)
  const requiredService = to.meta.service ?? feature?.service
  const requiredLevel = to.meta.level ?? feature?.minimum

  if (requiredService && requiredLevel) {
    if (!auth.hasPermission(requiredService, requiredLevel)) {
      return navigateTo(feature?.route || '/')
    }
  }
})
