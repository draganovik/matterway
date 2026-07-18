import { useCustomersClient } from "~/composables/api/useCustomersClient"
import { useAuthSessionStore } from "~/composables/stores/useAuthSessionStore"
import { useCartStore } from "~/composables/stores/useCartStore"
import { resolveNextRoute } from "~/lib/auth/navigation"

type RegisterForm = {
  firstName: string
  lastName: string
  birthDate: string
  email: string
  password: string
  confirmPassword: string
}

const emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/

function getDefaultBirthDate() {
  const date = new Date()
  date.setFullYear(date.getFullYear() - 18)
  const year = date.getFullYear()
  const month = String(date.getMonth() + 1).padStart(2, "0")
  const day = String(date.getDate()).padStart(2, "0")
  return `${year}-${month}-${day}`
}

export function useAuthRegisterPage() {
  const customersApi = useCustomersClient()
  const auth = useAuthSessionStore()
  const cart = useCartStore()
  const route = useRoute()
  const nuxtApp = useNuxtApp()

  const model = reactive<RegisterForm>({
    firstName: "",
    lastName: "",
    birthDate: getDefaultBirthDate(),
    email: "",
    password: "",
    confirmPassword: "",
  })
  const loading = ref(false)
  const error = ref("")

  async function submit() {
    error.value = ""
    const firstName = model.firstName.trim()
    const lastName = model.lastName.trim()
    const birthDate = model.birthDate.trim()
    const email = model.email.trim()
    const password = model.password
    const confirmPassword = model.confirmPassword

    if (!firstName) {
      error.value = "Unesite ime."
      return
    }

    if (!lastName) {
      error.value = "Unesite prezime."
      return
    }

    if (!birthDate) {
      error.value = "Unesite datum rođenja."
      return
    }

    if (!email) {
      error.value = "Unesite imejl adresu."
      return
    }

    if (!emailPattern.test(email)) {
      error.value = "Imejl adresa nije u ispravnom formatu."
      return
    }

    if (!password) {
      error.value = "Unesite lozinku."
      return
    }

    if (password.length < 6) {
      error.value = "Lozinka mora da ima najmanje 6 karaktera."
      return
    }

    if (password !== confirmPassword) {
      error.value = "Lozinke se ne podudaraju."
      return
    }

    loading.value = true
    try {
      const registration = await customersApi.registerCustomer({
        firstName,
        lastName,
        birthDate,
        email,
        password,
      })
      if (!registration.ok) {
        throw new Error(registration.error || "Registracija nije uspela.")
      }

      await auth.login({ email, password })

      await cart.mergeGuestItemsIntoRemote().catch(() => null)
      await nuxtApp.runWithContext(() => navigateTo(resolveNextRoute(route)))
    } catch (err) {
      error.value =
        err instanceof Error ? err.message : "Registracija nije uspela."
    } finally {
      loading.value = false
    }
  }

  return {
    model,
    loading,
    error,
    submit,
  }
}
