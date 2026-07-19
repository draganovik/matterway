<script setup lang="ts">
import { useUsersAccountsPage } from "~/composables/features/users/useUsersAccountsPage"

definePageMeta({
  title: "Nalozi",
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
  deleteConfirmOpen,
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
} = useUsersAccountsPage()
</script>

<template>
  <UDashboardPanel
    id="users-accounts"
    :ui="{
      body: 'min-h-0 overflow-hidden py-3',
    }"
  >
    <template #header>
      <UDashboardNavbar title="Nalozi">
        <template #leading>
          <UDashboardSidebarCollapse />
        </template>

        <template #trailing>
          <PageInfoTooltip
            text="Pregledajte korisnike, ažurirajte podatke za prijavu i upravljajte dozvolama po servisima."
          />
        </template>

        <template #right>
          <UButton
            color="primary"
            :disabled="!canManage"
            @click="
              () => {
                createModalOpen = true
              }
            "
          >
            Novi nalog zaposlenog
          </UButton>
        </template>
      </UDashboardNavbar>
    </template>

    <template #body>
      <div class="h-full min-h-0">
        <EntitiesSplitView
          class="h-full min-h-0"
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
              @update:filter="
                (value) => {
                  filter = value
                }
              "
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
              :save-error="saveState.error"
              :save-success="saveState.success"
              @save="saveSystemUser"
              @remove="requestRemoveSystemUser"
              @reveal-roles="revealRoles"
            />
          </template>
        </EntitiesSplitView>

        <UsersAccountsCreateModal
          v-model:open="createModalOpen"
          :can-manage="canManage"
          @created="handleEmployeeCreated"
        />

        <UsersAccountsPermissionsModal
          v-model:open="rolesModalOpen"
          :system-user-id="selectedSystemUser?.id || null"
          :user-label="selectedUserLabel"
          :can-manage="canManage"
        />

        <ConfirmDeleteModal
          v-model:open="deleteConfirmOpen"
          title="Obriši nalog"
          description="Nalog će biti trajno uklonjen iz identiteta i više neće moći da se koristi za prijavu."
          :subject="
            selectedSystemUser
              ? `${selectedUserLabel} · ${selectedSystemUser.id}`
              : ''
          "
          confirm-label="Obriši nalog"
          :loading="removeState.loading"
          :error="removeState.error"
          @confirm="removeSystemUser"
        />
      </div>
    </template>
  </UDashboardPanel>
</template>
