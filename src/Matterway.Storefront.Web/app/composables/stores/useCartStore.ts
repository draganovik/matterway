import type { CatalogArticle } from "~/types/catalog"
import type { CartItem } from "~/types/cart"
import { useAuthSessionStore } from "~/composables/stores/useAuthSessionStore"
import { useCustomersClient } from "~/composables/api/useCustomersClient"

const storageKey = "mw-storefront-cart-v1"
const articleCodePattern = /^[A-Z0-9]{8}$/

function normalizeNumber(value: unknown, fallback = 0) {
  const parsed = Number(value)
  return Number.isFinite(parsed) ? parsed : fallback
}

function normalizeArticleCode(value: unknown) {
  const normalized = String(value ?? "")
    .trim()
    .toUpperCase()
  return articleCodePattern.test(normalized) ? normalized : ""
}

function createCartItem(article: CatalogArticle): CartItem {
  const articleCode = normalizeArticleCode(article.code)
  return {
    articleCode,
    articleName: article.title,
    unitPrice: normalizeNumber(article.price ?? article.basePrice, 0),
    quantity: 1,
  }
}

function mapRemoteCartItem(item: {
  articleCode?: string
  articleName?: string
  unitPrice?: number
  quantity?: number
}): CartItem | null {
  const articleCode = normalizeArticleCode(item.articleCode)
  if (!articleCode) return null

  return {
    articleCode,
    articleName: item.articleName ? String(item.articleName) : "",
    unitPrice: normalizeNumber(item.unitPrice),
    quantity: Math.max(1, normalizeNumber(item.quantity, 1)),
  }
}

export function useCartStore() {
  const auth = useAuthSessionStore()
  const customers = useCustomersClient()
  const items = useState<CartItem[]>("storefront-cart-items", () => [])
  const hydrated = useState("storefront-cart-hydrated", () => false)
  const watchStarted = useState("storefront-cart-watch-started", () => false)

  function hydrate() {
    if (!import.meta.client || hydrated.value) return
    try {
      const raw = window.localStorage.getItem(storageKey)
      if (!raw) {
        hydrated.value = true
        return
      }
      const parsed = JSON.parse(raw)
      if (!Array.isArray(parsed)) {
        hydrated.value = true
        return
      }
      items.value = parsed
        .map((item) => {
          const source =
            item && typeof item === "object"
              ? (item as Record<string, unknown>)
              : {}
          const articleCode = normalizeArticleCode(source.articleCode)
          return {
            articleCode,
            articleName: String(source.articleName ?? ""),
            unitPrice: normalizeNumber(source.unitPrice),
            quantity: Math.max(1, normalizeNumber(source.quantity, 1)),
          }
        })
        .filter((item: CartItem) => item.articleCode)
    } catch {
      items.value = []
    } finally {
      hydrated.value = true
    }
  }

  function persist() {
    if (!import.meta.client || !hydrated.value) return
    window.localStorage.setItem(storageKey, JSON.stringify(items.value))
  }

  if (import.meta.client && !watchStarted.value) {
    watch(
      items,
      () => {
        persist()
      },
      { deep: true },
    )
    watchStarted.value = true
  }

  if (import.meta.client && getCurrentInstance()) {
    onMounted(() => {
      hydrate()
    })
  }

  async function syncItem(articleCode: string, quantity: number) {
    if (!auth.isLoggedIn.value || !auth.isCustomer.value) return
    await customers.upsertSelfCartItem(articleCode, quantity)
  }

  async function removeRemoteItem(articleCode: string) {
    if (!auth.isLoggedIn.value || !auth.isCustomer.value) return
    await customers.deleteSelfCartItem(articleCode)
  }

  async function add(article: CatalogArticle) {
    hydrate()
    const articleCode = normalizeArticleCode(article.code)
    if (!articleCode) return
    const existing = items.value.find(
      (item) => item.articleCode === articleCode,
    )
    if (existing) {
      existing.quantity += 1
      await syncItem(existing.articleCode, existing.quantity)
      return
    }

    const item = createCartItem(article)
    items.value = [...items.value, item]
    await syncItem(item.articleCode, item.quantity)
  }

  async function increase(articleCode: string) {
    hydrate()
    const existing = items.value.find(
      (item) => item.articleCode === articleCode,
    )
    if (!existing) return

    existing.quantity += 1
    await syncItem(existing.articleCode, existing.quantity)
  }

  async function decrease(articleCode: string) {
    hydrate()
    const existing = items.value.find(
      (item) => item.articleCode === articleCode,
    )
    if (!existing) return

    if (existing.quantity <= 1) {
      await remove(articleCode)
      return
    }

    existing.quantity -= 1
    await syncItem(existing.articleCode, existing.quantity)
  }

  async function setQuantity(articleCode: string, quantity: number) {
    hydrate()
    const existing = items.value.find(
      (item) => item.articleCode === articleCode,
    )
    if (!existing) return

    const normalized = Math.max(1, Math.trunc(normalizeNumber(quantity, 1)))
    if (existing.quantity === normalized) return

    existing.quantity = normalized
    await syncItem(existing.articleCode, existing.quantity)
  }

  async function remove(articleCode: string) {
    hydrate()
    items.value = items.value.filter((item) => item.articleCode !== articleCode)
    await removeRemoteItem(articleCode)
  }

  async function clearRemote() {
    if (!auth.isLoggedIn.value || !auth.isCustomer.value) return
    const remote = await customers.listSelfCartItems(1, 100)
    const uniqueIds = Array.from(
      new Set(remote.items.map((item) => item.articleCode).filter(Boolean)),
    ) as string[]

    await Promise.all(
      uniqueIds.map((articleCode) => removeRemoteItem(articleCode)),
    )
  }

  async function clear() {
    hydrate()
    await clearRemote().catch(() => null)
    items.value = []
  }

  async function mergeGuestItemsIntoRemote() {
    hydrate()

    if (!auth.isLoggedIn.value || !auth.isCustomer.value) {
      return { ok: true as const, skipped: true as const }
    }

    const guestItems = items.value.map((item) => ({ ...item }))
    if (!guestItems.length) {
      return { ok: true as const, skipped: true as const }
    }

    const remote = await customers.listSelfCartItems(1, 100)
    if (remote.error) {
      return {
        ok: false as const,
        error: remote.error,
      }
    }

    const remoteQuantities = new Map(
      remote.items
        .map((item) => [item.articleCode, normalizeNumber(item.quantity, 1)])
        .filter(([articleCode]) => Boolean(articleCode)) as Array<
        [string, number]
      >,
    )

    const results = await Promise.all(
      guestItems.map(async (item) => {
        try {
          return await customers.upsertSelfCartItem(
            item.articleCode,
            item.quantity + (remoteQuantities.get(item.articleCode) ?? 0),
          )
        } catch {
          return {
            ok: false as const,
            status: 0,
            error: "Spajanje korpe nije uspelo.",
          }
        }
      }),
    )

    const firstFailure = results.find((result) => !result.ok)
    const refreshed = await refreshFromRemote().catch(() => ({
      ok: false as const,
      error: "Osvežavanje korpe nije uspelo.",
    }))

    if (firstFailure || !refreshed.ok) {
      return {
        ok: false as const,
        error:
          firstFailure && "error" in firstFailure && firstFailure.error
            ? firstFailure.error
            : refreshed.error,
      }
    }

    return { ok: true as const }
  }

  async function refreshFromRemote() {
    hydrate()
    if (!auth.isLoggedIn.value || !auth.isCustomer.value) {
      return { ok: true as const }
    }

    const remote = await customers.listSelfCartItems(1, 100)
    if (remote.error) {
      return {
        ok: false as const,
        error: remote.error,
      }
    }

    items.value = remote.items
      .map(mapRemoteCartItem)
      .filter((item): item is CartItem => item !== null)
    hydrated.value = true

    return { ok: true as const }
  }

  const totalItems = computed(() =>
    items.value.reduce((sum, item) => sum + item.quantity, 0),
  )

  const totalPrice = computed(() =>
    items.value.reduce((sum, item) => sum + item.unitPrice * item.quantity, 0),
  )

  function quantityFor(articleCode: string) {
    return (
      items.value.find((item) => item.articleCode === articleCode)?.quantity ??
      0
    )
  }

  return {
    items,
    totalItems,
    totalPrice,
    add,
    increase,
    decrease,
    setQuantity,
    remove,
    clear,
    mergeGuestItemsIntoRemote,
    refreshFromRemote,
    quantityFor,
  }
}
