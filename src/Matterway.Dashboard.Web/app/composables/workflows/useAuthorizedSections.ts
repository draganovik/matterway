import { getAuthorizedSections } from "~/data/serviceRegistry"
import { useAuthSessionStore } from "~/composables/stores/useAuthSessionStore"

export function useAuthorizedSections() {
  const auth = useAuthSessionStore()

  return computed(() =>
    getAuthorizedSections((service, allowed) =>
      auth.hasPermission(service, allowed),
    ),
  )
}
