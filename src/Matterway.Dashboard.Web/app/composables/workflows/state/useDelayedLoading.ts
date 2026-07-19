import type { MaybeRefOrGetter } from "vue"
import { onScopeDispose, readonly, ref, toValue, watch } from "vue"

const DEFAULT_DELAY = 160
const DEFAULT_MINIMUM_DURATION = 240

export function useDelayedLoading(
  source: MaybeRefOrGetter<boolean>,
  delay = DEFAULT_DELAY,
  minimumDuration = DEFAULT_MINIMUM_DURATION,
) {
  const visible = ref(false)
  let shownAt = 0
  let showTimer: ReturnType<typeof setTimeout> | undefined
  let hideTimer: ReturnType<typeof setTimeout> | undefined

  function clearShowTimer() {
    if (showTimer) clearTimeout(showTimer)
    showTimer = undefined
  }

  function clearHideTimer() {
    if (hideTimer) clearTimeout(hideTimer)
    hideTimer = undefined
  }

  watch(
    () => Boolean(toValue(source)),
    (loading) => {
      clearShowTimer()

      if (loading) {
        clearHideTimer()
        if (visible.value) return

        showTimer = setTimeout(() => {
          visible.value = true
          shownAt = Date.now()
          showTimer = undefined
        }, delay)
        return
      }

      if (!visible.value) return

      const elapsed = Date.now() - shownAt
      const remaining = Math.max(0, minimumDuration - elapsed)

      hideTimer = setTimeout(() => {
        visible.value = false
        shownAt = 0
        hideTimer = undefined
      }, remaining)
    },
    { immediate: true },
  )

  onScopeDispose(() => {
    clearShowTimer()
    clearHideTimer()
  })

  return readonly(visible)
}
