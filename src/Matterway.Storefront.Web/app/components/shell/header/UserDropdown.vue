<script setup lang="ts">
import type { DropdownMenuItem } from "@nuxt/ui"
import { getJwtStringClaim } from "~/utils/jwt"
import { useAuthSession } from "~/composables/useAuthSession"
import { useCart } from "~/composables/useCart"

const auth = useAuthSession()
const cart = useCart()

const userLabel = computed(() => {
  if (auth.customerId.value?.trim()) return auth.customerId.value.trim()

  const payload = auth.payload.value
  const candidates = [
    getJwtStringClaim(payload, "email"),
    getJwtStringClaim(payload, "preferred_username"),
    getJwtStringClaim(payload, "name"),
    getJwtStringClaim(payload, "sub"),
  ].filter(Boolean) as string[]

  const raw = candidates[0] || "Kupac"
  return raw.includes("@") ? raw.split("@")[0] : raw
})

const roleLabel = computed(() => {
  const role = auth.role.value?.trim().toLowerCase()
  if (role === "customer") return "Kupac"
  if (role === "employee") return "Zaposleni"
  return auth.role.value?.trim() || "Korisnik"
})

const items = computed<DropdownMenuItem[][]>(() => [
  [
    {
      type: "label",
      label: roleLabel.value,
      icon: "i-lucide-shield",
    },
  ],
  [
    {
      label: "Odjavi se",
      icon: "i-lucide-log-out",
      onSelect: async () => {
        await cart.clear()
        await auth.logout()
        await navigateTo("/")
      },
    },
  ],
])
</script>

<template>
  <UDropdownMenu
    :items="items"
    :content="{ align: 'end' }"
    :ui="{ content: 'min-w-56 border border-default bg-default' }"
  >
    <UButton
      :label="userLabel"
      icon="i-lucide-user-round"
      color="neutral"
      variant="ghost"
      class="hidden sm:inline-flex"
    />
    <UButton
      icon="i-lucide-user-round"
      color="neutral"
      variant="ghost"
      square
      class="sm:hidden"
    />
  </UDropdownMenu>
</template>
