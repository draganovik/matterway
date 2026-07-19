import { useAuthSessionStore } from "~/composables/stores/useAuthSessionStore"
import { useIdentityClient } from "~/composables/api/useIdentityClient"
import { usePaginationState } from "~/composables/workflows/pagination/usePaginationState"
import { useRequestState } from "~/composables/workflows/state/useRequestState"
import type { IdentityRole, SystemUserResponse } from "~/types/identity"

type SystemUserForm = {
  email: string
  password: string
}

type RoleFilter = "all" | "customers" | "employees"

export function useUsersAccountsPage() {
  const auth = useAuthSessionStore()
  const api = useIdentityClient()

  const canOperate = computed(() =>
    auth.hasPermission("identity", ["operator", "manager"]),
  )
  const canManage = computed(() => auth.hasPermission("identity", ["manager"]))
  const isLookupMode = computed(() => Boolean(filter.value.trim()))

  const listState = useRequestState("Nema naloga.", true)
  const detailState = useRequestState()
  const saveState = useRequestState()
  const removeState = useRequestState()
  const deleteConfirmOpen = ref(false)

  const systemUsers = ref<SystemUserResponse[]>([])
  const filter = ref("")
  const roleFilter = ref<RoleFilter>("all")
  const {
    pagination,
    resetTotals,
    setSinglePageTotal,
    applyMeta,
    changePage,
    changePageSize,
    searchWithPageReset,
    watchPagination,
  } = usePaginationState()

  const roleFilterOptions = [
    { label: "Svi", value: "all" as const },
    { label: "Kupci", value: "customers" as const },
    { label: "Zaposleni", value: "employees" as const },
  ]

  const selectedId = ref<string | null>(null)
  const selectedSystemUser = ref<SystemUserResponse | null>(null)
  const rolesModalOpen = ref(false)
  const createModalOpen = ref(false)

  const form = ref<SystemUserForm>({
    email: "",
    password: "",
  })

  function applySystemUserToForm(systemUser: SystemUserResponse | null) {
    if (!systemUser) {
      form.value = {
        email: "",
        password: "",
      }
      return
    }

    form.value = {
      email: systemUser.email || "",
      password: "",
    }
  }

  function clearSelection() {
    selectedId.value = null
    selectedSystemUser.value = null
    rolesModalOpen.value = false
    detailState.error = ""
    applySystemUserToForm(null)
  }

  function resetMessages() {
    saveState.error = ""
    saveState.success = ""
    removeState.error = ""
    removeState.success = ""
  }

  function requestRemoveSystemUser() {
    resetMessages()

    if (!canManage.value) return

    const systemUserId = selectedId.value
    if (!systemUserId) {
      removeState.error = "Izaberite nalog za brisanje."
      return
    }

    deleteConfirmOpen.value = true
  }

  async function loadSystemUsers() {
    listState.loading = true
    listState.error = ""

    const lookupId = filter.value.trim()
    if (lookupId) {
      const result = await api.getSystemUserById(lookupId)
      listState.loading = false

      if (!result.ok || !result.data) {
        listState.error = result.error || "Učitavanje naloga nije uspelo."
        systemUsers.value = []
        setSinglePageTotal(0)
        clearSelection()
        return
      }

      systemUsers.value = [result.data]
      setSinglePageTotal(1)

      if (selectedId.value && selectedId.value !== result.data.id) {
        clearSelection()
      }
      return
    }

    const result = await api.querySystemUsers({
      page: pagination.page,
      pageSize: pagination.pageSize,
      role: resolveRoleFilter(),
    })

    listState.loading = false

    if (!result.ok) {
      listState.error = result.error || "Učitavanje naloga nije uspelo."
      systemUsers.value = []
      resetTotals()
      clearSelection()
      return
    }

    if (result.status === 204 || !result.data) {
      systemUsers.value = []
      resetTotals()
      clearSelection()
      return
    }

    systemUsers.value = result.data.data || []
    applyMeta(result.data.meta, systemUsers.value.length)

    if (selectedId.value) {
      const match =
        systemUsers.value.find((item) => item.id === selectedId.value) || null
      if (!match) clearSelection()
    }
  }

  async function loadSystemUser(systemUserId: string) {
    detailState.loading = true
    detailState.error = ""

    const result = await api.getSystemUserById(systemUserId)
    detailState.loading = false

    if (!result.ok || !result.data) {
      detailState.error =
        result.error || "Učitavanje detalja naloga nije uspelo."
      selectedSystemUser.value = null
      applySystemUserToForm(null)
      return
    }

    selectedSystemUser.value = result.data
    applySystemUserToForm(result.data)

    systemUsers.value = systemUsers.value.map((item) =>
      item.id === result.data?.id ? result.data : item,
    )
  }

  function searchSystemUsers() {
    searchWithPageReset(
      loadSystemUsers,
      () => !filter.value.trim() && pagination.page !== 1,
    )
  }

  function selectSystemUser(systemUserId: string) {
    selectedId.value = systemUserId
    rolesModalOpen.value = false
    resetMessages()
    void loadSystemUser(systemUserId)
  }

  const selectedUserLabel = computed(() => {
    if (!selectedSystemUser.value) return "Izabrani nalog"
    return selectedSystemUser.value.email?.trim() || selectedSystemUser.value.id
  })

  function revealRoles() {
    if (!selectedSystemUser.value?.id) return
    rolesModalOpen.value = true
  }

  function resolveRoleFilter(): IdentityRole | undefined {
    if (roleFilter.value === "customers") return "Customer"
    if (roleFilter.value === "employees") return "Employee"
    return undefined
  }

  function changeRoleFilter(value: RoleFilter) {
    roleFilter.value = value
    searchWithPageReset(loadSystemUsers)
  }

  function handleEmployeeCreated(systemUser: SystemUserResponse) {
    resetMessages()

    if (isLookupMode.value || roleFilter.value === "customers") {
      filter.value = systemUser.id
      systemUsers.value = [systemUser]
      setSinglePageTotal(1)
    } else {
      const exists = systemUsers.value.some((item) => item.id === systemUser.id)
      if (!exists) {
        systemUsers.value = [systemUser, ...systemUsers.value]
        pagination.totalCount += 1
      }
    }

    selectedId.value = systemUser.id
    selectedSystemUser.value = systemUser
    applySystemUserToForm(systemUser)
    detailState.error = ""
    rolesModalOpen.value = false
  }

  async function saveSystemUser() {
    resetMessages()

    if (!canOperate.value) return

    const systemUserId = selectedId.value
    const email = form.value.email.trim()
    const password = form.value.password.trim()

    if (!systemUserId) {
      saveState.error = "Izaberite nalog za ažuriranje."
      return
    }

    if (!email && !password) {
      saveState.error = "Unesite imejl i/ili lozinku za ažuriranje."
      return
    }

    saveState.loading = true
    const result = await api.updateSystemUser(systemUserId, {
      email: email || undefined,
      password: password || undefined,
    })
    saveState.loading = false

    if (!result.ok || !result.data) {
      saveState.error = result.error || "Ažuriranje naloga nije uspelo."
      return
    }

    const updated = result.data

    selectedId.value = updated.id
    selectedSystemUser.value = updated
    applySystemUserToForm(updated)

    systemUsers.value = systemUsers.value.map((item) =>
      item.id === updated.id ? updated : item,
    )

    saveState.success = "Nalog je uspešno ažuriran."
  }

  async function removeSystemUser() {
    resetMessages()

    if (!canManage.value) return

    const systemUserId = selectedId.value
    if (!systemUserId) {
      removeState.error = "Izaberite nalog za brisanje."
      return
    }

    removeState.loading = true
    const result = await api.deleteSystemUser(systemUserId)
    removeState.loading = false

    if (!result.ok) {
      removeState.error = result.error || "Brisanje naloga nije uspelo."
      return
    }

    systemUsers.value = systemUsers.value.filter(
      (item) => item.id !== systemUserId,
    )

    if (!isLookupMode.value) {
      pagination.totalCount = Math.max(0, pagination.totalCount - 1)
      if (systemUsers.value.length === 0 && pagination.page > 1) {
        pagination.page -= 1
      } else {
        void loadSystemUsers()
      }
    } else {
      setSinglePageTotal(systemUsers.value.length)
    }

    selectedId.value = null
    selectedSystemUser.value = null
    applySystemUserToForm(null)
    deleteConfirmOpen.value = false

    removeState.success = "Nalog je uspešno obrisan."
  }

  watchPagination(loadSystemUsers, () => !filter.value.trim())

  onMounted(() => {
    void loadSystemUsers()
  })

  return {
    canOperate,
    canManage,
    listState,
    detailState,
    saveState,
    removeState,
    systemUsers,
    filter,
    roleFilter,
    roleFilterOptions,
    pagination,
    selectedId,
    selectedSystemUser,
    deleteConfirmOpen,
    rolesModalOpen,
    createModalOpen,
    form,
    selectedUserLabel,
    changePage,
    changePageSize,
    searchSystemUsers,
    selectSystemUser,
    revealRoles,
    changeRoleFilter,
    handleEmployeeCreated,
    saveSystemUser,
    removeSystemUser,
    requestRemoveSystemUser,
  }
}
