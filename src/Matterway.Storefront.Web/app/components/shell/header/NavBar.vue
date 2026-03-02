<script setup lang="ts">
import { useAuthSession } from "~/composables/useAuthSession"
import { useCart } from "~/composables/useCart"
import { getJwtStringClaim } from "~/utils/jwt"

const auth = useAuthSession()
const cart = useCart()
const route = useRoute()
const router = useRouter()

const mobileMenuOpen = ref(false)
const searchTerm = ref("")

const navItems = [
  { label: "Pregled", to: "/" },
  { label: "Artikli", to: "/articles" },
]

const cartCount = computed(() => cart.totalItems.value)

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

function isActive(to: string) {
  return route.path === to || (to !== "/" && route.path.startsWith(`${to}/`))
}

function submitSearch() {
  const query = searchTerm.value.trim()
  router.push({
    path: "/articles",
    query: {
      articleName: query || undefined,
    },
  })
  mobileMenuOpen.value = false
}

async function logoutFromCompactMenu() {
  await cart.clear()
  await auth.logout()
  mobileMenuOpen.value = false
  await navigateTo("/")
}

watch(
  () => route.fullPath,
  () => {
    mobileMenuOpen.value = false
  },
)
</script>

<template>
  <header
    class="bg-default/85 border-default fixed inset-x-0 top-0 z-40 border-b backdrop-blur"
  >
    <div
      class="mx-auto flex h-16 w-full max-w-7xl items-center gap-4 px-4 sm:px-6 lg:px-8"
    >
      <HeaderAppLogo />

      <nav class="ml-2 hidden items-center gap-1 lg:flex">
        <UButton
          v-for="item in navItems"
          :key="item.to"
          :to="item.to"
          variant="ghost"
          color="neutral"
          :class="
            isActive(item.to)
              ? 'bg-elevated text-toned ring-default ring-1 ring-inset'
              : 'hover:bg-muted hover:text-highlighted focus-visible:bg-muted focus-visible:text-highlighted'
          "
        >
          {{ item.label }}
        </UButton>
      </nav>

      <div class="ml-auto hidden w-full max-w-3xl items-center gap-2 lg:flex">
        <form class="min-w-0 flex-1" @submit.prevent="submitSearch">
          <label for="article-search-desktop" class="sr-only">
            Pretraži artikle
          </label>
          <UInput
            id="article-search-desktop"
            v-model="searchTerm"
            icon="i-lucide-search"
            placeholder="Pretraži artikle"
            size="md"
            class="w-full"
            :ui="{ root: 'w-full' }"
          />
        </form>

        <UButton
          v-if="auth.isLoggedIn.value && auth.isCustomer.value"
          to="/orders"
          color="neutral"
          variant="ghost"
          icon="i-lucide-package-check"
          square
          :ui="{ leadingIcon: 'text-lg' }"
          class="hover:bg-muted hover:text-highlighted hover:ring-default focus-visible:bg-muted focus-visible:text-highlighted focus-visible:ring-default hover:ring-1 hover:ring-inset focus-visible:ring-1 focus-visible:ring-inset"
        />

        <UButton
          to="/cart"
          color="neutral"
          variant="ghost"
          icon="i-lucide-shopping-bag"
          :ui="{ leadingIcon: 'text-lg' }"
          class="hover:bg-muted hover:text-highlighted hover:ring-default focus-visible:bg-muted focus-visible:text-highlighted focus-visible:ring-default hover:ring-1 hover:ring-inset focus-visible:ring-1 focus-visible:ring-inset"
        >
          <span v-if="cartCount">{{ cartCount }}</span>
        </UButton>

        <HeaderUserDropdown
          v-if="auth.isLoggedIn.value && auth.isCustomer.value"
        />
        <UButton
          v-else
          to="/login"
          color="primary"
          variant="soft"
          icon="i-lucide-log-in"
        >
          Prijava
        </UButton>
      </div>

      <div class="ml-auto flex items-center gap-2 lg:hidden">
        <UButton
          to="/cart"
          color="neutral"
          variant="ghost"
          icon="i-lucide-shopping-bag"
          :ui="{ leadingIcon: 'text-lg' }"
          class="hover:bg-muted hover:text-highlighted hover:ring-default focus-visible:bg-muted focus-visible:text-highlighted focus-visible:ring-default hover:ring-1 hover:ring-inset focus-visible:ring-1 focus-visible:ring-inset"
        >
          <span v-if="cartCount">{{ cartCount }}</span>
        </UButton>

        <UButton
          color="neutral"
          variant="ghost"
          icon="i-lucide-menu"
          square
          class="hover:bg-muted hover:text-highlighted hover:ring-default focus-visible:bg-muted focus-visible:text-highlighted focus-visible:ring-default hover:ring-1 hover:ring-inset focus-visible:ring-1 focus-visible:ring-inset"
          @click="mobileMenuOpen = !mobileMenuOpen"
        />
      </div>
    </div>

    <div
      v-if="mobileMenuOpen"
      class="border-default bg-default border-t px-4 py-4 lg:hidden"
    >
      <form class="mb-3 w-full" @submit.prevent="submitSearch">
        <label for="article-search-mobile" class="sr-only">
          Pretraži artikle
        </label>
        <UInput
          id="article-search-mobile"
          v-model="searchTerm"
          icon="i-lucide-search"
          placeholder="Pretraži artikle"
          class="w-full"
          :ui="{ root: 'w-full' }"
        />
      </form>
      <div class="flex flex-col gap-1">
        <div
          v-if="auth.isLoggedIn.value && auth.isCustomer.value"
          class="border-default bg-elevated/60 mb-2 rounded-md border px-3 py-2"
        >
          <p class="text-muted text-xs">Prijavljeni korisnik</p>
          <p class="text-sm font-semibold">{{ userLabel }}</p>
        </div>

        <UButton
          v-for="item in navItems"
          :key="`mobile-${item.to}`"
          :to="item.to"
          variant="ghost"
          color="neutral"
          block
          :class="
            isActive(item.to)
              ? 'bg-elevated text-toned ring-default ring-1 ring-inset'
              : 'hover:bg-muted hover:text-highlighted focus-visible:bg-muted focus-visible:text-highlighted'
          "
          @click="mobileMenuOpen = false"
        >
          {{ item.label }}
        </UButton>

        <UButton
          v-if="!auth.isLoggedIn.value"
          to="/login"
          color="primary"
          variant="soft"
          block
          @click="mobileMenuOpen = false"
        >
          Prijava
        </UButton>

        <template v-else-if="auth.isCustomer.value">
          <UButton
            to="/orders"
            color="neutral"
            variant="ghost"
            icon="i-lucide-package-check"
            block
            @click="mobileMenuOpen = false"
          >
            Moje porudžbine
          </UButton>

          <UButton
            color="error"
            variant="soft"
            icon="i-lucide-log-out"
            block
            @click="logoutFromCompactMenu"
          >
            Odjavi se
          </UButton>
        </template>
      </div>
    </div>
  </header>
</template>
