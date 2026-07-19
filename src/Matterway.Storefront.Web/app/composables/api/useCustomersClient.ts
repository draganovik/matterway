import { useApiClient } from "~/composables/api/useApiClient"
import type {
  CustomerAddressResponse,
  PutSelfAddressRequest,
  RegisterPayload,
  SelfProfileResponse,
  SelfProfileUpdateRequest,
} from "~/types/customers"

type RemoteCartItem = {
  articleName?: string
  quantity?: number
  unitPrice?: number
  articleCode?: string
}

export function useCustomersClient() {
  const api = useApiClient()

  async function registerCustomer(payload: RegisterPayload) {
    return api.request("customers", "public/register", {
      method: "POST",
      body: payload,
    })
  }

  async function getSelfAddress() {
    return api.request<CustomerAddressResponse>("customers", "self/address", {
      method: "GET",
    })
  }

  async function putSelfAddress(payload: PutSelfAddressRequest) {
    return api.request<CustomerAddressResponse>("customers", "self/address", {
      method: "PUT",
      body: payload,
    })
  }

  async function getSelfProfile() {
    return api.request<SelfProfileResponse>("customers", "self/profile", {
      method: "GET",
    })
  }

  async function updateSelfProfile(payload: SelfProfileUpdateRequest) {
    return api.request<SelfProfileResponse>("customers", "self/profile", {
      method: "PATCH",
      body: payload,
    })
  }

  async function listSelfCartItems(page = 1, pageSize = 100) {
    const response = await api.request<{ data?: RemoteCartItem[] }>(
      "customers",
      `self/cart/items?page=${page}&pageSize=${pageSize}`,
      { method: "GET" },
    )
    if (!response.ok) {
      return {
        items: [] as RemoteCartItem[],
        error: response.error,
      }
    }

    return {
      items: response.data?.data ?? [],
    }
  }

  async function upsertSelfCartItem(articleCode: string, quantity: number) {
    return api.request(
      "customers",
      `self/cart/items/${encodeURIComponent(articleCode)}`,
      {
        method: "PUT",
        body: { quantity },
      },
    )
  }

  async function deleteSelfCartItem(articleCode: string) {
    return api.request(
      "customers",
      `self/cart/items/${encodeURIComponent(articleCode)}`,
      {
        method: "DELETE",
      },
    )
  }

  return {
    registerCustomer,
    getSelfAddress,
    putSelfAddress,
    getSelfProfile,
    updateSelfProfile,
    listSelfCartItems,
    upsertSelfCartItem,
    deleteSelfCartItem,
  }
}
