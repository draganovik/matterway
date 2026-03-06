import { useAuthSessionStore } from "~/composables/stores/useAuthSessionStore"

export default defineNuxtRouteMiddleware(async (to) => {
  const auth = useAuthSessionStore()

  if (to.meta.public) {
    await auth.initialize()
    return
  }

  await auth.initialize()

  if (!auth.isLoggedIn.value || !auth.isCustomer.value) {
    const nextPath = encodeURIComponent(to.fullPath)
    return navigateTo(`/login?next=${nextPath}`)
  }
})
