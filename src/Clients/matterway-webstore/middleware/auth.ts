import { useSessionStore } from "~/store/session";
import { UserRole } from "~/utils/UserRole";


export default defineNuxtRouteMiddleware((to, from) => {
  const sessionData = useSessionStore()
  const requiresAuthorization : UserRole[] = to.meta?.authorization as UserRole[];
  console.log(requiresAuthorization, sessionData)
  if(requiresAuthorization && !sessionData.isLoggedIn)
  {
    return navigateTo("/login");
  }
});
