import { useAuthSession } from '~/composables/useAuthSession'

export default defineNuxtRouteMiddleware(async (to) => {
  const auth = useAuthSession()

  if (import.meta.client) {
    await auth.initialize()
  }

  const isPublic = Boolean(to.meta.public)
  if (isPublic) return

  if (!auth.isLoggedIn.value) {
    if (to.path !== '/login') {
      return navigateTo('/login')
    }
    return
  }

  if (!auth.isEmployee.value) {
    return navigateTo('/login')
  }

  const requiredService = to.meta?.service as string | undefined
  const requiredLevel = to.meta?.level as 'observer' | 'operator' | 'administrator' | undefined
  if (requiredService && requiredLevel) {
    if (!auth.hasPermission(requiredService, requiredLevel)) {
      return navigateTo('/')
    }
  }
})
