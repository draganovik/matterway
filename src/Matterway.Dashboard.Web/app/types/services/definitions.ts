export type PermissionLevel = "observer" | "operator" | "manager"

export type ActionKey = "create" | "query" | "update" | "delete"

export type FeatureAction = {
  key: ActionKey
  label: string
  method: "GET" | "POST" | "PUT" | "PATCH" | "DELETE"
  path: string
  permission: PermissionLevel
  description?: string
}

export type FeatureDefinition = {
  key: string
  label: string
  route: string
  service: "catalog" | "customers" | "identity" | "sales"
  allowed: PermissionLevel[]
  actions: FeatureAction[]
}

export type ServiceSection = {
  key: string
  label: string
  icon: string
  service: FeatureDefinition["service"]
  allowed: PermissionLevel[]
  features: FeatureDefinition[]
}
