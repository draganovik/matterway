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

export const adminServices: ServiceSection[] = [
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
        minimum: 'operator',
        actions: [
          { key: 'query', label: 'Query', method: 'GET', path: 'admin/articles', permission: 'observer' },
          { key: 'create', label: 'Create', method: 'POST', path: 'admin/articles', permission: 'operator' },
          { key: 'update', label: 'Update', method: 'PATCH', path: 'admin/articles/{id}', permission: 'operator' },
          { key: 'delete', label: 'Delete', method: 'DELETE', path: 'admin/articles/{id}', permission: 'operator' }
        ]
      },
      {
        key: 'article-images',
        label: 'Article Images',
        route: '/catalog/article-images',
        service: 'catalog',
        minimum: 'operator',
        actions: [
          {
            key: 'query',
            label: 'Query',
            method: 'GET',
            path: 'admin/articles/{articleId}/images',
            permission: 'observer'
          },
          {
            key: 'create',
            label: 'Create',
            method: 'POST',
            path: 'admin/articles/{articleId}/images',
            permission: 'operator'
          },
          {
            key: 'update',
            label: 'Update',
            method: 'PATCH',
            path: 'admin/articles/{articleId}/images/{orderIndex}',
            permission: 'operator'
          },
          {
            key: 'delete',
            label: 'Delete',
            method: 'DELETE',
            path: 'admin/articles/{articleId}/images/{orderIndex}',
            permission: 'operator'
          }
        ]
      },
      {
        key: 'article-details',
        label: 'Article Details',
        route: '/catalog/article-details',
        service: 'catalog',
        minimum: 'operator',
        actions: [
          {
            key: 'query',
            label: 'Query',
            method: 'GET',
            path: 'admin/articles/{articleId}/details',
            permission: 'observer'
          },
          {
            key: 'create',
            label: 'Create',
            method: 'POST',
            path: 'admin/articles/{articleId}/details',
            permission: 'operator'
          },
          {
            key: 'update',
            label: 'Update',
            method: 'PATCH',
            path: 'admin/articles/{articleId}/details/{detailSlug}',
            permission: 'operator'
          },
          {
            key: 'delete',
            label: 'Delete',
            method: 'DELETE',
            path: 'admin/articles/{articleId}/details/{detailSlug}',
            permission: 'operator'
          }
        ]
      },
      {
        key: 'article-specs',
        label: 'Article Specifications',
        route: '/catalog/article-specifications',
        service: 'catalog',
        minimum: 'operator',
        actions: [
          {
            key: 'query',
            label: 'Query',
            method: 'GET',
            path: 'admin/articles/{articleId}/specifications',
            permission: 'observer'
          },
          {
            key: 'create',
            label: 'Create',
            method: 'POST',
            path: 'admin/articles/{articleId}/specifications',
            permission: 'operator'
          },
          {
            key: 'update',
            label: 'Update',
            method: 'PATCH',
            path: 'admin/articles/{articleId}/specifications/{specificationSlug}',
            permission: 'operator'
          },
          {
            key: 'delete',
            label: 'Delete',
            method: 'DELETE',
            path: 'admin/articles/{articleId}/specifications/{specificationSlug}',
            permission: 'operator'
          }
        ]
      },
      {
        key: 'details',
        label: 'Details',
        route: '/catalog/details',
        service: 'catalog',
        minimum: 'observer',
        actions: [
          { key: 'query', label: 'Query', method: 'GET', path: 'admin/details', permission: 'observer' },
          { key: 'update', label: 'Update', method: 'PUT', path: 'admin/details/{slug}', permission: 'operator' },
          { key: 'delete', label: 'Delete', method: 'DELETE', path: 'admin/details/{slug}', permission: 'operator' }
        ]
      },
      {
        key: 'specifications',
        label: 'Specifications',
        route: '/catalog/specifications',
        service: 'catalog',
        minimum: 'observer',
        actions: [
          {
            key: 'query',
            label: 'Query',
            method: 'GET',
            path: 'admin/specifications',
            permission: 'observer'
          },
          {
            key: 'update',
            label: 'Update',
            method: 'PUT',
            path: 'admin/specifications/{slug}',
            permission: 'operator'
          },
          {
            key: 'delete',
            label: 'Delete',
            method: 'DELETE',
            path: 'admin/specifications/{slug}',
            permission: 'operator'
          }
        ]
      },
      {
        key: 'discounts',
        label: 'Discounts',
        route: '/catalog/discounts',
        service: 'catalog',
        minimum: 'operator',
        actions: [
          { key: 'query', label: 'Query', method: 'GET', path: 'admin/discounts', permission: 'observer' },
          { key: 'create', label: 'Create', method: 'POST', path: 'admin/discounts', permission: 'operator' },
          {
            key: 'update',
            label: 'Update',
            method: 'PUT',
            path: 'admin/discounts/{code}',
            permission: 'operator'
          },
          {
            key: 'delete',
            label: 'Delete',
            method: 'DELETE',
            path: 'admin/discounts/{code}',
            permission: 'operator'
          }
        ]
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

export function getFeatureByRoute(route: string) {
  for (const service of adminServices) {
    const match = service.features.find(feature =>
      route === feature.route || route.startsWith(`${feature.route}/`)
    )
    if (match) return match
  }
  return null
}
