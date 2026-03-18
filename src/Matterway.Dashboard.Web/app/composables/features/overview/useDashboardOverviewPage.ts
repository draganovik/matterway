import { useAuthorizedSections } from "~/composables/workflows/useAuthorizedSections"
import { useAuthSessionStore } from "~/composables/stores/useAuthSessionStore"
import { getJwtStringClaim, type JwtPayload } from "~/utils/jwt"
import type {
  FeatureDefinition,
  PermissionLevel,
  ServiceSection,
} from "~/types/services/definitions"

type OverviewSectionTheme = {
  eyebrow: string
  description: string
  icon: string
  glowClass: string
  iconClass: string
  dotClass: string
}

type OverviewSection = ServiceSection &
  OverviewSectionTheme & {
    featureCount: number
    sectionRoute: string
    primaryFeature: FeatureDefinition | null
  }

type OverviewLink = {
  key: string
  label: string
  route: string
  icon: string
  sectionLabel: string
}

type OverviewStat = {
  label: string
  value: string
  note: string
}

const permissionPriority: Record<PermissionLevel, number> = {
  observer: 1,
  operator: 2,
  manager: 3,
}

const sectionThemes: Record<string, OverviewSectionTheme> = {
  catalog: {
    eyebrow: "Katalog",
    description:
      "Upravljajte artiklima, detaljima i popustima na jednom mestu.",
    icon: "i-lucide-boxes",
    glowClass: "bg-amber-500/20 dark:bg-amber-400/20",
    iconClass:
      "bg-amber-500/12 text-amber-700 dark:bg-amber-400/15 dark:text-amber-200",
    dotClass: "bg-amber-500",
  },
  users: {
    eyebrow: "Korisnici",
    description:
      "Vodite evidenciju kupaca, zaposlenih naloga i prava pristupa na jednom mestu.",
    icon: "i-lucide-users-round",
    glowClass: "bg-sky-500/20 dark:bg-sky-400/20",
    iconClass:
      "bg-sky-500/12 text-sky-700 dark:bg-sky-400/15 dark:text-sky-200",
    dotClass: "bg-sky-500",
  },
  sales: {
    eyebrow: "Prodaja",
    description:
      "Pratite porudžbine, proveravajte detalje i upravljajte prodajom uz manje koraka.",
    icon: "i-lucide-receipt-text",
    glowClass: "bg-emerald-500/20 dark:bg-emerald-400/20",
    iconClass:
      "bg-emerald-500/12 text-emerald-700 dark:bg-emerald-400/15 dark:text-emerald-200",
    dotClass: "bg-emerald-500",
  },
}

const defaultTheme: OverviewSectionTheme = {
  eyebrow: "Servis",
  description: "Otvorite alate koji su vam dostupni u ovom delu administracije.",
  icon: "i-lucide-layout-panel-top",
  glowClass: "bg-primary/20",
  iconClass: "bg-primary/12 text-primary",
  dotClass: "bg-primary",
}

function formatPermissionLabel(level: PermissionLevel | null) {
  if (!level) return "Ograničeno"
  if (level === "observer") return "Pregled"
  if (level === "operator") return "Operater"
  return "Menadžer"
}

function resolveHighestPermission(values: string[]) {
  let highest: PermissionLevel | null = null

  for (const value of values) {
    const [, rawLevel] = value.split(":", 2)
    const normalized = rawLevel?.trim().toLowerCase()
    if (
      normalized !== "observer" &&
      normalized !== "operator" &&
      normalized !== "manager"
    ) {
      continue
    }

    if (
      !highest ||
      permissionPriority[normalized] > permissionPriority[highest]
    ) {
      highest = normalized
    }
  }

  return highest
}

function resolveEmployeeLabel(payload: JwtPayload | null) {
  const candidates = [
    getJwtStringClaim(payload, "name"),
    getJwtStringClaim(payload, "preferred_username"),
    getJwtStringClaim(payload, "email"),
    getJwtStringClaim(payload, "sub"),
  ].filter(Boolean) as string[]

  const raw = candidates[0] || "Zaposleni"
  return raw.includes("@") ? raw.split("@")[0] : raw
}

export function useDashboardOverviewPage() {
  const auth = useAuthSessionStore()
  const authorizedSections = useAuthorizedSections()

  const highestPermission = computed(() =>
    resolveHighestPermission(auth.permissions.value),
  )

  const employeeLabel = computed(() => resolveEmployeeLabel(auth.payload.value))

  const sections = computed<OverviewSection[]>(() =>
    authorizedSections.value.map((section) => {
      const theme = sectionThemes[section.key] || defaultTheme
      return {
        ...section,
        ...theme,
        featureCount: section.features.length,
        sectionRoute: `/${section.key}`,
        primaryFeature: section.features[0] || null,
      }
    }),
  )

  const featureCount = computed(() =>
    sections.value.reduce((sum, section) => sum + section.featureCount, 0),
  )

  const quickLinks = computed<OverviewLink[]>(() =>
    sections.value.flatMap((section) =>
      section.features.slice(0, 2).map((feature) => ({
        key: `${section.key}-${feature.key}`,
        label: feature.label,
        route: feature.route,
        icon: section.icon,
        sectionLabel: section.label,
      })),
    ),
  )

  const primaryLinks = computed(() => quickLinks.value.slice(0, 3))
  const secondaryLinks = computed(() => quickLinks.value.slice(3, 7))

  const heroStats = computed<OverviewStat[]>(() => [
    {
      label: "Sekcije",
      value: String(sections.value.length),
      note: "servisi kojima imate pristup",
    },
    {
      label: "Opcije",
      value: String(featureCount.value),
      note: "dostupne stranice",
    },
    {
      label: "Pristup",
      value: formatPermissionLabel(highestPermission.value),
      note: `${auth.permissions.value.length} dodeljenih dozvola`,
    },
  ])

  return {
    employeeLabel,
    featureCount,
    heroStats,
    highestPermissionLabel: computed(() =>
      formatPermissionLabel(highestPermission.value),
    ),
    primaryLinks,
    secondaryLinks,
    sections,
  }
}
