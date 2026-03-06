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
        allowed: [...managerAllowed],
        actions: [],
      },
    ],
  },
  {
    key: "users",
    label: "Users",
    service: "customers",
    allowed: [...readAllowed],
    features: [
      {
        key: "customers",
        label: "Customers",
        route: "/users/customers",
        service: "customers",
        allowed: [...readAllowed],
        actions: [],
      },
      {
        key: "accounts",
        label: "Accounts",
        route: "/users/accounts",
        service: "identity",
        allowed: [...operateAllowed],
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
]

export const allFeatures = serviceSections.flatMap(
  (section) => section.features,
)
