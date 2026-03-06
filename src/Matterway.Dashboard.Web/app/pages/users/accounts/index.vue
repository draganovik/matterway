<script setup lang="ts">
import { useUsersAccountsPage } from "~/composables/features/users/useUsersAccountsPage"

definePageMeta({
  title: "Accounts",
  service: "identity",
  permissions: ["operator", "manager"],
})

const {
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
} = useUsersAccountsPage()
</script>

<template>
  <UDashboardPanel
    id="users-accounts"
    :ui="{ body: 'py-3 sm:py-4 lg:py-6 min-h-0 overflow-hidden' }"
  >
    <template #header>
      <UDashboardNavbar title="Accounts">
        <template #leading>
          <UDashboardSidebarCollapse />
        </template>
      </UDashboardNavbar>
    </template>

    <template #body>
      <div class="h-full min-h-0">
        <div class="flex h-full min-h-0 flex-col gap-4 overflow-hidden">
          <div
            class="flex shrink-0 flex-wrap items-center justify-between gap-3"
          >
            <div>
              <h2 class="text-foreground text-base font-semibold">
                Manage Accounts
              </h2>
              <p class="text-muted text-sm">
                Browse users, update credentials, and manage service
                permissions.
              </p>
            </div>

            <div
              class="flex w-full flex-wrap items-end justify-end gap-3 sm:w-auto"
            >
              <UButton
                color="primary"
                :disabled="!canManage"
                @click="createModalOpen = true"
              >
                Create new Employee Account
              </UButton>
            </div>
          </div>

          <EntitiesSplitView
            class="min-h-0 flex-1"
            list-class="overflow-hidden"
            detail-class="overflow-y-auto"
            :detail-loading="detailState.loading"
          >
            <template #list>
              <UsersAccountsListView
                :items="systemUsers"
                :role-filter="roleFilter"
                :role-filter-options="roleFilterOptions"
                :selected-id="selectedId"
                :filter="filter"
                :loading="listState.loading"
                :error="listState.error"
                :empty-message="listState.empty"
                :page="pagination.page"
                :page-size="pagination.pageSize"
                :total-count="pagination.totalCount"
                :total-pages="pagination.totalPages"
                @update:filter="(value) => (filter = value)"
                @search="searchSystemUsers"
                @update:page="changePage"
                @update:page-size="changePageSize"
                @update:role-filter="changeRoleFilter"
                @select="selectSystemUser"
              />
            </template>

            <template #detail>
              <UsersAccountsInformationPanel
                v-model="form"
                :system-user="selectedSystemUser"
                :error="detailState.error"
                :can-operate="canOperate"
                :can-manage="canManage"
                :save-loading="saveState.loading"
                :remove-loading="removeState.loading"
                :save-error="saveState.error"
                :remove-error="removeState.error"
                :save-success="saveState.success"
                :remove-success="removeState.success"
                @save="saveSystemUser"
                @remove="removeSystemUser"
                @reveal-roles="revealRoles"
              />
            </template>
          </EntitiesSplitView>
        </div>

        <UsersAccountsModalInformationView
          v-model:open="createModalOpen"
          :can-manage="canManage"
          @created="handleEmployeeCreated"
        />

        <UsersAccountsModalRolesView
          v-model:open="rolesModalOpen"
          :system-user-id="selectedSystemUser?.id || null"
          :user-label="selectedUserLabel"
          :can-manage="canManage"
        />
      </div>
    </template>
  </UDashboardPanel>
</template>
