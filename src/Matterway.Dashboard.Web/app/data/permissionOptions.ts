import { allFeatures } from "~/data/serviceDefinitions"

export const permissionLevels = [
  { label: "Observer", value: "observer" },
  { label: "Operator", value: "operator" },
  { label: "Manager", value: "manager" },
] as const

const serviceLabelMap: Record<string, string> = {
  catalog: "Catalog",
  customers: "Customers",
  identity: "Identity",
  sales: "Sales",
}

export const permissionServices = [
  ...new Set(allFeatures.map((f) => f.service)),
]
  .sort((left, right) => left.localeCompare(right))
  .map((service) => ({
    label: serviceLabelMap[service] || service,
    value: service,
  }))
