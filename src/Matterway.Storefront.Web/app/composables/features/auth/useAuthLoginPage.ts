import { useAuthSessionStore } from "~/composables/stores/useAuthSessionStore"
import { useCartStore } from "~/composables/stores/useCartStore"
import { resolveNextRoute } from "~/lib/auth/navigation"

type LoginForm = {
  email: string
  password: string
}

const emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/

export function useAuthLoginPage() {
  const auth = useAuthSessionStore()
  const cart = useCartStore()
  const route = useRoute()
  const nuxtApp = useNuxtApp()

  const model = reactive<LoginForm>({
    email: "",
    password: "",
  })
  const loading = ref(false)
  const error = ref("")

  async function initialize() {
    await auth.initialize()
    if (auth.isLoggedIn.value && auth.isCustomer.value) {
      await nuxtApp.runWithContext(() => navigateTo(resolveNextRoute(route)))
    }
  }

  async function submit() {
    error.value = ""
    const email = model.email.trim()
    const password = model.password

    if (!email) {
      error.value = "Unesite imejl adresu."
      return
    }

    if (!emailPattern.test(email)) {
      error.value = "Imejl adresa nije u ispravnom formatu."
      return
    }

    if (!password.trim()) {
      error.value = "Unesite lozinku."
      return
    }

    loading.value = true
    try {
      await auth.login({
        email,
        password,
      })

      await cart.mergeGuestItemsIntoRemote().catch(() => null)
      await nuxtApp.runWithContext(() => navigateTo(resolveNextRoute(route)))
    } catch (err) {
      error.value = err instanceof Error ? err.message : "Prijava nije uspela."
    } finally {
      loading.value = false
    }
  }

  return {
    model,
    loading,
    error,
    initialize,
    submit,
  }
}
