export function formatIdentityRole(value?: string | null) {
  if (!value) return "Nepoznata uloga"
  if (value === "Customer") return "Kupac"
  if (value === "Employee") return "Zaposleni"
  return value
}

export function formatPermissionLevel(value?: string | null) {
  if (!value) return "Nepoznata dozvola"
  if (value === "Observer") return "Pregled"
  if (value === "Operator") return "Operater"
  if (value === "Manager") return "Menadžer"
  return value
}

export function formatServiceName(value?: string | null) {
  if (!value) return "Nepoznati servis"
  if (value === "catalog") return "Katalog"
  if (value === "customers") return "Kupci"
  if (value === "identity") return "Identitet"
  if (value === "sales") return "Prodaja"
  return value
}

export function formatOrderType(value?: string | null) {
  if (!value) return "Nepoznat tip"
  if (value === "Retail") return "Maloprodaja"
  if (value === "Ecommerce") return "E-trgovina"
  return value
}

export function formatOrderStatus(value?: string | null) {
  if (!value) return "Nepoznat status"
  if (value === "Processing") return "Obrada"
  if (value === "Reserved") return "Rezervisano"
  if (value === "Delivery") return "Dostava"
  if (value === "Completed") return "Završeno"
  if (value === "Cancelled") return "Otkazano"
  return value
}

export function formatPaymentStatus(value?: string | null) {
  if (!value) return "Nepoznat status"
  if (value === "Reserved") return "Rezervisano"
  if (value === "Charged") return "Naplaćeno"
  if (value === "Failed") return "Neuspešno"
  if (value === "Refunded") return "Refundirano"
  return value
}
