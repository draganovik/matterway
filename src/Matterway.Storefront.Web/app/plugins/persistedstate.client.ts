import piniaPluginPersistedstate from "pinia-plugin-persistedstate";

export default defineNuxtPlugin((nuxtApp) => {
  if (!nuxtApp.$pinia) {
    return;
  }

  nuxtApp.$pinia.use(piniaPluginPersistedstate);
});
