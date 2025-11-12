<script lang="ts" setup>
import logoUrl from "@assets/brand/logo.svg?url";

const year = new Date().getFullYear();

const quickLinks = [
  { label: "Naslovna", to: "/" },
  { label: "Proizvodi", to: "/products" },
  { label: "Kontakt", to: "/#kontakt" },
  { label: "Politika privatnosti", to: "/privacy" },
];

const contactInfo = [
  {
    label: "E-mail",
    value: "hello@matterway.com",
    href: "mailto:hello@matterway.com",
  },
  { label: "Telefon", value: "+381 11 123 456", href: "tel:+38111123456" },
  {
    label: "Showroom",
    value: "Bulevar Oslobođenja 32, Novi Sad",
    href: "https://maps.google.com/?q=Bulevar+Oslobođenja+32,+Novi+Sad",
  },
];
</script>

<template>
  <footer
    class="border-t border-slate-200 bg-white pb-10 pt-14 dark:border-slate-700 dark:bg-slate-900"
  >
    <div
      class="mx-auto flex max-w-6xl flex-col gap-12 px-4 md:flex-row md:justify-between md:px-6"
    >
      <div class="max-w-md space-y-5">
        <NuxtLink
          to="/"
          class="flex items-center gap-3 transition hover:opacity-90"
        >
          <img :src="logoUrl" alt="Matterway logo" class="h-9 w-9" />
          <span class="text-xl font-semibold text-slate-900 dark:text-white"
            >Matterway</span
          >
        </NuxtLink>
        <p class="text-sm leading-relaxed text-slate-500 dark:text-slate-400">
          Povezujemo rasvetu, energiju i sigurnost u inteligentni sistem koji
          radi u ritmu vašeg života. Dizajnirano i razvijeno u Srbiji.
        </p>
        <div class="flex gap-3">
          <a
            href="https://linkedin.com"
            target="_blank"
            rel="noopener"
            class="inline-flex h-9 w-9 items-center justify-center rounded-full bg-slate-100 text-slate-500 transition hover:bg-blue-600 hover:text-white dark:bg-slate-800 dark:text-slate-300 dark:hover:bg-blue-500"
          >
            <span class="sr-only">LinkedIn</span>
            <Icon name="mdi:linkedin" class="text-base" aria-hidden="true" />
          </a>
          <a
            href="https://instagram.com"
            target="_blank"
            rel="noopener"
            class="inline-flex h-9 w-9 items-center justify-center rounded-full bg-slate-100 text-slate-500 transition hover:bg-pink-500 hover:text-white dark:bg-slate-800 dark:text-slate-300 dark:hover:bg-pink-500"
          >
            <span class="sr-only">Instagram</span>
            <Icon name="mdi:instagram" class="text-base" aria-hidden="true" />
          </a>
          <a
            href="https://facebook.com"
            target="_blank"
            rel="noopener"
            class="inline-flex h-9 w-9 items-center justify-center rounded-full bg-slate-100 text-slate-500 transition hover:bg-blue-500 hover:text-white dark:bg-slate-800 dark:text-slate-300 dark:hover:bg-blue-500"
          >
            <span class="sr-only">Facebook</span>
            <Icon name="mdi:facebook" class="text-base" aria-hidden="true" />
          </a>
        </div>
      </div>

      <div class="grid flex-1 gap-10 sm:grid-cols-2 lg:grid-cols-3">
        <div class="space-y-3">
          <h3
            class="text-sm font-semibold uppercase tracking-wide text-slate-500 dark:text-slate-400"
          >
            Brzi linkovi
          </h3>
          <ul class="space-y-2 text-sm text-slate-600 dark:text-slate-300">
            <li v-for="link in quickLinks" :key="link.to">
              <NuxtLink
                :to="link.to"
                class="hover:text-blue-600 hover:underline dark:hover:text-blue-300"
              >
                {{ link.label }}
              </NuxtLink>
            </li>
          </ul>
        </div>

        <div class="space-y-3">
          <h3
            class="text-sm font-semibold uppercase tracking-wide text-slate-500 dark:text-slate-400"
          >
            Kontakt
          </h3>
          <ul class="space-y-2 text-sm text-slate-600 dark:text-slate-300">
            <li v-for="entry in contactInfo" :key="entry.label">
              <p class="text-xs uppercase tracking-wide text-slate-400">
                {{ entry.label }}
              </p>
              <a
                :href="entry.href"
                class="font-medium text-slate-700 hover:text-blue-600 dark:text-slate-200 dark:hover:text-blue-300"
              >
                {{ entry.value }}
              </a>
            </li>
          </ul>
        </div>
      </div>
    </div>

    <div
      class="mt-12 border-t border-slate-200 pt-6 text-sm text-slate-400 dark:border-slate-700 dark:text-slate-500"
    >
      <div
        class="mx-auto flex max-w-6xl flex-col items-center justify-between gap-4 px-4 md:flex-row md:px-6"
      >
        <p>© {{ year }} Matterway. Sva prava zadržana.</p>
        <p class="text-xs">
          Dizajn i razvoj:
          <a
            href="https://draganovik.com/"
            target="_blank"
            rel="noopener"
            class="font-medium text-slate-500 hover:text-blue-600 dark:text-slate-300 dark:hover:text-blue-300"
          >
            Mladen Draganović
          </a>
        </p>
      </div>
    </div>
  </footer>
</template>
