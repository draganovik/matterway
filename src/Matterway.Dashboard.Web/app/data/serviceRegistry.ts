import { allFeatures } from '~/data/serviceDefinitions'

export {
  type PermissionLevel,
  type ActionKey,
  type FeatureAction,
  type FeatureDefinition,
  type ServiceSection,
  serviceSections,
  allFeatures
} from '~/data/serviceDefinitions'

export { permissionLevels, permissionServices } from '~/data/permissionOptions'

export function getFeatureByRoute(route: string) {
  for (const feature of allFeatures) {
    if (route === feature.route || route.startsWith(`${feature.route}/`)) {
      return feature
    }
  }
  return null
}
