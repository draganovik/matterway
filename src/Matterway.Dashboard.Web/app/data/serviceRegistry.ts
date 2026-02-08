import { allFeatures, serviceSections } from '~/data/serviceDefinitions'
import type { PermissionLevel, ServiceSection } from '~/types/services/definitions'

export {
  serviceSections,
  allFeatures
} from '~/data/serviceDefinitions'
export type {
  PermissionLevel,
  ActionKey,
  FeatureAction,
  FeatureDefinition,
  ServiceSection
} from '~/types/services/definitions'

export { permissionLevels, permissionServices } from '~/data/permissionOptions'

type HasPermission = (service: ServiceSection['service'], minimum: PermissionLevel) => boolean

export function getAuthorizedSections(hasPermission: HasPermission) {
  return serviceSections
    .filter(section => hasPermission(section.service, section.minimum))
    .map(section => ({
      ...section,
      features: section.features.filter(feature =>
        hasPermission(feature.service, feature.minimum)
      )
    }))
    .filter(section => section.features.length > 0)
}

export function getFeatureByRoute(route: string) {
  for (const feature of allFeatures) {
    if (route === feature.route || route.startsWith(`${feature.route}/`)) {
      return feature
    }
  }
  return null
}
