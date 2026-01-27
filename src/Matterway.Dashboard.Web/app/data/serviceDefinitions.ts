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
  service: 'catalog' | 'customers' | 'sales' | 'identity'
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
        key: 'specifications',
        label: 'Specifications',
        route: '/catalog/specifications',
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
  },
  {
    key: 'customers',
    label: 'Customers',
    service: 'customers',
    minimum: 'observer',
    features: [
      {
        key: 'customers',
        label: 'Customers',
        route: '/customers/customers',
        service: 'customers',
        minimum: 'observer',
        actions: [
          { key: 'query', label: 'Query', method: 'GET', path: 'admin/customers', permission: 'observer' },
          { key: 'create', label: 'Create', method: 'POST', path: 'admin/customers', permission: 'operator' },
          {
            key: 'update',
            label: 'Update',
            method: 'PATCH',
            path: 'admin/customers/{systemUserId}',
            permission: 'operator'
          },
          {
            key: 'delete',
            label: 'Delete',
            method: 'DELETE',
            path: 'admin/customers/{systemUserId}',
            permission: 'operator'
          }
        ]
      },
      {
        key: 'cart-items',
        label: 'Cart Items',
        route: '/customers/cart-items',
        service: 'customers',
        minimum: 'observer',
        actions: [
          {
            key: 'query',
            label: 'Query',
            method: 'GET',
            path: 'admin/customers/{customerId}/cart-items/{articleId}',
            permission: 'observer'
          }
        ]
      }
    ]
  },
  {
    key: 'sales',
    label: 'Sales',
    service: 'sales',
    minimum: 'observer',
    features: [
      {
        key: 'orders',
        label: 'Orders',
        route: '/sales/orders',
        service: 'sales',
        minimum: 'observer',
        actions: [
          { key: 'query', label: 'Query', method: 'GET', path: 'admin/orders', permission: 'observer' },
          {
            key: 'create',
            label: 'Create',
            method: 'POST',
            path: 'admin/orders/{orderId}/statuses',
            permission: 'operator'
          }
        ]
      },
      {
        key: 'payments',
        label: 'Payments',
        route: '/sales/payments',
        service: 'sales',
        minimum: 'observer',
        actions: [
          { key: 'query', label: 'Query', method: 'GET', path: 'admin/payments', permission: 'observer' }
        ]
      }
    ]
  },
  {
    key: 'identity',
    label: 'Identity',
    service: 'identity',
    minimum: 'operator',
    features: [
      {
        key: 'system-users',
        label: 'System Users',
        route: '/identity/system-users',
        service: 'identity',
        minimum: 'operator',
        actions: [
          { key: 'query', label: 'Query', method: 'GET', path: 'admin/system-users', permission: 'operator' },
          {
            key: 'update',
            label: 'Update',
            method: 'PATCH',
            path: 'admin/system-users/{id}',
            permission: 'operator'
          },
          {
            key: 'delete',
            label: 'Delete',
            method: 'DELETE',
            path: 'admin/system-users/{id}',
            permission: 'administrator'
          }
        ]
      },
      {
        key: 'user-perms',
        label: 'User Permissions',
        route: '/identity/user-perms',
        service: 'identity',
        minimum: 'operator',
        actions: [
          {
            key: 'query',
            label: 'Query',
            method: 'GET',
            path: 'admin/system-users/{id}/perms',
            permission: 'operator'
          },
          {
            key: 'create',
            label: 'Create',
            method: 'POST',
            path: 'admin/system-users/{id}/perms',
            permission: 'administrator'
          },
          {
            key: 'delete',
            label: 'Remove',
            method: 'DELETE',
            path: 'admin/system-users/{id}/perms',
            permission: 'administrator'
          }
        ]
      }
    ]
  }
]

export const allFeatures = serviceSections.flatMap(section => section.features)
