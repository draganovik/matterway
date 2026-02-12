<script setup lang="ts">
import type { DropdownMenuItem } from "@nuxt/ui";
import { getJwtStringClaim } from "~/utils/jwt";
import { useAuthSession } from "~/composables/useAuthSession";
import { useCart } from "~/composables/useCart";

const auth = useAuthSession();
const cart = useCart();

const userLabel = computed(() => {
  const payload = auth.payload.value;
  const candidates = [
    getJwtStringClaim(payload, "email"),
    getJwtStringClaim(
      payload,
      "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress",
    ),
    getJwtStringClaim(payload, "preferred_username"),
    getJwtStringClaim(payload, "name"),
    getJwtStringClaim(payload, "sub"),
  ].filter(Boolean) as string[];

  const raw = candidates[0] || "Kupac";
  return raw.includes("@") ? raw.split("@")[0] : raw;
});

const items = computed<DropdownMenuItem[][]>(() => [
  [
    {
      type: "label",
      label: userLabel.value,
      icon: "i-lucide-user-round",
    },
  ],
  [
    {
      label: "Moje porudžbine",
      icon: "i-lucide-package-check",
      to: "/orders",
    },
    {
      label: "Korpa",
      icon: "i-lucide-shopping-bag",
      to: "/cart",
    },
  ],
  [
    {
      label: "Odjavi se",
      icon: "i-lucide-log-out",
      onSelect: async () => {
        await cart.clear();
        await auth.logout();
        await navigateTo("/");
      },
    },
  ],
]);
</script>

<template>
  <UDropdownMenu :items="items" :content="{ align: 'end' }">
    <UButton
      :label="userLabel"
      icon="i-lucide-user-round"
      color="neutral"
      variant="soft"
      class="hidden sm:inline-flex"
    />
    <UButton
      icon="i-lucide-user-round"
      color="neutral"
      variant="soft"
      square
      class="sm:hidden"
    />
  </UDropdownMenu>
</template>
