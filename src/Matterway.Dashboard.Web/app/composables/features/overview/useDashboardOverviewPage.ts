import { useAuthorizedSections } from "~/composables/workflows/useAuthorizedSections"
import type { ServiceSection } from "~/types/services/definitions"

type OverviewSectionTheme = {
  description: string
  icon: string
}

type OverviewSection = ServiceSection &
  OverviewSectionTheme & {
    featureCount: number
  }

const sectionThemes: Record<string, OverviewSectionTheme> = {
  catalog: {
    description:
      "Upravljajte artiklima, detaljima i popustima na jednom mestu.",
    icon: "i-lucide-boxes",
  },
  users: {
    description:
      "Vodite evidenciju kupaca, zaposlenih naloga i prava pristupa na jednom mestu.",
    icon: "i-lucide-users-round",
  },
  sales: {
    description:
      "Pratite porudžbine, proveravajte detalje i upravljajte prodajom uz manje koraka.",
    icon: "i-lucide-receipt-text",
  },
}

const defaultTheme: OverviewSectionTheme = {
  description:
    "Otvorite alate koji su vam dostupni u ovom delu administracije.",
  icon: "i-lucide-layout-panel-top",
}

export function useDashboardOverviewPage() {
  const authorizedSections = useAuthorizedSections()

  const sections = computed<OverviewSection[]>(() =>
    authorizedSections.value.map((section) => {
      const theme = sectionThemes[section.key] || defaultTheme
      return {
        ...section,
        ...theme,
        featureCount: section.features.length,
      }
    }),
  )

  return { sections }
}
