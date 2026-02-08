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
  service: 'catalog'
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

export const serviceSections: ServiceSection[] = [
  {
    key: 'catalog',
    label: 'Catalog',
    service: 'catalog',
    minimum: 'observer',
    features: [
      {
        key: 'articles',
        label: 'Articles',
        route: '/catalog/articles',
        service: 'catalog',
        minimum: 'observer',
        actions: []
      },
      {
        key: 'details',
        label: 'Details',
        route: '/catalog/details',
        service: 'catalog',
        minimum: 'observer',
        actions: []
      },
      {
        key: 'discounts',
        label: 'Discounts',
        route: '/catalog/discounts',
        service: 'catalog',
        minimum: 'observer',
        actions: []
      }
    ]
  }
]

export const allFeatures = serviceSections.flatMap(section => section.features)
