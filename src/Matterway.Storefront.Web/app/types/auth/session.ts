export type AuthSession = {
  accessToken: string | null
  tokenType: string
  created: string | null
  expires: string | null
  refreshExpires: string | null
}

export type LoginResponse = {
  token: string
  refreshToken: string
  tokenType: string
  created: string
  expires: string
  refreshExpires: string
}

export type LoginPayload = {
  email: string
  password: string
}
