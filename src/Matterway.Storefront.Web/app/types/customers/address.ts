export type CheckoutAddress = {
  receiverName: string
  residence: string
  street: string
  city: string
  zipCode: string
  country: string
  contactPhone: string
  note: string
}

export type RegisterPayload = {
  firstName: string
  lastName: string
  birthDate: string
  email: string
  password: string
}

export type SelfProfileResponse = {
  systemUserId: string
  firstName?: string | null
  lastName?: string | null
  birthDate: string
  defaultAddressId?: string | null
}

export type SelfProfileUpdateRequest = {
  systemUserId: string
  firstName: string
  lastName: string
  birthDate: string
  defaultAddressId?: string | null
}

export type PutSelfAddressRequest = {
  country: string
  city: string
  zipCode: string
  addressLine1: string
  addressLine2: string
  contactPhone: string
}

export type CustomerAddressResponse = {
  id?: string
  addressLine1?: string
  addressLine2?: string
  city?: string
  zipCode?: string
  country?: string
  contactPhone?: string
}
