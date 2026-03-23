export function useScrollReset(
  source: Parameters<typeof watch>[0],
  options: ScrollIntoViewOptions = { block: "start" },
) {
  const targetRef = ref<HTMLElement | null>(null)
  const pending = ref(false)

  function requestScrollReset() {
    pending.value = true
  }

  function cancelScrollReset() {
    pending.value = false
  }

  watch(
    source,
    () => {
      if (!pending.value) return

      pending.value = false
      targetRef.value?.scrollIntoView(options)
    },
    { flush: "post" },
  )

  return {
    targetRef,
    requestScrollReset,
    cancelScrollReset,
  }
}
