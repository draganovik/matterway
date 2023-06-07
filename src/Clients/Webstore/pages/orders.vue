<template>
  <div class="relative overflow-x-auto shadow-md sm:rounded-lg">
    <table class="w-full text-left text-sm text-gray-500 dark:text-gray-400">
      <thead class="text-xs uppercase text-gray-700 dark:text-gray-400">
        <tr>
          <th scope="col" class="bg-gray-50 px-6 py-3 dark:bg-gray-800">
            Referentni broj
          </th>
          <th scope="col" class="px-6 py-3">Broj artikala</th>
          <th
            v-if="sessionStore.getTokenData?.role != 'Customer'"
            scope="col"
            class="bg-gray-50 px-6 py-3 dark:bg-gray-800"
          >
            Kupac
          </th>
          <th scope="col" class="px-6 py-3">Ukupna cena</th>
        </tr>
      </thead>
      <tbody>
        <tr
          v-for="order in orders"
          class="border-b border-gray-200 dark:border-gray-700"
        >
          <th
            scope="row"
            class="whitespace-nowrap bg-gray-50 px-6 py-4 font-medium text-gray-900 dark:bg-gray-800 dark:text-white"
          >
            {{ order["referenceNumber"] }}
          </th>
          <td class="px-6 py-4">
            <ol class="list-decimal">
              <li class="mx-2 p-1" v-for="item in order.orderItems">
                <b
                  ><span class="text-blue-500">{{ item["quantity"] }} X </span>
                  {{ item["productName"] }}</b
                >
              </li>
            </ol>
          </td>
          <td
            v-if="sessionStore.getTokenData?.role != 'Customer'"
            class="bg-gray-50 px-6 py-4 dark:bg-gray-800"
          >
            {{ order.address?.receiverName }}
          </td>
          <td class="px-6 py-4">{{ formatMoney(order.total || 0) }}</td>
        </tr>
      </tbody>
    </table>
  </div>
</template>

<script lang="ts">
class Order {
  referenceNumber?: string;
  orderItems?: OrderItem[];
  address?: Address;
  total?: number;
}
class OrderItem {
  quantity?: number;
  productName?: string;
}
class Address {
  receiverName?: string;
}
</script>

<script setup lang="ts">
import { useSessionStore } from "~/store/session";
const orders: Ref<Order[]> = ref([]);
const sessionStore = useSessionStore();

const config = useRuntimeConfig();

const getOrders = async () => {
  const response = await request(
    `${config.public.ordering_api_base_url}/api/Orders?page=1&pageSize=100`,
    {},
  );
  orders.value = (await response.json()).data;
  console.log(orders.value);
};
onMounted(() => {
  getOrders();
});

definePageMeta({
  middleware: "auth",
});
</script>
