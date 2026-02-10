<script setup lang="ts">
import { useCustomersApi } from '~/composables/useCustomersApi'
import type { CustomerResponse } from '~/types/customers'
import { useRequestState } from '~/composables/useRequestState'
import { useAuthSession } from '~/composables/useAuthSession'
import { parseNumberOr } from '~/utils/numbers'

type CustomerForm = {
  firstName: string
  lastName: string
  birthDate: string
  defaultAddressId: string
}

const auth = useAuthSession()
const api = useCustomersApi()

const canEdit = computed(() => auth.hasPermission('customers', 'operator'))
const isLookupMode = computed(() => Boolean(filter.value.trim()))

const listState = useRequestState({ empty: 'No customers found.' })
const detailState = useRequestState()
const saveState = useRequestState()
const removeState = useRequestState()

const customers = ref<CustomerResponse[]>([])
const filter = ref('')
const pagination = reactive({
  page: 1,
  pageSize: 20,
  totalCount: 0,
  totalPages: 1
})

const selectedId = ref<string | null>(null)
const selectedCustomer = ref<CustomerResponse | null>(null)
const createModalOpen = ref(false)

const form = ref<CustomerForm>({
  firstName: '',
  lastName: '',
  birthDate: '',
  defaultAddressId: ''
})

function applyCustomerToForm(customer: CustomerResponse | null) {
  if (!customer) {
    form.value = {
      firstName: '',
      lastName: '',
      birthDate: '',
      defaultAddressId: ''
    }
    return
  }

  form.value = {
    firstName: customer.firstName || '',
    lastName: customer.lastName || '',
    birthDate: (customer.birthDate || '').split('T')[0] || '',
    defaultAddressId: customer.defaultAddressId || ''
  }
}

function clearSelection() {
  selectedId.value = null
  selectedCustomer.value = null
  detailState.error = ''
  applyCustomerToForm(null)
}

function resetMessages() {
  saveState.error = ''
  saveState.success = ''
  removeState.error = ''
  removeState.success = ''
}

async function loadCustomers() {
  listState.loading = true
  listState.error = ''

  const lookupId = filter.value.trim()
  if (lookupId) {
    const result = await api.getCustomerById(lookupId)
    listState.loading = false

    if (!result.ok || !result.data) {
      listState.error = result.error || 'Unable to load customer.'
      customers.value = []
      pagination.page = 1
      pagination.totalCount = 0
      pagination.totalPages = 1
      clearSelection()
      return
    }

    customers.value = [result.data]
    pagination.page = 1
    pagination.totalCount = 1
    pagination.totalPages = 1

    if (selectedId.value && selectedId.value !== result.data.systemUserId) {
      clearSelection()
    }
    return
  }

  const result = await api.queryCustomers({
    page: pagination.page,
    pageSize: pagination.pageSize
  })

  listState.loading = false

  if (!result.ok) {
    listState.error = result.error || 'Unable to load customers.'
    customers.value = []
    pagination.totalCount = 0
    pagination.totalPages = 1
    clearSelection()
    return
  }

  if (result.status === 204 || !result.data) {
    customers.value = []
    pagination.totalCount = 0
    pagination.totalPages = 1
    clearSelection()
    return
  }

  customers.value = result.data.data || []
  pagination.totalCount = parseNumberOr(
    result.data.meta?.totalCount,
    customers.value.length
  )
  pagination.totalPages = Math.max(
    1,
    parseNumberOr(result.data.meta?.totalPages, 1)
  )
  pagination.page = Math.max(
    1,
    parseNumberOr(result.data.meta?.currentPage, pagination.page)
  )
  pagination.pageSize = Math.max(
    1,
    parseNumberOr(result.data.meta?.pageSize, pagination.pageSize)
  )

  if (selectedId.value) {
    const match =
      customers.value.find((item) => item.systemUserId === selectedId.value) ||
      null
    if (!match) clearSelection()
  }
}

async function loadCustomer(customerId: string) {
  detailState.loading = true
  detailState.error = ''

  const result = await api.getCustomerById(customerId)
  detailState.loading = false

  if (!result.ok || !result.data) {
    detailState.error = result.error || 'Unable to load customer details.'
    selectedCustomer.value = null
    applyCustomerToForm(null)
    return
  }

  selectedCustomer.value = result.data
  applyCustomerToForm(result.data)

  customers.value = customers.value.map((item) =>
    item.systemUserId === result.data?.systemUserId ? result.data : item
  )
}

function searchCustomers() {
  if (!filter.value.trim() && pagination.page !== 1) {
    pagination.page = 1
    return
  }
  void loadCustomers()
}

function selectCustomer(systemUserId: string) {
  selectedId.value = systemUserId
  resetMessages()
  void loadCustomer(systemUserId)
}

function changePage(page: number) {
  pagination.page = page
}

function changePageSize(pageSize: number) {
  pagination.pageSize = pageSize
  pagination.page = 1
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
    saveState.error = 'Select a customer to update.'
    return
  }

  if (!firstName || !lastName || !birthDate) {
    saveState.error =
      'First name, last name, and birth date are required for updates.'
    return
  }

  saveState.loading = true
  const result = await api.updateCustomer(customerId, {
    systemUserId: customerId,
    firstName,
    lastName,
    birthDate,
    defaultAddressId
  })
  saveState.loading = false

  if (!result.ok) {
    saveState.error = result.error || 'Unable to update customer.'
    return
  }

  const updated: CustomerResponse = {
    systemUserId: result.data?.systemUserId || customerId,
    firstName: result.data?.firstName ?? firstName,
    lastName: result.data?.lastName ?? lastName,
    birthDate: result.data?.birthDate || birthDate,
    defaultAddressId: result.data?.defaultAddressId ?? defaultAddressId
  }

  selectedId.value = updated.systemUserId
  selectedCustomer.value = updated
  applyCustomerToForm(updated)

  customers.value = customers.value.map((item) =>
    item.systemUserId === updated.systemUserId ? updated : item
  )

  saveState.success = 'Customer updated.'
}

async function removeCustomer() {
  resetMessages()

  if (!canEdit.value) return

  const customerId = selectedId.value
  if (!customerId) {
    removeState.error = 'Select a customer to delete.'
    return
  }

  removeState.loading = true
  const result = await api.deleteCustomer(customerId)
  removeState.loading = false

  if (!result.ok) {
    removeState.error = result.error || 'Unable to delete customer.'
    return
  }

  customers.value = customers.value.filter(
    (item) => item.systemUserId !== customerId
  )

  if (!isLookupMode.value) {
    pagination.totalCount = Math.max(0, pagination.totalCount - 1)
    if (customers.value.length === 0 && pagination.page > 1) {
      pagination.page -= 1
    } else {
      void loadCustomers()
    }
  } else {
    pagination.totalCount = customers.value.length
    pagination.totalPages = 1
    pagination.page = 1
  }

  selectedId.value = null
  selectedCustomer.value = null
  applyCustomerToForm(null)

  removeState.success = result.data?.message || 'Customer removed.'
}

function handleCustomerCreated(customer: CustomerResponse) {
  resetMessages()

  if (isLookupMode.value) {
    filter.value = customer.systemUserId
    customers.value = [customer]
    pagination.page = 1
    pagination.totalCount = 1
    pagination.totalPages = 1
  } else {
    const exists = customers.value.some(
      (item) => item.systemUserId === customer.systemUserId
    )
    customers.value = [
      customer,
      ...customers.value.filter(
        (item) => item.systemUserId !== customer.systemUserId
      )
    ]
    if (!exists) pagination.totalCount += 1
  }

  selectedId.value = customer.systemUserId
  selectedCustomer.value = customer
  applyCustomerToForm(customer)
}

watch([() => pagination.page, () => pagination.pageSize], () => {
  if (filter.value.trim()) return
  void loadCustomers()
})

onMounted(() => {
  void loadCustomers()
})
</script>

<template>
  <div class="flex h-full min-h-0 flex-col gap-4 overflow-hidden">
    <div class="flex shrink-0 flex-wrap items-center justify-between gap-3">
      <div>
        <h2 class="text-foreground text-base font-semibold">Manage Customers</h2>
        <p class="text-muted text-sm">
          Browse customer profiles, edit selected records, and remove invalid
          entries.
        </p>
      </div>

      <UButton color="primary" :disabled="!canEdit" @click="createModalOpen = true">
        Create New
      </UButton>
    </div>

    <EntitiesSplitView
      class="min-h-0 flex-1"
      list-class="overflow-y-auto"
      detail-class="overflow-y-auto"
    >
      <template #list>
        <EntitiesListPanel
          title="Customers"
          description="Use pagination or provide an exact System User ID to fetch one customer."
          :items="customers"
          item-key="systemUserId"
          item-title-key="firstName"
          item-subtitle-key="systemUserId"
          :selected-id="selectedId"
          :filter="filter"
          filter-input-type="input"
          filter-placeholder="Optional exact System User ID (GUID)."
          :loading="listState.loading"
          :error="listState.error"
          :empty-message="listState.empty"
          :page="pagination.page"
          :page-size="pagination.pageSize"
          :total-count="pagination.totalCount"
          :total-pages="pagination.totalPages"
          @update:filter="(value) => (filter = value)"
          @search="searchCustomers"
          @update:page="changePage"
          @update:page-size="changePageSize"
          @select="selectCustomer"
        >
          <template #item="{ item }">
            <CustomersCustomersListItem :item="item" />
          </template>
        </EntitiesListPanel>
      </template>

      <template #detail>
        <div class="space-y-4">
          <div class="space-y-1">
            <h3 class="text-foreground text-base font-semibold">
              {{ selectedCustomer ? 'Edit Customer' : 'Customer Editor' }}
            </h3>
            <p class="text-muted text-sm">
              {{
                canEdit
                  ? 'Operator permission is required for create, update, and delete.'
                  : 'Read-only mode: operator permission required for changes.'
              }}
            </p>
          </div>

          <StatusMessages
            v-if="detailState.loading || detailState.error"
            :loading="detailState.loading ? 'Loading customer.' : false"
            :error="detailState.error"
          />

          <EntitiesEmptyState
            v-else-if="!selectedCustomer"
            title="Nothing selected"
            description="Select a customer from the list to start editing."
          />

          <div v-else class="grid gap-4">
            <div class="text-muted text-sm">
              System User ID: {{ selectedCustomer.systemUserId }}
            </div>

            <CustomersCustomersBaseForm
              v-model="form"
              :disabled="!canEdit"
              :show-system-user-id="false"
            />

            <div class="flex flex-wrap items-center gap-3">
              <UButton
                color="primary"
                :loading="saveState.loading"
                :disabled="!canEdit"
                @click="saveCustomer"
              >
                Update Customer
              </UButton>

              <UButton
                color="error"
                variant="ghost"
                :loading="removeState.loading"
                :disabled="!canEdit"
                @click="removeCustomer"
              >
                Delete Customer
              </UButton>
            </div>

            <StatusMessages
              :error="saveState.error || removeState.error"
              :success="saveState.success || removeState.success"
            />
          </div>
        </div>
      </template>
    </EntitiesSplitView>
  </div>

  <CustomersCustomersCreateModal
    v-model:open="createModalOpen"
    :can-edit="canEdit"
    @created="handleCustomerCreated"
  />
</template>
