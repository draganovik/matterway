<script setup lang="ts">
import type { DropdownMenuItem } from '@nuxt/ui'
import { useAuthSession } from '~/composables/useAuthSession'

defineProps<{
  collapsed?: boolean
}>()

const auth = useAuthSession()
const colorMode = useColorMode()
const userLabel = computed(() => auth.role.value ? `Employee (${auth.role.value})` : 'Employee')
const items = computed<DropdownMenuItem[][]>(() => ([[{
  type: 'label',
  label: userLabel.value,
  icon: 'i-lucide-shield'
}], [{
  label: 'Light',
  icon: 'i-lucide-sun',
  type: 'checkbox',
  checked: colorMode.value === 'light',
  onSelect(e: Event) {
    e.preventDefault()
    colorMode.preference = 'light'
  }
}, {
  label: 'Dark',
  icon: 'i-lucide-moon',
  type: 'checkbox',
  checked: colorMode.value === 'dark',
  onSelect(e: Event) {
    e.preventDefault()
    colorMode.preference = 'dark'
  }
}], [{
  label: 'Log out',
  icon: 'i-lucide-log-out',
  onSelect: async () => {
    await auth.logout()
    await navigateTo('/login')
  }
}]]))
</script>

<template>
  <UDropdownMenu
    :items="items"
    :content="{ align: 'center', collisionPadding: 12 }"
    :ui="{ content: collapsed ? 'w-40' : 'w-(--reka-dropdown-menu-trigger-width)' }"
  >
    <UButton
      :label="collapsed ? undefined : userLabel"
      icon="i-lucide-user"
      color="neutral"
      variant="ghost"
      block
      :square="collapsed"
      class="data-[state=open]:bg-elevated"
    />
  </UDropdownMenu>
</template>
