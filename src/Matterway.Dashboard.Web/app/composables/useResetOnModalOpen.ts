import type { Ref } from 'vue'

export function useResetOnModalOpen(isOpen: Ref<boolean>, reset: () => void) {
  watch(isOpen, (open) => {
    if (!open) return
    reset()
  })
}
