import { useAuthorizedSections } from "~/composables/workflows/useAuthorizedSections"

export function useDashboardOverviewPage() {
  const sections = useAuthorizedSections()

  return {
    sections,
  }
}
