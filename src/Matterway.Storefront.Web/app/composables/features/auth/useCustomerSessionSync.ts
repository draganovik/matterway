import { useAuthSessionStore } from "~/composables/stores/useAuthSessionStore"
import { useCartStore } from "~/composables/stores/useCartStore"
import { useCustomersClient } from "~/composables/api/useCustomersClient"

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
  const customersApi = useCustomersClient()
  const runtime = useCustomerSessionSyncRuntime()
  const syncedCustomerId = useState<string | null>(
    "storefront-customer-session-synced-customer-id",
    () => null,
  )
  const profiledCustomerId = useState<string | null>(
    "storefront-customer-session-profiled-customer-id",
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
      profiledCustomerId.value = null
      auth.setCustomerFirstName(null)
      return { ok: true as const, skipped: true as const }
    }

    const shouldSyncCart =
      options.force || syncedCustomerId.value !== customerId
    const shouldLoadProfile = profiledCustomerId.value !== customerId

    if (!shouldSyncCart && !shouldLoadProfile) {
      return { ok: true as const, skipped: true as const }
    }

    if (runtime.syncPromise) {
      return runtime.syncPromise
    }

    runtime.syncPromise = (async () => {
      const [cartResult, profileResponse] = await Promise.all([
        shouldSyncCart
          ? cart.refreshFromRemote()
          : Promise.resolve({ ok: true as const, skipped: true as const }),
        shouldLoadProfile
          ? customersApi.getSelfProfile().catch(() => null)
          : Promise.resolve(null),
      ])

      if (shouldLoadProfile) {
        profiledCustomerId.value = customerId
        auth.setCustomerFirstName(
          profileResponse?.ok ? profileResponse.data?.firstName : null,
        )
      }

      if (shouldSyncCart && cartResult.ok) {
        syncedCustomerId.value = customerId
      }
      return cartResult
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
