import { buildQuery } from '~/utils/http'
import { useApiClient } from '~/composables/useApiClient'
import type {
  AddSystemUserPermRequest,
  AddSystemUserPermResponse,
  DeleteSystemUserPermRequest,
  DeleteSystemUserPermResponse,
  QuerySystemUsersParams,
  QuerySystemUsersResponse,
  SystemUserPermResponse,
  SystemUserResponse,
  UpdateSystemUserRequest,
  UpdateSystemUserResponse
} from '~/types/identity'

const ADMIN_SYSTEM_USERS_PATH = 'admin/system-users'

export function useIdentityApi() {
  const api = useApiClient()

  async function querySystemUsers(params: QuerySystemUsersParams) {
    const query = buildQuery({
      Page: params.page,
      PageSize: params.pageSize,
      Role: params.role || undefined
    })

    return api.request<QuerySystemUsersResponse>(
      'identity',
      `${ADMIN_SYSTEM_USERS_PATH}${query}`
    )
  }

  async function getSystemUserById(id: string) {
    return api.request<SystemUserResponse>(
      'identity',
      `${ADMIN_SYSTEM_USERS_PATH}/${id}`
    )
  }

  async function updateSystemUser(
    id: string,
    payload: UpdateSystemUserRequest
  ) {
    return api.request<UpdateSystemUserResponse>(
      'identity',
      `${ADMIN_SYSTEM_USERS_PATH}/${id}`,
      {
        method: 'PATCH',
        body: JSON.stringify(payload)
      }
    )
  }

  async function deleteSystemUser(id: string) {
    return api.request('identity', `${ADMIN_SYSTEM_USERS_PATH}/${id}`, {
      method: 'DELETE'
    })
  }

  async function getSystemUserPerms(id: string) {
    return api.request<SystemUserPermResponse[]>(
      'identity',
      `${ADMIN_SYSTEM_USERS_PATH}/${id}/perms`
    )
  }

  async function addSystemUserPerm(
    id: string,
    payload: AddSystemUserPermRequest
  ) {
    return api.request<AddSystemUserPermResponse[]>(
      'identity',
      `${ADMIN_SYSTEM_USERS_PATH}/${id}/perms`,
      {
        method: 'POST',
        body: JSON.stringify(payload)
      }
    )
  }

  async function deleteSystemUserPerm(
    id: string,
    payload: DeleteSystemUserPermRequest
  ) {
    return api.request<DeleteSystemUserPermResponse[]>(
      'identity',
      `${ADMIN_SYSTEM_USERS_PATH}/${id}/perms`,
      {
        method: 'DELETE',
        body: JSON.stringify(payload)
      }
    )
  }

  return {
    querySystemUsers,
    getSystemUserById,
    updateSystemUser,
    deleteSystemUser,
    getSystemUserPerms,
    addSystemUserPerm,
    deleteSystemUserPerm
  }
}
