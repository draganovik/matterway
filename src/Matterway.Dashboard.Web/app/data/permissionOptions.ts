import { allFeatures } from "~/data/serviceDefinitions"

const serviceLabelMap: Record<string, string> = {
  catalog: "Katalog",
  customers: "Kupci",
  identity: "Identitet",
  sales: "Prodaja",
}

export const permissionServices = [
  ...new Set(allFeatures.map((f) => f.service)),
]
  .sort((left, right) => left.localeCompare(right))
  .map((service) => ({
    label: serviceLabelMap[service] || service,
    value: service,
  }))
