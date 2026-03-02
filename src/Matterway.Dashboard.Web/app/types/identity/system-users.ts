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

export type CreateEmployeeUserRequest = {
  email: string
  password: string
}

export type CreateEmployeeUserResponse = SystemUserResponse

export type SystemUserPermLevel = "Observer" | "Operator" | "Manager"

export type SystemUserPermResponse = {
  service: string
  level: SystemUserPermLevel
}

export type PatchSystemUserPermRequest = {
  service: string
  level: SystemUserPermLevel
}

export type PatchSystemUserPermResponse = SystemUserPermResponse
