export type PermissionLevel = "observer" | "operator" | "manager"

export type FeatureDefinition = {
  key: string
  label: string
  route: string
  service: "catalog" | "customers" | "identity" | "sales"
  allowed: PermissionLevel[]
}

export type ServiceSection = {
  key: string
  label: string
  icon: string
  service: FeatureDefinition["service"]
  features: FeatureDefinition[]
}
