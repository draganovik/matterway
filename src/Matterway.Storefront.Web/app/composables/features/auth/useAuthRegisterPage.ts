import { useAuthRegistrationWorkflow } from "~/composables/workflows/useAuthRegistrationWorkflow"
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

function getDefaultBirthDate() {
  return (
    new Date(new Date().setFullYear(new Date().getFullYear() - 18))
      .toISOString()
      .split("T")[0] || ""
  )
}

export function useAuthRegisterPage() {
  const registerCustomer = useAuthRegistrationWorkflow()
  const cart = useCartStore()
  const route = useRoute()

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

    if (model.password !== model.confirmPassword) {
      error.value = "Lozinke se ne podudaraju."
      return
    }

    loading.value = true
    try {
      await registerCustomer.registerAndSignIn({
        firstName: model.firstName,
        lastName: model.lastName,
        birthDate: model.birthDate,
        email: model.email,
        password: model.password,
      })

      await cart.clear()
      await navigateTo(resolveNextRoute(route))
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
