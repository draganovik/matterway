import { getFeatureByRoute, type FeatureAction, type ActionKey } from '~/data/adminFeatures'
import { useAuthSession } from '~/composables/useAuthSession'

export function useFeatureTabs() {
  const route = useRoute()
  const auth = useAuthSession()
  const feature = computed(() => getFeatureByRoute(route.path))
  const actionOrder: ActionKey[] = ['query', 'create', 'update', 'delete']

  const tabs = computed<FeatureAction[]>(() => {
    const current = feature.value
    if (!current) return []
    return current.actions
      .filter(action => auth.hasPermission(current.service, action.permission))
      .sort((a, b) => actionOrder.indexOf(a.key) - actionOrder.indexOf(b.key))
  })

  const active = computed<string>(() => {
    const available = tabs.value
    if (!available.length) return ''

    const metaAction = typeof route.meta?.action === 'string' ? route.meta.action : ''
    const actionParam = route.params?.action
    const paramAction = typeof actionParam === 'string'
      ? actionParam
      : Array.isArray(actionParam)
        ? actionParam[0]
        : ''
    const pathAction = feature.value && route.path.startsWith(`${feature.value.route}/`)
      ? route.path.slice(feature.value.route.length + 1).split('/')[0]
      : ''
    const actionKey = metaAction || paramAction || pathAction

    const base = available.find(tab => tab.key === 'query')?.key ?? available[0]?.key ?? ''
    if (!actionKey) return base
    return available.some(tab => tab.key === actionKey) ? actionKey : base
  })

  function getActionRoute(actionKey: ActionKey) {
    const current = feature.value
    if (!current) return ''
    return actionKey === 'query' ? current.route : `${current.route}/${actionKey}`
  }

  function setActive(value: string) {
    const tab = tabs.value.find(item => item.key === value)
    if (!tab) return
    const target = getActionRoute(tab.key)
    if (target) {
      navigateTo(target)
    }
  }

  return {
    feature,
    tabs,
    active,
    setActive,
    getActionRoute
  }
}
