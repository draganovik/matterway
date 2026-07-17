import type {
  PermissionLevel,
  ServiceSection,
} from "~/types/services/definitions"

const readAllowed: PermissionLevel[] = ["observer", "operator", "manager"]
const operateAllowed: PermissionLevel[] = ["operator", "manager"]
const managerAllowed: PermissionLevel[] = ["manager"]

export const serviceSections: ServiceSection[] = [
  {
    key: "catalog",
    label: "Katalog",
    icon: "i-lucide-package",
    service: "catalog",
    allowed: [...readAllowed],
    features: [
      {
        key: "articles",
        label: "Artikli",
        route: "/catalog/articles",
        service: "catalog",
        allowed: [...readAllowed],
        actions: [],
      },
      {
        key: "details",
        label: "Detalji",
        route: "/catalog/details",
        service: "catalog",
        allowed: [...readAllowed],
        actions: [],
      },
      {
        key: "discounts",
        label: "Popusti",
        route: "/catalog/discounts",
        service: "catalog",
        allowed: [...readAllowed],
        actions: [],
      },
      {
        key: "archive",
        label: "Arhiva kataloga",
        route: "/catalog/archive",
        service: "catalog",
        allowed: [...managerAllowed],
        actions: [],
      },
    ],
  },
  {
    key: "users",
    label: "Korisnici",
    icon: "i-lucide-users",
    service: "customers",
    allowed: [...readAllowed],
    features: [
      {
        key: "customers",
        label: "Kupci",
        route: "/users/customers",
        service: "customers",
        allowed: [...readAllowed],
        actions: [],
      },
      {
        key: "accounts",
        label: "Nalozi",
        route: "/users/accounts",
        service: "identity",
        allowed: [...operateAllowed],
        actions: [],
      },
    ],
  },
  {
    key: "sales",
    label: "Prodaja",
    icon: "i-lucide-receipt-text",
    service: "sales",
    allowed: [...readAllowed],
    features: [
      {
        key: "orders",
        label: "Porudžbine",
        route: "/sales/orders",
        service: "sales",
        allowed: [...readAllowed],
        actions: [],
      },
    ],
  },
]

export const allFeatures = serviceSections.flatMap(
  (section) => section.features,
)
