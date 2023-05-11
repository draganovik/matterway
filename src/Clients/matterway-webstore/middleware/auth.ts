import useSessionData from "~/composables/useSessionData";

export default defineNuxtRouteMiddleware((to, from) => {
  const { sessionData, getSessionData } = useSessionData();
  getSessionData();

  if (!sessionData.value) {
    return navigateTo("/login");
  }
});
