import { serviceSections } from '~/data/serviceDefinitions'

export const permissionLevels = [
  { label: 'Observer', value: 'observer' },
  { label: 'Operator', value: 'operator' },
  { label: 'Administrator', value: 'administrator' }
] as const

export const permissionServices = serviceSections.map((service) => ({
  label: service.label,
  value: service.service
}))
