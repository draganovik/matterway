import { useAuthSessionStore } from "~/composables/stores/useAuthSessionStore"
import { useCustomersClient } from "~/composables/api/useCustomersClient"
import { usePaginationState } from "~/composables/workflows/pagination/usePaginationState"
import { useRequestState } from "~/composables/workflows/state/useRequestState"
import type {
  CustomerAddressResponse,
  CustomerResponse,
} from "~/types/customers"

type CustomerForm = {
  firstName: string
  lastName: string
  birthDate: string
  defaultAddressId: string
}

export function useUsersCustomersPage() {
  const auth = useAuthSessionStore()
  const api = useCustomersClient()

  const canEdit = computed(() =>
    auth.hasPermission("customers", ["operator", "manager"]),
  )
  const isLookupMode = computed(() => Boolean(filter.value.trim()))

  const listState = useRequestState({ empty: "No customers found." })
  const detailState = useRequestState()
  const saveState = useRequestState()
  const removeState = useRequestState()

  const customers = ref<CustomerResponse[]>([])
  const filter = ref("")
  const {
    pagination,
    resetTotals,
    setSinglePageTotal,
    applyMeta,
    changePage,
    changePageSize,
    searchWithPageReset,
    watchPagination,
  } = usePaginationState({ pageSize: 20 })

  const selectedId = ref<string | null>(null)
  const selectedCustomer = ref<CustomerResponse | null>(null)
  const createModalOpen = ref(false)
  const addressModalOpen = ref(false)

  const form = ref<CustomerForm>({
    firstName: "",
    lastName: "",
    birthDate: "",
    defaultAddressId: "",
  })

  function applyCustomerToForm(customer: CustomerResponse | null) {
    if (!customer) {
      form.value = {
        firstName: "",
        lastName: "",
        birthDate: "",
        defaultAddressId: "",
      }
      return
    }

    form.value = {
      firstName: customer.firstName || "",
      lastName: customer.lastName || "",
      birthDate: (customer.birthDate || "").split("T")[0] || "",
      defaultAddressId: customer.defaultAddressId || "",
    }
  }

  function clearSelection() {
    selectedId.value = null
    selectedCustomer.value = null
    addressModalOpen.value = false
    detailState.error = ""
    applyCustomerToForm(null)
  }

  function resetMessages() {
    saveState.error = ""
    saveState.success = ""
    removeState.error = ""
    removeState.success = ""
  }

  async function loadCustomers() {
    listState.loading = true
    listState.error = ""

    const lookupId = filter.value.trim()
    if (lookupId) {
      const result = await api.getCustomerById(lookupId)
      listState.loading = false

      if (!result.ok || !result.data) {
        listState.error = result.error || "Unable to load customer."
        customers.value = []
        setSinglePageTotal(0)
        clearSelection()
        return
      }

      customers.value = [result.data]
      setSinglePageTotal(1)

      if (selectedId.value && selectedId.value !== result.data.systemUserId) {
        clearSelection()
      }
      return
    }

    const result = await api.queryCustomers({
      page: pagination.page,
      pageSize: pagination.pageSize,
    })

    listState.loading = false

    if (!result.ok) {
      listState.error = result.error || "Unable to load customers."
      customers.value = []
      resetTotals()
      clearSelection()
      return
    }

    if (result.status === 204 || !result.data) {
      customers.value = []
      resetTotals()
      clearSelection()
      return
    }

    customers.value = result.data.data || []
    applyMeta(result.data.meta, customers.value.length)

    if (selectedId.value) {
      const match =
        customers.value.find(
          (item) => item.systemUserId === selectedId.value,
        ) || null
      if (!match) clearSelection()
    }
  }

  async function loadCustomer(customerId: string) {
    detailState.loading = true
    detailState.error = ""

    const result = await api.getCustomerById(customerId)
    detailState.loading = false

    if (!result.ok || !result.data) {
      detailState.error = result.error || "Unable to load customer details."
      selectedCustomer.value = null
      applyCustomerToForm(null)
      return
    }

    selectedCustomer.value = result.data
    applyCustomerToForm(result.data)

    customers.value = customers.value.map((item) =>
      item.systemUserId === result.data?.systemUserId ? result.data : item,
    )
  }

  function searchCustomers() {
    searchWithPageReset(
      loadCustomers,
      () => !filter.value.trim() && pagination.page !== 1,
    )
  }

  function selectCustomer(systemUserId: string) {
    selectedId.value = systemUserId
    addressModalOpen.value = false
    resetMessages()
    void loadCustomer(systemUserId)
  }

  const selectedCustomerName = computed(() => {
    const first = selectedCustomer.value?.firstName?.trim() || ""
    const last = selectedCustomer.value?.lastName?.trim() || ""
    const fullName = `${first} ${last}`.trim()
    return fullName || "Selected customer"
  })

  function revealAddress() {
    if (!selectedCustomer.value?.systemUserId) return
    addressModalOpen.value = true
  }

  function handleAddressSaved(address: CustomerAddressResponse) {
    if (!selectedCustomer.value) return

    const next = {
      ...selectedCustomer.value,
      defaultAddressId: address.id,
    }
    selectedCustomer.value = next
    form.value.defaultAddressId = address.id

    customers.value = customers.value.map((item) =>
      item.systemUserId === next.systemUserId
        ? { ...item, defaultAddressId: address.id }
        : item,
    )
  }

  async function saveCustomer() {
    resetMessages()

    if (!canEdit.value) return

    const customerId = selectedId.value
    const firstName = form.value.firstName.trim()
    const lastName = form.value.lastName.trim()
    const birthDate = form.value.birthDate.trim()
    const defaultAddressId = form.value.defaultAddressId.trim() || null

    if (!customerId) {
      saveState.error = "Select a customer to update."
      return
    }

    if (!firstName || !lastName || !birthDate) {
      saveState.error =
        "First name, last name, and birth date are required for updates."
      return
    }

    saveState.loading = true
    const result = await api.updateCustomer(customerId, {
      systemUserId: customerId,
      firstName,
      lastName,
      birthDate,
      defaultAddressId,
    })
    saveState.loading = false

    if (!result.ok) {
      saveState.error = result.error || "Unable to update customer."
      return
    }

    const updated: CustomerResponse = {
      systemUserId: result.data?.systemUserId || customerId,
      firstName: result.data?.firstName ?? firstName,
      lastName: result.data?.lastName ?? lastName,
      birthDate: result.data?.birthDate || birthDate,
      defaultAddressId: result.data?.defaultAddressId ?? defaultAddressId,
    }

    selectedId.value = updated.systemUserId
    selectedCustomer.value = updated
    applyCustomerToForm(updated)

    customers.value = customers.value.map((item) =>
      item.systemUserId === updated.systemUserId ? updated : item,
    )

    saveState.success = "Customer updated."
  }

  async function removeCustomer() {
    resetMessages()

    if (!canEdit.value) return

    const customerId = selectedId.value
    if (!customerId) {
      removeState.error = "Select a customer to delete."
      return
    }

    removeState.loading = true
    const result = await api.deleteCustomer(customerId)
    removeState.loading = false

    if (!result.ok) {
      removeState.error = result.error || "Unable to delete customer."
      return
    }

    customers.value = customers.value.filter(
      (item) => item.systemUserId !== customerId,
    )

    if (!isLookupMode.value) {
      pagination.totalCount = Math.max(0, pagination.totalCount - 1)
      if (customers.value.length === 0 && pagination.page > 1) {
        pagination.page -= 1
      } else {
        void loadCustomers()
      }
    } else {
      setSinglePageTotal(customers.value.length)
    }

    selectedId.value = null
    selectedCustomer.value = null
    applyCustomerToForm(null)

    removeState.success = result.data?.message || "Customer removed."
  }

  function handleCustomerCreated(customer: CustomerResponse) {
    resetMessages()

    if (isLookupMode.value) {
      filter.value = customer.systemUserId
      customers.value = [customer]
      setSinglePageTotal(1)
    } else {
      const exists = customers.value.some(
        (item) => item.systemUserId === customer.systemUserId,
      )
      customers.value = [
        customer,
        ...customers.value.filter(
          (item) => item.systemUserId !== customer.systemUserId,
        ),
      ]
      if (!exists) pagination.totalCount += 1
    }

    selectedId.value = customer.systemUserId
    selectedCustomer.value = customer
    applyCustomerToForm(customer)
  }

  watchPagination(loadCustomers, () => !filter.value.trim())

  onMounted(() => {
    void loadCustomers()
  })

  return {
    canEdit,
    listState,
    detailState,
    saveState,
    removeState,
    customers,
    filter,
    pagination,
    selectedId,
    selectedCustomer,
    createModalOpen,
    addressModalOpen,
    form,
    selectedCustomerName,
    changePage,
    changePageSize,
    searchCustomers,
    selectCustomer,
    revealAddress,
    handleAddressSaved,
    saveCustomer,
    removeCustomer,
    handleCustomerCreated,
  }
}
