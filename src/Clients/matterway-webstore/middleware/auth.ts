import { useSessionStore } from "~/store/session";
import { UserRole } from "~/utils/UserRole";

export default defineNuxtRouteMiddleware((to, from) => {
  const sessionData = useSessionStore();
  const requiresRole: UserRole[] = to.meta?.authOnlyRoles as UserRole[];
  const requiresNoSession: boolean = to.meta?.authNoSession as boolean;

  if (sessionData.isLoggedIn) {
    if (requiresNoSession) {
      if (from == null || to.path == from.path) {
        return navigateTo("/");
      }
      return navigateTo(from);
    }
    return navigateTo(to);
  }

  if (requiresRole && !sessionData.isLoggedIn) {
    return navigateTo("/login");
  }
});
