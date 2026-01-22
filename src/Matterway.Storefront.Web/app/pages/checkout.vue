<script lang="ts" setup>
import { computed, onMounted, type Ref } from "vue";
import { useCartStore } from "@stores/cart";
import { useSessionStore } from "@stores/session";
import AddressModel from "#models/AddressModel";
import CardPaymentModel from "#models/CardPaymentModel";

const cart = useCartStore();
const session = useSessionStore();
const config = useRuntimeConfig();

const paymentData: Ref<CardPaymentModel> = ref(new CardPaymentModel());
const addressData: Ref<AddressModel> = ref(new AddressModel());
const isProcessingPayment = ref(false);

const cartItems = computed(() => cart.getCartItems);
const totalItems = computed(() => cart.getTotalItemCount);
const totalPrice = computed(() => cart.getTotalPrice);

const loadDefaultAddress = async () => {
  const customerId = session.getTokenData?.sub;
  if (!customerId) return;

  try {
    const response = await request(
      `${config.public.customersApiBaseUrl}/api/v1.0/Customers/${customerId}/Address`,
      { method: "GET" },
    );
    const address = await response.json();

    if (!addressData.value.street && address.addressLine1) {
      addressData.value.street = address.addressLine1;
    }
    if (!addressData.value.residence && address.addressLine2) {
      addressData.value.residence = address.addressLine2;
    }
    if (!addressData.value.city && address.city) {
      addressData.value.city = address.city;
    }
    if (!addressData.value.zipCode && address.zipCode) {
      addressData.value.zipCode = address.zipCode;
    }
    if (!addressData.value.country && address.country) {
      addressData.value.country = address.country;
    }
    if (!addressData.value.contactPhone && address.contactPhone) {
      addressData.value.contactPhone = address.contactPhone;
    }
  } catch {
    // Ignore missing default address or auth mismatch.
  }
};

const pay = async () => {
  if (!addressData.value.validate()) {
    return;
  }
  isProcessingPayment.value = true;
  try {
    const salesOrderResponse = await request(
      `${config.public.salesApiBaseUrl}/api/v1/Orders`,
      {
        method: "POST",
        body: JSON.stringify({
          customerId: session.getTokenData?.sub,
          type: "Ecommerce",
          deliveryInfo: {
            country: addressData.value.country || "Serbia",
            city: addressData.value.city,
            zipCode: addressData.value.zipCode,
            addressLine1: addressData.value.street,
            addressLine2: addressData.value.residence,
            contactPhone: addressData.value.contactPhone || undefined,
          },
        }),
      },
    );
    const salesOrder = await salesOrderResponse.json();

    const response = await fetch("/api/payments", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        ...paymentData.value,
        amount: salesOrder.totalAmount ?? totalPrice.value,
        ...addressData.value,
        orderId: salesOrder.id,
        userId: session.getTokenData?.sub,
      }),
    });
    if (response.ok) {
      await cart.clearCart();
      navigateTo("/orders");
    }
  } finally {
    isProcessingPayment.value = false;
  }
};

useHead({
  title: "Kupovina",
});

onMounted(() => {
  loadDefaultAddress();
});

definePageMeta({
  middleware: [
    function (to, from) {
      const cartCheck = useCartStore();
      if (!cartCheck.getTotalItemCount) {
        return navigateTo("/cart");
      }
    },
    "auth",
  ],
});
</script>

<template>
  <main class="mx-auto max-w-6xl space-y-12 px-4 pb-16 md:px-6">
    <section class="space-y-3">
      <span
        class="inline-flex items-center rounded-full border border-slate-200 px-3 py-1 text-xs font-semibold uppercase tracking-wide text-slate-500 dark:border-slate-700 dark:text-slate-300"
      >
        Završetak kupovine
      </span>
      <div class="flex flex-wrap items-center justify-between gap-4">
        <div class="space-y-2">
          <h1 class="text-3xl font-semibold text-slate-900 dark:text-white">
            Kupovina
          </h1>
          <p class="max-w-2xl text-sm text-slate-500 dark:text-slate-400">
            Unesite podatke o dostavi i plaćanju kako biste završili kupovinu.
          </p>
        </div>
        <span
          class="inline-flex items-center rounded-full border border-blue-100 bg-blue-50 px-4 py-2 text-sm font-medium text-blue-600 dark:border-blue-900/40 dark:bg-blue-900/20 dark:text-blue-200"
        >
          {{ totalItems }} {{ totalItems === 1 ? "proizvod" : "proizvoda" }}
        </span>
      </div>
    </section>

    <div class="grid gap-8 lg:grid-cols-[minmax(0,2fr)_minmax(0,1fr)]">
      <div class="space-y-8">
        <!-- Order Items -->
        <section
          class="rounded-3xl border border-slate-200 bg-white p-6 shadow-xs dark:border-slate-700 dark:bg-slate-800"
        >
          <header class="mb-6 flex items-center justify-between">
            <h2 class="text-lg font-semibold text-slate-900 dark:text-white">
              Vaša narudžbina
            </h2>
            <span
              class="text-xs font-medium uppercase tracking-wide text-slate-400 dark:text-slate-500"
            >
              {{ totalItems }} {{ totalItems === 1 ? "artikal" : "artikala" }}
            </span>
          </header>
          <CartItemsList :items="cartItems" />
        </section>

        <!-- Delivery Address -->
        <section
          class="rounded-3xl border border-slate-200 bg-white p-6 shadow-xs dark:border-slate-700 dark:bg-slate-800"
        >
          <header class="mb-6">
            <h2 class="text-lg font-semibold text-slate-900 dark:text-white">
              Podaci za dostavu
            </h2>
            <p class="mt-1 text-sm text-slate-500 dark:text-slate-400">
              Unesite adresu na koju želite da dostavimo vašu narudžbinu.
            </p>
          </header>
          <CheckoutAddressForm v-model="addressData" />
        </section>
      </div>
      <CheckoutPaymentForm
        v-model="paymentData"
        :total-items="totalItems"
        :total-price="totalPrice"
        :loading="isProcessingPayment"
        @submit="pay"
      />
    </div>
  </main>
</template>
