import { getFeatureByRoute, type FeatureAction } from '~/data/adminFeatures'
import { useAuthSession } from '~/composables/useAuthSession'

export function useFeatureTabs() {
  const route = useRoute()
  const auth = useAuthSession()
  const feature = computed(() => getFeatureByRoute(route.path))

  const tabs = computed<FeatureAction[]>(() => {
    const current = feature.value
    if (!current) return []
    return current.actions.filter((action) =>
      auth.hasPermission(current.service, action.permission)
    )
  })

  const active = ref<string>('')

  watch(
    tabs,
    (next) => {
      if (!next.length) {
        active.value = ''
        return
      }
      if (!next.some((tab) => tab.key === active.value)) {
        active.value = next[0]?.key || ''
      }
    },
    { immediate: true }
  )

  return {
    feature,
    tabs,
    active
  }
}
