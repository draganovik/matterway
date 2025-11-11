import { useSessionStore } from "~/stores/session";

export default defineNuxtRouteMiddleware((to, from) => {
  const sessionData = useSessionStore();
  const requiresRole: string[] = to.meta?.authOnlyRoles as string[];
  const requiresNoSession: boolean = to.meta?.authNoSession as boolean;

  if (requiresNoSession && sessionData.isLoggedIn) {
    console.log(to.path, from.path);
    if (from == null || to.path == from.path) {
      return navigateTo("/");
    }
    return navigateTo(from.path);
  }

  if (
    requiresRole &&
    !requiresRole.includes(sessionData.getTokenData?.role || "None")
  ) {
    if (from == null || to.path == from.path) {
      return navigateTo("/");
    }
    return navigateTo(from.path);
  }

  if (
    !sessionData.isLoggedIn &&
    to.path != "/login" &&
    to.path != "/register"
  ) {
    return navigateTo("/login");
  }
});
