import { serviceSections } from "~/data/serviceDefinitions"

export const permissionLevels = [
  { label: "Observer", value: "observer" },
  { label: "Operator", value: "operator" },
  { label: "Manager", value: "manager" },
] as const

export const permissionServices = serviceSections.map((service) => ({
  label: service.label,
  value: service.service,
}))
