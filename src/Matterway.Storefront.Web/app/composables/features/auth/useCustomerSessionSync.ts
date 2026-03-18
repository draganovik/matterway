import { useAuthSessionStore } from "~/composables/stores/useAuthSessionStore"
import { useCartStore } from "~/composables/stores/useCartStore"

type CustomerSessionSyncResult =
  | { ok: true; skipped?: true }
  | { ok: false; error?: string }

type CustomerSessionSyncRuntime = {
  syncPromise: Promise<CustomerSessionSyncResult> | null
}

function useCustomerSessionSyncRuntime() {
  const nuxtApp = useNuxtApp() as ReturnType<typeof useNuxtApp> & {
    _mwStorefrontCustomerSessionSync?: CustomerSessionSyncRuntime
  }

  if (!nuxtApp._mwStorefrontCustomerSessionSync) {
    nuxtApp._mwStorefrontCustomerSessionSync = {
      syncPromise: null,
    }
  }

  return nuxtApp._mwStorefrontCustomerSessionSync
}

export function useCustomerSessionSync() {
  const auth = useAuthSessionStore()
  const cart = useCartStore()
  const runtime = useCustomerSessionSyncRuntime()
  const syncedCustomerId = useState<string | null>(
    "storefront-customer-session-synced-customer-id",
    () => null,
  )

  async function syncCustomerSession(options: { force?: boolean } = {}) {
    if (!auth.isInitialized.value) {
      await auth.initialize()
    }

    const customerId = auth.customerId.value?.trim() || null
    if (!auth.isLoggedIn.value || !auth.isCustomer.value || !customerId) {
      if (syncedCustomerId.value) {
        await cart.clear().catch(() => null)
      }
      syncedCustomerId.value = null
      return { ok: true as const, skipped: true as const }
    }

    if (!options.force && syncedCustomerId.value === customerId) {
      return { ok: true as const, skipped: true as const }
    }

    if (runtime.syncPromise) {
      return runtime.syncPromise
    }

    runtime.syncPromise = (async () => {
      const result = await cart.refreshFromRemote()
      if (result.ok) {
        syncedCustomerId.value = customerId
      }
      return result
    })()

    try {
      return await runtime.syncPromise
    } finally {
      runtime.syncPromise = null
    }
  }

  return {
    syncCustomerSession,
  }
}
