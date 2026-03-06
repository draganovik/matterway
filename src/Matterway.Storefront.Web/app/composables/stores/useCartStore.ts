import type { CatalogArticle } from "~/types/catalog/articles"
import type { CartItem } from "~/types/cart"
import { useAuthSessionStore } from "~/composables/stores/useAuthSessionStore"
import { useCustomersClient } from "~/composables/api/useCustomersClient"

const storageKey = "mw-storefront-cart-v1"

function normalizeNumber(value: unknown, fallback = 0) {
  const parsed = Number(value)
  return Number.isFinite(parsed) ? parsed : fallback
}

function createCartItem(article: CatalogArticle): CartItem {
  return {
    articleId: article.id,
    articleCode: article.articleCode,
    articleName: article.title,
    unitPrice: normalizeNumber(article.price ?? article.basePrice, 0),
    quantity: 1,
  }
}

function mapRemoteCartItem(item: {
  articleId?: string
  articleCode?: string
  articleName?: string
  unitPrice?: number
  quantity?: number
}): CartItem | null {
  if (!item.articleId) return null

  return {
    articleId: String(item.articleId),
    articleCode: item.articleCode ? String(item.articleCode) : "",
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
          return {
            articleId: String(source.articleId ?? ""),
            articleCode: String(source.articleCode ?? ""),
            articleName: String(source.articleName ?? ""),
            unitPrice: normalizeNumber(source.unitPrice),
            quantity: Math.max(1, normalizeNumber(source.quantity, 1)),
          }
        })
        .filter((item: CartItem) => item.articleId)
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

  async function syncItem(articleId: string, quantity: number) {
    if (!auth.isLoggedIn.value || !auth.isCustomer.value) return
    await customers.upsertSelfCartItem(articleId, quantity)
  }

  async function removeRemoteItem(articleId: string) {
    if (!auth.isLoggedIn.value || !auth.isCustomer.value) return
    await customers.deleteSelfCartItem(articleId)
  }

  async function add(article: CatalogArticle) {
    hydrate()
    const existing = items.value.find((item) => item.articleId === article.id)
    if (existing) {
      existing.quantity += 1
      await syncItem(existing.articleId, existing.quantity)
      return
    }

    const item = createCartItem(article)
    items.value = [...items.value, item]
    await syncItem(item.articleId, item.quantity)
  }

  async function increase(articleId: string) {
    hydrate()
    const existing = items.value.find((item) => item.articleId === articleId)
    if (!existing) return

    existing.quantity += 1
    await syncItem(existing.articleId, existing.quantity)
  }

  async function decrease(articleId: string) {
    hydrate()
    const existing = items.value.find((item) => item.articleId === articleId)
    if (!existing) return

    if (existing.quantity <= 1) {
      await remove(articleId)
      return
    }

    existing.quantity -= 1
    await syncItem(existing.articleId, existing.quantity)
  }

  async function setQuantity(articleId: string, quantity: number) {
    hydrate()
    const existing = items.value.find((item) => item.articleId === articleId)
    if (!existing) return

    const normalized = Math.max(1, Math.trunc(normalizeNumber(quantity, 1)))
    if (existing.quantity === normalized) return

    existing.quantity = normalized
    await syncItem(existing.articleId, existing.quantity)
  }

  async function remove(articleId: string) {
    hydrate()
    items.value = items.value.filter((item) => item.articleId !== articleId)
    await removeRemoteItem(articleId)
  }

  async function clearRemote() {
    if (!auth.isLoggedIn.value || !auth.isCustomer.value) return
    const remote = await customers.listSelfCartItems(1, 100)
    const uniqueIds = Array.from(
      new Set(remote.items.map((item) => item.articleId).filter(Boolean)),
    ) as string[]

    await Promise.all(uniqueIds.map((articleId) => removeRemoteItem(articleId)))
  }

  async function clear() {
    hydrate()
    await clearRemote().catch(() => null)
    items.value = []
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

  function hasArticle(articleId: string) {
    return items.value.some((item) => item.articleId === articleId)
  }

  function quantityFor(articleId: string) {
    return (
      items.value.find((item) => item.articleId === articleId)?.quantity ?? 0
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
    refreshFromRemote,
    hasArticle,
    quantityFor,
  }
}
