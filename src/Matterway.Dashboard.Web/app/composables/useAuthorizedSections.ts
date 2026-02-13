import { getAuthorizedSections } from "~/data/serviceRegistry"
import { useAuthSession } from "~/composables/useAuthSession"

export function useAuthorizedSections() {
  const auth = useAuthSession()

  return computed(() =>
    getAuthorizedSections((service, minimum) =>
      auth.hasPermission(service, minimum),
    ),
  )
}
