import { useAuthSessionStore } from "~/composables/stores/useAuthSessionStore"
import type { ApiResult, ApiService } from "~/types/common/api"
import type { RuntimeConfig } from "nuxt/schema"

const validEndpointKinds = new Set(["self", "admin", "public", "system"])
const fieldLabels: Record<string, string> = {
  Email: "Imejl",
  Password: "Lozinka",
  FirstName: "Ime",
  LastName: "Prezime",
  BirthDate: "Datum rođenja",
  Country: "Država",
  City: "Grad",
  ZipCode: "Poštanski broj",
  AddressLine1: "Ulica i broj",
  AddressLine2: "Stan, sprat ili dodatak",
  ContactPhone: "Kontakt telefon",
  Quantity: "Količina",
  SystemUserId: "ID korisnika",
  CustomerId: "ID kupca",
}

function getBaseUrl(service: ApiService, config: RuntimeConfig) {
  if (service === "catalog") return config.public.catalogApiBaseUrl
  if (service === "customers") return config.public.customersApiBaseUrl
  if (service === "identity") return config.public.identityApiBaseUrl
  if (service === "sales") return config.public.salesApiBaseUrl
  return null
}

function getValidationErrors(
  payload: unknown,
): Record<string, string[]> | undefined {
  if (!payload || typeof payload !== "object") return undefined
  const payloadWithErrors = payload as { errors?: unknown }
  if (!payloadWithErrors.errors || typeof payloadWithErrors.errors !== "object")
    return undefined
  return payloadWithErrors.errors as Record<string, string[]>
}

function formatValidationErrors(errors?: Record<string, string[]>) {
  if (!errors) return ""
  return Object.entries(errors)
    .map(
      ([field, messages]) =>
        `${fieldLabels[field] || field}: ${messages
          .map((message) => translateMessage(message))
          .join(" ")}`,
    )
    .join(" | ")
}

function translateMessage(message: string) {
  const normalized = message.trim()

  if (!normalized) return normalized
  if (normalized === "One or more validation errors occurred.") {
    return "Proverite unesene podatke."
  }
  if (normalized === "Registration failed") return "Registracija nije uspela."
  if (normalized === "Could not create identity user.") {
    return "Nalog trenutno nije moguće kreirati."
  }
  if (normalized === "Could not clean up the identity user.") {
    return "Registracija trenutno nije uspela. Pokušajte ponovo."
  }
  if (normalized === "Could not create customer profile.") {
    return "Profil kupca trenutno nije moguće kreirati."
  }
  if (normalized === "Bad Request") return "Neispravan zahtev."
  if (normalized === "Cannot upsert address.") {
    return "Adresu trenutno nije moguće sačuvati."
  }
  if (normalized === "Cannot update customer default address.") {
    return "Podrazumevanu adresu nije moguće ažurirati."
  }
  if (normalized === "Unable to retrieve article.") {
    return "Artikal trenutno nije moguće učitati."
  }
  if (normalized === "Article price is unavailable.") {
    return "Cena artikla trenutno nije dostupna."
  }

  return normalized
}

function getErrorMessage(payload: unknown) {
  if (typeof payload === "string") {
    const message = payload.trim()
    return message || null
  }

  if (!payload || typeof payload !== "object") return null

  const candidate = payload as {
    title?: unknown
    detail?: unknown
    message?: unknown
  }

  if (typeof candidate.detail === "string" && candidate.detail.trim()) {
    return candidate.detail.trim()
  }

  if (typeof candidate.title === "string" && candidate.title.trim()) {
    return candidate.title.trim()
  }

  if (typeof candidate.message === "string" && candidate.message.trim()) {
    return candidate.message.trim()
  }

  return null
}

export function useApiClient() {
  const auth = useAuthSessionStore()
  const config = useRuntimeConfig()

  async function request<T>(
    service: ApiService,
    path: string,
    options: RequestInit = {},
    allowUnauthorized = false,
  ): Promise<ApiResult<T>> {
    if (!auth.isInitialized.value) {
      await auth.initialize()
    }

    const baseUrl = getBaseUrl(service, config)
    if (!baseUrl) {
      return {
        ok: false,
        status: 0,
        error: "Usluga trenutno nije dostupna. Pokušajte ponovo kasnije.",
      }
    }

    const normalizedPath = path.replace(/^\/+/, "")
    const [endpointKind, ...resourcePath] = normalizedPath.split("/")
    if (!endpointKind || !validEndpointKinds.has(endpointKind)) {
      return {
        ok: false,
        status: 0,
        error: "Zahtev nije ispravan.",
      }
    }
    if (resourcePath.length === 0) {
      return {
        ok: false,
        status: 0,
        error: "Zahtev nije ispravan.",
      }
    }
    const url = `${baseUrl}/api/${endpointKind}/v1/${resourcePath.join("/")}`
    const headers = new Headers(options.headers || {})
    if (!headers.has("Accept")) headers.set("Accept", "application/json")
    if (!headers.has("Content-Type") && !(options.body instanceof FormData)) {
      headers.set("Content-Type", "application/json")
    }

    const accessToken = auth.getAccessToken()
    if (accessToken) headers.set("Authorization", accessToken)

    const runFetch = async () =>
      fetch(url, {
        ...options,
        headers,
      })

    let response = await runFetch()

    if (response.status === 401 && !allowUnauthorized) {
      await auth.refreshTokens()
      const refreshedToken = auth.getAccessToken()
      if (refreshedToken) {
        headers.set("Authorization", refreshedToken)
        response = await runFetch()
      }
    }

    if (response.status === 204) {
      return { ok: true, status: response.status }
    }

    const contentType = response.headers.get("content-type") || ""
    const isJson = contentType.includes("json")
    const payload = isJson
      ? await response.json().catch(() => null)
      : await response.text().catch(() => null)

    if (response.ok) {
      return { ok: true, status: response.status, data: payload as T }
    }

    const validationErrors = getValidationErrors(payload)
    const baseError = translateMessage(
      getErrorMessage(payload) || "Zahtev nije uspeo.",
    )
    const validationMessage = formatValidationErrors(validationErrors)
    return {
      ok: false,
      status: response.status,
      error: validationMessage
        ? `${baseError} ${validationMessage}`
        : baseError,
      validationErrors,
    }
  }

  return { request }
}
