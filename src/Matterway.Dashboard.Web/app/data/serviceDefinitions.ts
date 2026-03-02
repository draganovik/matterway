import type {
  PermissionLevel,
  ServiceSection,
} from "~/types/services/definitions"

const readAllowed: PermissionLevel[] = ["observer", "operator", "manager"]
const operateAllowed: PermissionLevel[] = ["operator", "manager"]

export const serviceSections: ServiceSection[] = [
  {
    key: "catalog",
    label: "Catalog",
    service: "catalog",
    allowed: [...readAllowed],
    features: [
      {
        key: "articles",
        label: "Articles",
        route: "/catalog/articles",
        service: "catalog",
        allowed: [...readAllowed],
        actions: [],
      },
      {
        key: "details",
        label: "Details",
        route: "/catalog/details",
        service: "catalog",
        allowed: [...readAllowed],
        actions: [],
      },
      {
        key: "discounts",
        label: "Discounts",
        route: "/catalog/discounts",
        service: "catalog",
        allowed: [...readAllowed],
        actions: [],
      },
      {
        key: "archive",
        label: "Catalog Data",
        route: "/catalog/archive",
        service: "catalog",
        allowed: [...operateAllowed],
        actions: [],
      },
    ],
  },
  {
    key: "customers",
    label: "Customers",
    service: "customers",
    allowed: [...readAllowed],
    features: [
      {
        key: "customers",
        label: "Customers",
        route: "/customers/customers",
        service: "customers",
        allowed: [...readAllowed],
        actions: [],
      },
    ],
  },
  {
    key: "sales",
    label: "Sales",
    service: "sales",
    allowed: [...readAllowed],
    features: [
      {
        key: "orders",
        label: "Orders",
        route: "/sales/orders",
        service: "sales",
        allowed: [...readAllowed],
        actions: [],
      },
    ],
  },
  {
    key: "identity",
    label: "Identity",
    service: "identity",
    allowed: [...operateAllowed],
    features: [
      {
        key: "system-users",
        label: "System Users",
        route: "/identity/system-users",
        service: "identity",
        allowed: [...operateAllowed],
        actions: [],
      },
    ],
  },
]

export const allFeatures = serviceSections.flatMap(
  (section) => section.features,
)
