export type PermissionLevel = 'observer' | 'operator' | 'administrator'

export type ActionKey = 'create' | 'query' | 'update' | 'delete'

export type FeatureAction = {
  key: ActionKey
  label: string
  method: 'GET' | 'POST' | 'PUT' | 'PATCH' | 'DELETE'
  path: string
  permission: PermissionLevel
  description?: string
}

export type FeatureDefinition = {
  key: string
  label: string
  route: string
  service: 'catalog' | 'customers' | 'identity' | 'sales'
  minimum: PermissionLevel
  actions: FeatureAction[]
}

export type ServiceSection = {
  key: string
  label: string
  service: FeatureDefinition['service']
  minimum: PermissionLevel
  features: FeatureDefinition[]
}
