<script setup lang="ts">
import { useAuthSession } from "~/composables/useAuthSession";
import { useCart } from "~/composables/useCart";
import { formatMoney } from "~/utils/formatters";

const auth = useAuthSession();
const cart = useCart();

const isEmpty = computed(() => cart.totalItems.value === 0);

function goToCheckout() {
  if (isEmpty.value) return;
  if (!auth.isLoggedIn.value) {
    void navigateTo("/login?next=%2Fcheckout");
    return;
  }
  void navigateTo("/checkout");
}
</script>

<template>
  <div class="space-y-6">
    <UCard class="border-default bg-elevated/60 border">
      <div class="flex flex-wrap items-center justify-between gap-3">
        <div>
          <p class="text-primary text-xs tracking-[0.3em] uppercase">Korpa</p>
          <h1 class="text-2xl font-semibold">Vaša korpa</h1>
        </div>
        <div class="flex items-center gap-2">
          <UBadge color="primary" variant="soft"
            >{{ cart.totalItems.value }} stavki</UBadge
          >
          <UBadge color="neutral" variant="soft">{{
            formatMoney(cart.totalPrice.value)
          }}</UBadge>
        </div>
      </div>
    </UCard>

    <CommonEmptyState
      v-if="isEmpty"
      title="Korpa je prazna"
      description="Dodajte artikle iz kataloga da nastavite."
      icon="i-lucide-shopping-cart"
    >
      <UButton to="/articles" color="primary">Pregledaj artikle</UButton>
    </CommonEmptyState>

    <div v-else class="grid gap-6 lg:grid-cols-[1fr_20rem]">
      <UCard class="border-default bg-default border">
        <ul class="divide-default divide-y">
          <li
            v-for="item in cart.items.value"
            :key="item.articleId"
            class="flex items-center gap-4 py-4"
          >
            <div
              class="bg-elevated flex aspect-[4/3] w-20 shrink-0 items-center justify-center overflow-hidden rounded-lg"
            >
              <img
                v-if="item.imageUrl"
                :src="item.imageUrl"
                :alt="item.imageAlt || item.articleName"
                class="h-full w-full object-cover"
              />
              <UIcon
                v-else
                name="i-lucide-image-off"
                class="text-muted h-5 w-5"
              />
            </div>

            <div class="min-w-0 flex-1">
              <NuxtLink
                :to="`/articles/${item.articleId}`"
                class="truncate text-sm font-semibold hover:text-cyan-700"
              >
                {{ item.articleName }}
              </NuxtLink>
              <p class="text-muted text-xs">#{{ item.articleCode }}</p>
              <p class="text-sm">{{ formatMoney(item.unitPrice) }}</p>
            </div>

            <div class="flex items-center gap-2">
              <UButton
                color="neutral"
                variant="soft"
                icon="i-lucide-minus"
                square
                @click="cart.decrease(item.articleId)"
              />
              <UBadge color="primary" variant="subtle">
                {{ item.quantity }}
              </UBadge>
              <UButton
                color="primary"
                variant="soft"
                icon="i-lucide-plus"
                square
                @click="cart.increase(item.articleId)"
              />
              <UButton
                color="error"
                variant="ghost"
                icon="i-lucide-trash"
                square
                @click="cart.remove(item.articleId)"
              />
            </div>
          </li>
        </ul>
      </UCard>

      <UCard class="border-default bg-default border h-fit">
        <div class="space-y-3">
          <div class="flex items-center justify-between text-sm">
            <span class="text-muted">Stavke</span>
            <span>{{ cart.totalItems.value }}</span>
          </div>
          <div class="flex items-center justify-between text-sm">
            <span class="text-muted">Međuzbir</span>
            <span>{{ formatMoney(cart.totalPrice.value) }}</span>
          </div>
          <div class="flex items-center justify-between text-sm">
            <span class="text-muted">Dostava</span>
            <span>Besplatna</span>
          </div>
          <div class="border-default border-t pt-2">
            <div class="flex items-center justify-between font-semibold">
              <span>Ukupno</span>
              <span>{{ formatMoney(cart.totalPrice.value) }}</span>
            </div>
          </div>
        </div>

        <template #footer>
          <div class="space-y-2">
            <UButton color="primary" block @click="goToCheckout"
              >Nastavi na plaćanje</UButton
            >
            <UButton
              color="neutral"
              variant="soft"
              block
              @click="
                () => {
                  void cart.clear();
                }
              "
            >
              Isprazni korpu
            </UButton>
          </div>
        </template>
      </UCard>
    </div>
  </div>
</template>
