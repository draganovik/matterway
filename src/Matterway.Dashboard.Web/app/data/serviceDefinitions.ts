import type { ServiceSection } from "~/types/services/definitions"

export const serviceSections: ServiceSection[] = [
  {
    key: "catalog",
    label: "Catalog",
    service: "catalog",
    minimum: "observer",
    features: [
      {
        key: "articles",
        label: "Articles",
        route: "/catalog/articles",
        service: "catalog",
        minimum: "observer",
        actions: [],
      },
      {
        key: "details",
        label: "Details",
        route: "/catalog/details",
        service: "catalog",
        minimum: "observer",
        actions: [],
      },
      {
        key: "discounts",
        label: "Discounts",
        route: "/catalog/discounts",
        service: "catalog",
        minimum: "observer",
        actions: [],
      },
      {
        key: "archive",
        label: "Catalog Data",
        route: "/catalog/archive",
        service: "catalog",
        minimum: "operator",
        actions: [],
      },
    ],
  },
  {
    key: "customers",
    label: "Customers",
    service: "customers",
    minimum: "observer",
    features: [
      {
        key: "customers",
        label: "Customers",
        route: "/customers/customers",
        service: "customers",
        minimum: "observer",
        actions: [],
      },
    ],
  },
  {
    key: "sales",
    label: "Sales",
    service: "sales",
    minimum: "observer",
    features: [
      {
        key: "orders",
        label: "Orders",
        route: "/sales/orders",
        service: "sales",
        minimum: "observer",
        actions: [],
      },
    ],
  },
  {
    key: "identity",
    label: "Identity",
    service: "identity",
    minimum: "operator",
    features: [
      {
        key: "system-users",
        label: "System Users",
        route: "/identity/system-users",
        service: "identity",
        minimum: "operator",
        actions: [],
      },
    ],
  },
]

export const allFeatures = serviceSections.flatMap(
  (section) => section.features,
)
