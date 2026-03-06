import { useAuthSessionStore } from "~/composables/stores/useAuthSessionStore"
import { serviceSections } from "~/data/serviceRegistry"

type LoginForm = {
  email: string
  password: string
}

export function useAuthLoginPage() {
  const auth = useAuthSessionStore()

  const model = reactive<LoginForm>({
    email: "",
    password: "",
  })
  const error = ref("")
  const loading = ref(false)

  function getFirstRoute() {
    for (const service of serviceSections) {
      const feature = service.features.find((item) =>
        auth.hasPermission(item.service, item.allowed),
      )
      if (feature) return feature.route
    }
    return "/"
  }

  async function initialize() {
    await auth.initialize()
    if (auth.isLoggedIn.value) {
      await navigateTo(getFirstRoute())
    }
  }

  async function submit() {
    const email = model.email.trim()
    const password = model.password

    error.value = ""

    if (!email || !password.trim()) {
      error.value = "Email and password are required."
      return
    }

    loading.value = true
    try {
      await auth.login(email, password)
      await navigateTo(getFirstRoute())
    } catch (err) {
      error.value = err instanceof Error ? err.message : "Login failed."
    } finally {
      loading.value = false
    }
  }

  return {
    model,
    error,
    loading,
    initialize,
    submit,
  }
}
