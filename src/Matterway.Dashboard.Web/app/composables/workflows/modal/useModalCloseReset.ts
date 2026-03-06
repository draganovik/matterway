import type { Ref, WatchSource } from "vue"

type UseModalCloseResetOptions = {
  isOpen: Ref<boolean>
  onCloseReset: () => void
  onOpen?: () => void | Promise<void>
  watchSources?: WatchSource<unknown>[]
  delayMs?: number
}

export function useModalCloseReset(options: UseModalCloseResetOptions) {
  let closeResetTimer: ReturnType<typeof setTimeout> | null = null
  const delayMs = options.delayMs ?? 200

  const sources: WatchSource<unknown>[] = [
    options.isOpen,
    ...(options.watchSources || []),
  ]

  watch(sources, (values) => {
    const open = Boolean(values[0])

    if (!open) {
      if (closeResetTimer) clearTimeout(closeResetTimer)

      closeResetTimer = setTimeout(() => {
        options.onCloseReset()
        closeResetTimer = null
      }, delayMs)

      return
    }

    if (closeResetTimer) {
      clearTimeout(closeResetTimer)
      closeResetTimer = null
    }

    if (options.onOpen) {
      void options.onOpen()
    }
  })

  onBeforeUnmount(() => {
    if (closeResetTimer) clearTimeout(closeResetTimer)
  })
}
