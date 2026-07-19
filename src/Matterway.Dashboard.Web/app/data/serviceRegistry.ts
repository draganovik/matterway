import { allFeatures, serviceSections } from "~/data/serviceDefinitions"
import type {
  FeatureDefinition,
  PermissionLevel,
} from "~/types/services/definitions"

export { serviceSections } from "~/data/serviceDefinitions"
export { permissionServices } from "~/data/permissionOptions"

type HasPermission = (
  service: FeatureDefinition["service"],
  allowed: PermissionLevel[],
) => boolean

export function getAuthorizedSections(hasPermission: HasPermission) {
  return serviceSections
    .map((section) => ({
      ...section,
      features: section.features.filter((feature) =>
        hasPermission(feature.service, feature.allowed),
      ),
    }))
    .filter((section) => section.features.length > 0)
}

export function getFeatureByRoute(route: string) {
  for (const feature of allFeatures) {
    if (route === feature.route || route.startsWith(`${feature.route}/`)) {
      return feature
    }
  }
  return null
}
