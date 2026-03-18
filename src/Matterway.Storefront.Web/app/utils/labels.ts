export function formatOrderStatus(value?: string | null) {
  if (!value) return "Nema statusa"
  if (value === "Processing") return "Obrada"
  if (value === "Reserved") return "Rezervisano"
  if (value === "Delivery") return "Dostava"
  if (value === "Completed") return "Završeno"
  if (value === "Cancelled") return "Otkazano"
  return value
}
