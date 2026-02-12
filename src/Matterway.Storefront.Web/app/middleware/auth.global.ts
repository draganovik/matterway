import { useAuthSession } from "~/composables/useAuthSession";

export default defineNuxtRouteMiddleware(async (to) => {
  const auth = useAuthSession();

  if (to.meta.public) {
    void auth.initialize();
    return;
  }

  await auth.initialize();

  if (!auth.isLoggedIn.value || !auth.isCustomer.value) {
    const nextPath = encodeURIComponent(to.fullPath);
    return navigateTo(`/login?next=${nextPath}`);
  }
});
