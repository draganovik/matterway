import type { ServiceSection } from '~/types/services/definitions'

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
