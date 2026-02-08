import { allFeatures } from '~/data/serviceDefinitions'

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

export function getFeatureByRoute(route: string) {
  for (const feature of allFeatures) {
    if (route === feature.route || route.startsWith(`${feature.route}/`)) {
      return feature
    }
  }
  return null
}
