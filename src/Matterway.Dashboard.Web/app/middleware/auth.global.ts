import { useAuthSession } from '~/composables/useAuthSession'
import { getFeatureByRoute } from '~/data/serviceRegistry'

export default defineNuxtRouteMiddleware(async (to) => {
  const auth = useAuthSession()

  if (to.meta.public) {
    void auth.initialize()
    return
  }

  await auth.initialize()

  if (!auth.isLoggedIn.value || !auth.isEmployee.value) {
    return navigateTo('/login')
  }

  const requiredService = to.meta?.service as string | undefined
  const requiredLevel = to.meta?.level as 'observer' | 'operator' | 'administrator' | undefined
  if (requiredService && requiredLevel) {
    if (!auth.hasPermission(requiredService, requiredLevel)) {
      return navigateTo('/')
    }
  }

  const metaAction = typeof to.meta?.action === 'string' ? to.meta.action : null
  const actionParam = to.params?.action
  const paramAction = typeof actionParam === 'string' ? actionParam : Array.isArray(actionParam) ? actionParam[0] : null
  const featureForAction = getFeatureByRoute(to.path)
  const pathAction = featureForAction && to.path.startsWith(`${featureForAction.route}/`)
    ? to.path.slice(featureForAction.route.length + 1).split('/')[0]
    : null
  const actionKey = metaAction || paramAction || pathAction
  if (actionKey && featureForAction) {
    const action = featureForAction.actions.find(item => item.key === actionKey)
    if (action && !auth.hasPermission(featureForAction.service, action.permission)) {
      return navigateTo(featureForAction.route)
    }
  }
})
