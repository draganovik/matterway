export type CheckoutAddress = {
  receiverName: string;
  residence: string;
  street: string;
  city: string;
  zipCode: string;
  country: string;
  contactPhone: string;
  note: string;
};

export type RegisterPayload = {
  firstName: string;
  lastName: string;
  birthDate: string;
  email: string;
  password: string;
};

export type CustomerAddressResponse = {
  addressLine1?: string;
  addressLine2?: string;
  city?: string;
  zipCode?: string;
  country?: string;
  contactPhone?: string;
};
