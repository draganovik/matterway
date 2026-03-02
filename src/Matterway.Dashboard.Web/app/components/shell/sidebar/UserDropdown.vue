<script setup lang="ts">
import type { DropdownMenuItem } from "@nuxt/ui"
import { getJwtStringClaim } from "~/utils/jwt"
import { useAuthSession } from "~/composables/useAuthSession"

defineProps<{
  collapsed?: boolean
}>()

const auth = useAuthSession()
const userLabel = computed(() => {
  const payload = auth.payload.value
  const candidates = [
    getJwtStringClaim(payload, "email"),
    getJwtStringClaim(
      payload,
      "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress",
    ),
    getJwtStringClaim(payload, "preferred_username"),
    getJwtStringClaim(payload, "upn"),
    getJwtStringClaim(payload, "unique_name"),
    getJwtStringClaim(payload, "name"),
    getJwtStringClaim(payload, "sub"),
  ].filter(Boolean) as string[]

  const isUuid = (value: string) =>
    /^[0-9a-f]{8}-[0-9a-f]{4}-[1-5][0-9a-f]{3}-[89ab][0-9a-f]{3}-[0-9a-f]{12}$/i.test(
      value,
    )
  const isHexish = (value: string) => /^[0-9a-f]{16,}$/i.test(value)

  const raw =
    candidates.find((value) => value && !isUuid(value) && !isHexish(value)) ||
    ""
  const prefix = raw.split("@")[0] || ""
  return prefix || auth.role.value || "Employee"
})

const buttonLabel = computed(() => {
  const payload = auth.payload.value
  const guidCandidates = [
    getJwtStringClaim(payload, "sub"),
    getJwtStringClaim(
      payload,
      "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier",
    ),
  ].filter(Boolean) as string[]

  const guid = guidCandidates[0]?.trim()
  return guid || userLabel.value
})
const items = computed<DropdownMenuItem[][]>(() => [
  [
    {
      type: "label",
      label: userLabel.value,
      icon: "i-lucide-shield",
    },
  ],
  [
    {
      label: "Log out",
      icon: "i-lucide-log-out",
      onSelect: async () => {
        await auth.logout()
        await navigateTo("/login")
      },
    },
  ],
])
</script>

<template>
  <UDropdownMenu
    :items="items"
    :content="{ align: 'center', collisionPadding: 12 }"
    :ui="{
      content: collapsed
        ? 'min-w-56 w-56'
        : 'min-w-56 w-(--reka-dropdown-menu-trigger-width)',
    }"
  >
    <UButton
      :label="collapsed ? undefined : buttonLabel"
      icon="i-lucide-user"
      color="neutral"
      variant="ghost"
      block
      :square="collapsed"
      class="data-[state=open]:bg-elevated"
    />
  </UDropdownMenu>
</template>
