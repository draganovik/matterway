import { useAuthSessionStore } from "~/composables/stores/useAuthSessionStore"

export default defineNuxtPlugin((nuxtApp) => {
  const auth = useAuthSessionStore()
  const route = useRoute()

  function synchronizeVisibleSession() {
    if (document.visibilityState === "visible") void auth.initialize()
  }

  document.addEventListener("visibilitychange", synchronizeVisibleSession)
  window.addEventListener("focus", synchronizeVisibleSession)

  watch(
    [auth.isInitialized, auth.isLoggedIn, auth.hasRefreshSession],
    ([isInitialized, isLoggedIn, hasRefreshSession]) => {
      if (
        isInitialized &&
        !isLoggedIn &&
        !hasRefreshSession &&
        route.path !== "/login"
      ) {
        void nuxtApp.runWithContext(() => navigateTo("/login"))
      }
    },
  )
})
