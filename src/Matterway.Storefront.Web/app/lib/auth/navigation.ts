export function resolveNextRoute(route: ReturnType<typeof useRoute>) {
  const next = route.query.next
  if (typeof next === "string" && next.startsWith("/")) {
    return next
  }
  return "/"
}
