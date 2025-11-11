<script lang="ts" setup>
import type AddressModel from "~/models/AddressModel";

const address = defineModel<AddressModel>({ required: true });
</script>

<template>
  <div class="space-y-5">
    <div>
      <label
        for="name"
        class="mb-2 block text-sm font-medium text-slate-900 dark:text-white"
      >
        Ime primaoca
      </label>
      <input
        type="text"
        id="name"
        autocomplete="name"
        v-model="address.receiverName"
        placeholder="Ime i prezime"
        required
        class="block w-full rounded-xl border border-slate-300 bg-slate-50 px-4 py-2.5 text-sm text-slate-900 transition focus:border-blue-500 focus:ring-2 focus:ring-blue-500/20 dark:border-slate-600 dark:bg-slate-700 dark:text-white dark:placeholder-slate-400 dark:focus:border-blue-500 dark:focus:ring-blue-500/20"
      />
    </div>
    <div>
      <label
        for="residence"
        class="mb-2 block text-sm font-medium text-slate-900 dark:text-white"
      >
        Stambena jedinica
      </label>
      <input
        type="text"
        id="residence"
        autocomplete="address-line2"
        v-model="address.residence"
        placeholder="Stan / ulaz / broj"
        required
        class="block w-full rounded-xl border border-slate-300 bg-slate-50 px-4 py-2.5 text-sm text-slate-900 transition focus:border-blue-500 focus:ring-2 focus:ring-blue-500/20 dark:border-slate-600 dark:bg-slate-700 dark:text-white dark:placeholder-slate-400 dark:focus:border-blue-500 dark:focus:ring-blue-500/20"
      />
    </div>
    <div class="grid gap-5 md:grid-cols-2">
      <div>
        <label
          for="street"
          class="mb-2 block text-sm font-medium text-slate-900 dark:text-white"
        >
          Ulica
        </label>
        <input
          type="text"
          id="street"
          autocomplete="street-address"
          v-model="address.street"
          placeholder="Ulica i broj"
          required
          class="block w-full rounded-xl border border-slate-300 bg-slate-50 px-4 py-2.5 text-sm text-slate-900 transition focus:border-blue-500 focus:ring-2 focus:ring-blue-500/20 dark:border-slate-600 dark:bg-slate-700 dark:text-white dark:placeholder-slate-400 dark:focus:border-blue-500 dark:focus:ring-blue-500/20"
        />
      </div>
      <div>
        <label
          for="city"
          class="mb-2 block text-sm font-medium text-slate-900 dark:text-white"
        >
          Grad
        </label>
        <input
          type="text"
          id="city"
          autocomplete="address-level2"
          v-model="address.city"
          placeholder="Grad"
          required
          class="block w-full rounded-xl border border-slate-300 bg-slate-50 px-4 py-2.5 text-sm text-slate-900 transition focus:border-blue-500 focus:ring-2 focus:ring-blue-500/20 dark:border-slate-600 dark:bg-slate-700 dark:text-white dark:placeholder-slate-400 dark:focus:border-blue-500 dark:focus:ring-blue-500/20"
        />
      </div>
    </div>
    <div class="grid gap-5 md:grid-cols-2">
      <div>
        <label
          for="zipCode"
          class="mb-2 block text-sm font-medium text-slate-900 dark:text-white"
        >
          Poštanski broj
        </label>
        <input
          type="text"
          id="zipCode"
          autocomplete="postal-code"
          v-model="address.zipCode"
          placeholder="11000"
          required
          class="block w-full rounded-xl border border-slate-300 bg-slate-50 px-4 py-2.5 text-sm text-slate-900 transition focus:border-blue-500 focus:ring-2 focus:ring-blue-500/20 dark:border-slate-600 dark:bg-slate-700 dark:text-white dark:placeholder-slate-400 dark:focus:border-blue-500 dark:focus:ring-blue-500/20"
        />
      </div>
      <div>
        <label
          for="note"
          class="mb-2 block text-sm font-medium text-slate-900 dark:text-white"
        >
          Napomena
        </label>
        <input
          type="text"
          id="note"
          v-model="address.note"
          placeholder="Opcionalno"
          class="block w-full rounded-xl border border-slate-300 bg-slate-50 px-4 py-2.5 text-sm text-slate-900 transition focus:border-blue-500 focus:ring-2 focus:ring-blue-500/20 dark:border-slate-600 dark:bg-slate-700 dark:text-white dark:placeholder-slate-400 dark:focus:border-blue-500 dark:focus:ring-blue-500/20"
        />
      </div>
    </div>
  </div>
</template>
