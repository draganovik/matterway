<script setup lang="ts">
import { useApiClient } from '~/composables/useApiClient'
import { useFeatureTabs } from '~/composables/useFeatureTabs'

const api = useApiClient()
const { active } = useFeatureTabs()

const queryForm = reactive({
  page: 1,
  pageSize: 10
})
const queryState = reactive({ loading: false, error: '', empty: '' })
const queryResults = ref<any[]>([])

const lookupForm = reactive({
  customerId: ''
})
const lookupState = reactive({ loading: false, error: '' })
const lookupResult = ref<any | null>(null)

const createForm = reactive({
  systemUserId: '',
  firstName: '',
  lastName: '',
  birthDate: '',
  defaultAddressId: ''
})
const createState = reactive({ loading: false, error: '', success: '' })

const updateForm = reactive({
  systemUserId: '',
  firstName: '',
  lastName: '',
  birthDate: '',
  defaultAddressId: ''
})
const updateState = reactive({ loading: false, error: '', success: '' })

const deleteForm = reactive({
  systemUserId: ''
})
const deleteState = reactive({ loading: false, error: '', success: '' })

async function queryCustomers() {
  queryState.loading = true
  queryState.error = ''
  queryState.empty = ''
  queryResults.value = []
  const params = new URLSearchParams({
    page: queryForm.page.toString(),
    pageSize: queryForm.pageSize.toString()
  })
  const result = await api.request<any>('customers', `admin/customers?${params.toString()}`)
  queryState.loading = false
  if (!result.ok) {
    queryState.error = result.error || 'Failed to query customers.'
    return
  }
  queryResults.value = result.data?.data ?? []
  if (!queryResults.value.length) queryState.empty = 'No customers found.'
}

async function lookupCustomer() {
  lookupState.loading = true
  lookupState.error = ''
  lookupResult.value = null
  const result = await api.request<any>('customers', `admin/customers/${lookupForm.customerId}`)
  lookupState.loading = false
  if (!result.ok) {
    lookupState.error = result.error || 'Customer not found.'
    return
  }
  lookupResult.value = result.data
}

async function createCustomer() {
  createState.loading = true
  createState.error = ''
  createState.success = ''
  const result = await api.request<any>('customers', 'admin/customers', {
    method: 'POST',
    body: JSON.stringify({
      systemUserId: createForm.systemUserId,
      firstName: createForm.firstName,
      lastName: createForm.lastName,
      birthDate: createForm.birthDate,
      defaultAddressId: createForm.defaultAddressId || null
    })
  })
  createState.loading = false
  if (!result.ok) {
    createState.error = result.error || 'Failed to create customer.'
    return
  }
  createState.success = 'Customer created.'
}

async function updateCustomer() {
  updateState.loading = true
  updateState.error = ''
  updateState.success = ''
  const result = await api.request<any>(
    'customers',
    `admin/customers/${updateForm.systemUserId}`,
    {
      method: 'PATCH',
      body: JSON.stringify({
        systemUserId: updateForm.systemUserId,
        firstName: updateForm.firstName,
        lastName: updateForm.lastName,
        birthDate: updateForm.birthDate,
        defaultAddressId: updateForm.defaultAddressId || null
      })
    }
  )
  updateState.loading = false
  if (!result.ok) {
    updateState.error = result.error || 'Failed to update customer.'
    return
  }
  updateState.success = 'Customer updated.'
}

async function deleteCustomer() {
  deleteState.loading = true
  deleteState.error = ''
  deleteState.success = ''
  const result = await api.request<any>(
    'customers',
    `admin/customers/${deleteForm.systemUserId}`,
    { method: 'DELETE' }
  )
  deleteState.loading = false
  if (!result.ok) {
    deleteState.error = result.error || 'Failed to delete customer.'
    return
  }
  deleteState.success = 'Customer deleted.'
}
</script>

<template>
  <FeatureShell>
    <div
      v-if="active === 'query'"
      class="space-y-6"
    >
      <UCard class="border border-default">
        <template #header>
          <h2 class="text-lg font-semibold">
            Query customers
          </h2>
        </template>
        <div class="grid gap-4 md:grid-cols-[1fr_1fr_auto]">
          <UFormField label="Page">
            <UInput
              v-model.number="queryForm.page"
              type="number"
              min="1"
            />
          </UFormField>
          <UFormField label="Page Size">
            <UInput
              v-model.number="queryForm.pageSize"
              type="number"
              min="1"
            />
          </UFormField>
          <UButton
            color="primary"
            class="self-end"
            @click="queryCustomers"
          >
            Query
          </UButton>
        </div>
        <div class="mt-4">
          <FormStatus
            :loading="queryState.loading"
            :error="queryState.error"
            :empty="queryState.empty"
          />
          <UTable
            v-if="queryResults.length"
            :rows="queryResults"
            :columns="[
              { key: 'systemUserId', label: 'User Id' },
              { key: 'firstName', label: 'First Name' },
              { key: 'lastName', label: 'Last Name' },
              { key: 'birthDate', label: 'Birth Date' }
            ]"
          >
            <template #systemUserId-data="{ row }">
              <span class="font-mono text-xs">{{ row.systemUserId }}</span>
            </template>
          </UTable>
        </div>
      </UCard>

      <UCard class="border border-default">
        <template #header>
          <h2 class="text-lg font-semibold">
            Lookup customer
          </h2>
        </template>
        <div class="flex gap-3">
          <UFormField
            label="Customer Id"
            required
            class="flex-1"
          >
            <UInput
              v-model="lookupForm.customerId"
              placeholder="GUID"
            />
          </UFormField>
          <UButton
            color="neutral"
            variant="outline"
            class="self-end"
            @click="lookupCustomer"
          >
            Load
          </UButton>
        </div>
        <FormStatus
          :loading="lookupState.loading"
          :error="lookupState.error"
        />
        <div
          v-if="lookupResult"
          class="mt-4 rounded-lg border border-default p-4 text-sm"
        >
          <p class="font-semibold">
            {{ lookupResult.firstName }} {{ lookupResult.lastName }}
          </p>
          <p class="text-muted">
            User Id: {{ lookupResult.systemUserId }}
          </p>
          <p class="text-muted">
            Birth Date: {{ lookupResult.birthDate }}
          </p>
        </div>
      </UCard>
    </div>

    <div
      v-else-if="active === 'create'"
      class="grid gap-6 lg:grid-cols-[2fr_1fr]"
    >
      <UCard class="border border-default">
        <template #header>
          <h2 class="text-lg font-semibold">
            Create customer
          </h2>
        </template>
        <UForm
          class="space-y-4"
          @submit="createCustomer"
        >
          <UFormField
            label="System User Id"
            required
          >
            <UInput
              v-model="createForm.systemUserId"
              placeholder="GUID"
            />
          </UFormField>
          <div class="grid gap-4 md:grid-cols-2">
            <UFormField
              label="First Name"
              required
            >
              <UInput v-model="createForm.firstName" />
            </UFormField>
            <UFormField
              label="Last Name"
              required
            >
              <UInput v-model="createForm.lastName" />
            </UFormField>
          </div>
          <UFormField
            label="Birth Date"
            required
          >
            <UInput
              v-model="createForm.birthDate"
              type="date"
            />
          </UFormField>
          <UFormField label="Default Address Id">
            <UInput
              v-model="createForm.defaultAddressId"
              placeholder="GUID"
            />
          </UFormField>
          <UButton
            type="submit"
            color="primary"
            :loading="createState.loading"
          >
            Create Customer
          </UButton>
          <FormStatus
            :error="createState.error"
            :success="createState.success"
          />
        </UForm>
      </UCard>
      <UCard class="border border-default bg-elevated/40">
        <template #header>
          <h3 class="text-sm font-semibold text-muted">
            Requirements
          </h3>
        </template>
        <p class="text-sm text-muted">
          Customer must reference an existing system user (Identity service).
        </p>
      </UCard>
    </div>

    <div
      v-else-if="active === 'update'"
      class="grid gap-6 lg:grid-cols-[2fr_1fr]"
    >
      <UCard class="border border-default">
        <template #header>
          <h2 class="text-lg font-semibold">
            Update customer
          </h2>
        </template>
        <UForm
          class="space-y-4"
          @submit="updateCustomer"
        >
          <UFormField
            label="System User Id"
            required
          >
            <UInput
              v-model="updateForm.systemUserId"
              placeholder="GUID"
            />
          </UFormField>
          <div class="grid gap-4 md:grid-cols-2">
            <UFormField
              label="First Name"
              required
            >
              <UInput v-model="updateForm.firstName" />
            </UFormField>
            <UFormField
              label="Last Name"
              required
            >
              <UInput v-model="updateForm.lastName" />
            </UFormField>
          </div>
          <UFormField
            label="Birth Date"
            required
          >
            <UInput
              v-model="updateForm.birthDate"
              type="date"
            />
          </UFormField>
          <UFormField label="Default Address Id">
            <UInput
              v-model="updateForm.defaultAddressId"
              placeholder="GUID"
            />
          </UFormField>
          <UButton
            type="submit"
            color="primary"
            :loading="updateState.loading"
          >
            Update Customer
          </UButton>
          <FormStatus
            :error="updateState.error"
            :success="updateState.success"
          />
        </UForm>
      </UCard>
      <UCard class="border border-default bg-elevated/40">
        <template #header>
          <h3 class="text-sm font-semibold text-muted">
            Tip
          </h3>
        </template>
        <p class="text-sm text-muted">
          SystemUserId must match the customer record being updated.
        </p>
      </UCard>
    </div>
    <div
      v-else-if="active === 'delete'"
      class="grid gap-6 lg:grid-cols-[2fr_1fr]"
    >
      <UCard class="border border-default">
        <template #header>
          <h2 class="text-lg font-semibold">
            Delete customer
          </h2>
        </template>
        <UForm
          class="space-y-4"
          @submit="deleteCustomer"
        >
          <UFormField
            label="System User Id"
            required
          >
            <UInput
              v-model="deleteForm.systemUserId"
              placeholder="GUID"
            />
          </UFormField>
          <UButton
            type="submit"
            color="error"
            variant="solid"
            :loading="deleteState.loading"
          >
            Delete Customer
          </UButton>
          <FormStatus
            :error="deleteState.error"
            :success="deleteState.success"
          />
        </UForm>
      </UCard>
      <UCard class="border border-default bg-elevated/40">
        <template #header>
          <h3 class="text-sm font-semibold text-muted">
            Result
          </h3>
        </template>
        <p class="text-sm text-muted">
          Customer deletion responds with a confirmation message if successful.
        </p>
      </UCard>
    </div>
  </FeatureShell>
</template>
