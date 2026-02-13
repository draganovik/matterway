import type { PaginationResponse } from "../common/pagination"

export type QuerySystemUsersParams = {
  page: number
  pageSize: number
  role?: IdentityRole
}

export type IdentityRole = "Customer" | "Employee"

export type SystemUserResponse = {
  id: string
  email?: string | null
  created: string
  role: IdentityRole
}

export type QuerySystemUsersResponse = PaginationResponse<SystemUserResponse>

export type UpdateSystemUserRequest = {
  email?: string | null
  password?: string | null
}

export type UpdateSystemUserResponse = SystemUserResponse

export type SystemUserPermLevel = "Observer" | "Operator" | "Administrator"

export type SystemUserPermResponse = {
  service: string
  level: SystemUserPermLevel
}

export type AddSystemUserPermRequest = {
  service: string
  level: SystemUserPermLevel
}

export type DeleteSystemUserPermRequest = AddSystemUserPermRequest

export type AddSystemUserPermResponse = SystemUserPermResponse

export type DeleteSystemUserPermResponse = SystemUserPermResponse
