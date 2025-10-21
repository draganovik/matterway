<template>
  <div class="relative overflow-x-auto shadow-md sm:rounded-lg">
    <table
      v-if="orders.length > 0"
      class="w-full text-left text-sm text-gray-500 dark:text-gray-400"
    >
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
          v-for="order in orders.reverse()"
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
    <div v-else role="status">
      <svg
        aria-hidden="true"
        class="m-auto mt-12 h-10 w-10 animate-spin fill-blue-600 text-gray-200 dark:text-gray-600"
        viewBox="0 0 100 101"
        fill="none"
        xmlns="http://www.w3.org/2000/svg"
      >
        <path
          d="M100 50.5908C100 78.2051 77.6142 100.591 50 100.591C22.3858 100.591 0 78.2051 0 50.5908C0 22.9766 22.3858 0.59082 50 0.59082C77.6142 0.59082 100 22.9766 100 50.5908ZM9.08144 50.5908C9.08144 73.1895 27.4013 91.5094 50 91.5094C72.5987 91.5094 90.9186 73.1895 90.9186 50.5908C90.9186 27.9921 72.5987 9.67226 50 9.67226C27.4013 9.67226 9.08144 27.9921 9.08144 50.5908Z"
          fill="currentColor"
        />
        <path
          d="M93.9676 39.0409C96.393 38.4038 97.8624 35.9116 97.0079 33.5539C95.2932 28.8227 92.871 24.3692 89.8167 20.348C85.8452 15.1192 80.8826 10.7238 75.2124 7.41289C69.5422 4.10194 63.2754 1.94025 56.7698 1.05124C51.7666 0.367541 46.6976 0.446843 41.7345 1.27873C39.2613 1.69328 37.813 4.19778 38.4501 6.62326C39.0873 9.04874 41.5694 10.4717 44.0505 10.1071C47.8511 9.54855 51.7191 9.52689 55.5402 10.0491C60.8642 10.7766 65.9928 12.5457 70.6331 15.2552C75.2735 17.9648 79.3347 21.5619 82.5849 25.841C84.9175 28.9121 86.7997 32.2913 88.1811 35.8758C89.083 38.2158 91.5421 39.6781 93.9676 39.0409Z"
          fill="currentFill"
        />
      </svg>
      <span class="sr-only">Loading...</span>
    </div>
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
  await new Promise((resolve) => setTimeout(resolve, 2000));
  const response = await request(
    `${config.public.orderingApiBaseUrl}/api/v1.0/Orders?page=1&pageSize=100`,
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
