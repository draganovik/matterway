<script setup lang="ts">
import { useAuthSessionStore } from "~/composables/stores/useAuthSessionStore"
import { useCartStore } from "~/composables/stores/useCartStore"

const auth = useAuthSessionStore()
const cart = useCartStore()
const route = useRoute()
const router = useRouter()

const mobileMenuOpen = ref(false)
const searchTerm = ref("")

const navItems = [
  { label: "Početna", to: "/" },
  { label: "Artikli", to: "/articles" },
]

const cartCount = computed(() => cart.totalItems.value)

const userLabel = computed(() => {
  return `Zdravo, ${auth.customerFirstName.value || "Kupac"}!`
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
    class="bg-default/85 border-default sticky top-0 z-40 border-b backdrop-blur"
  >
    <div class="px-4 sm:px-5 lg:px-6">
      <div class="mx-auto flex h-14 w-full max-w-6xl items-center gap-3">
        <AppLogo />

        <nav class="ml-2 hidden items-center gap-1 lg:flex">
          <UButton
            v-for="item in navItems"
            :key="item.to"
            :to="item.to"
            variant="ghost"
            color="neutral"
            :class="
              isActive(item.to)
                ? 'bg-muted text-highlighted'
                : 'text-toned hover:bg-muted hover:text-highlighted focus-visible:bg-muted focus-visible:text-highlighted'
            "
          >
            {{ item.label }}
          </UButton>
        </nav>

        <div
          class="ml-auto hidden max-w-2xl min-w-0 flex-1 items-center gap-2 lg:flex"
        >
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
              :ui="{
                root: 'group',
                leadingIcon: 'relative z-20 group-focus-within:text-primary',
              }"
            />
          </form>

          <UButton
            v-if="auth.isLoggedIn.value && auth.isCustomer.value"
            to="/orders"
            color="neutral"
            variant="ghost"
            icon="i-lucide-package-check"
            square
            aria-label="Moje porudžbine"
            :ui="{ leadingIcon: 'text-lg' }"
            class="hover:bg-muted hover:text-highlighted focus-visible:bg-muted focus-visible:text-highlighted"
          />

          <UButton
            to="/cart"
            color="neutral"
            variant="ghost"
            icon="i-lucide-shopping-bag"
            aria-label="Korpa"
            :ui="{ leadingIcon: 'text-lg' }"
            class="hover:bg-muted hover:text-highlighted focus-visible:bg-muted focus-visible:text-highlighted"
          >
            <span v-if="cartCount">{{ cartCount }}</span>
          </UButton>

          <UserDropdown v-if="auth.isLoggedIn.value && auth.isCustomer.value" />
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
            aria-label="Korpa"
            :ui="{ leadingIcon: 'text-lg' }"
            class="hover:bg-muted hover:text-highlighted focus-visible:bg-muted focus-visible:text-highlighted"
          >
            <span v-if="cartCount">{{ cartCount }}</span>
          </UButton>

          <UButton
            color="neutral"
            variant="ghost"
            icon="i-lucide-menu"
            square
            aria-label="Otvori meni"
            :aria-expanded="mobileMenuOpen"
            class="hover:bg-muted hover:text-highlighted focus-visible:bg-muted focus-visible:text-highlighted"
            @click="mobileMenuOpen = !mobileMenuOpen"
          />
        </div>
      </div>
    </div>

    <div
      v-if="mobileMenuOpen"
      class="border-default bg-default border-t lg:hidden"
    >
      <div class="mx-auto w-full max-w-6xl px-4 py-3 sm:px-5">
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
            :ui="{
              root: 'group',
              leadingIcon: 'relative z-20 group-focus-within:text-primary',
            }"
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
                ? 'bg-muted text-highlighted'
                : 'text-toned hover:bg-muted hover:text-highlighted focus-visible:bg-muted focus-visible:text-highlighted'
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
              to="/profile"
              color="neutral"
              variant="ghost"
              icon="i-lucide-user-cog"
              block
              @click="mobileMenuOpen = false"
            >
              Moj profil
            </UButton>

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
    </div>
  </header>
</template>
