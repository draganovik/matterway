<script lang="ts" setup>
import logoUrl from "~/assets/brand/logo.svg?url";

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
            <svg class="h-4 w-4" fill="currentColor" viewBox="0 0 448 512">
              <path
                d="M100.3 448l-92.9 0 0-299.1 92.9 0 0 299.1zM53.8 108.1C24.1 108.1 0 83.5 0 53.8 0 39.5 5.7 25.9 15.8 15.8s23.8-15.8 38-15.8 27.9 5.7 38 15.8 15.8 23.8 15.8 38c0 29.7-24.1 54.3-53.8 54.3zM447.9 448l-92.7 0 0-145.6c0-34.7-.7-79.2-48.3-79.2-48.3 0-55.7 37.7-55.7 76.7l0 148.1-92.8 0 0-299.1 89.1 0 0 40.8 1.3 0c12.4-23.5 42.7-48.3 87.9-48.3 94 0 111.3 61.9 111.3 142.3l0 164.3-.1 0z"
              />
            </svg>
          </a>
          <a
            href="https://instagram.com"
            target="_blank"
            rel="noopener"
            class="inline-flex h-9 w-9 items-center justify-center rounded-full bg-slate-100 text-slate-500 transition hover:bg-pink-500 hover:text-white dark:bg-slate-800 dark:text-slate-300 dark:hover:bg-pink-500"
          >
            <span class="sr-only">Instagram</span>
            <svg class="h-4 w-4" fill="currentColor" viewBox="0 0 448 512">
              <path
                d="M224.3 141a115 115 0 1 0 -.6 230 115 115 0 1 0 .6-230zm-.6 40.4a74.6 74.6 0 1 1 .6 149.2 74.6 74.6 0 1 1 -.6-149.2zm93.4-45.1a26.8 26.8 0 1 1 53.6 0 26.8 26.8 0 1 1 -53.6 0zm129.7 27.2c-1.7-35.9-9.9-67.7-36.2-93.9-26.2-26.2-58-34.4-93.9-36.2-37-2.1-147.9-2.1-184.9 0-35.8 1.7-67.6 9.9-93.9 36.1s-34.4 58-36.2 93.9c-2.1 37-2.1 147.9 0 184.9 1.7 35.9 9.9 67.7 36.2 93.9s58 34.4 93.9 36.2c37 2.1 147.9 2.1 184.9 0 35.9-1.7 67.7-9.9 93.9-36.2 26.2-26.2 34.4-58 36.2-93.9 2.1-37 2.1-147.8 0-184.8zM399 388c-7.8 19.6-22.9 34.7-42.6 42.6-29.5 11.7-99.5 9-132.1 9s-102.7 2.6-132.1-9c-19.6-7.8-34.7-22.9-42.6-42.6-11.7-29.5-9-99.5-9-132.1s-2.6-102.7 9-132.1c7.8-19.6 22.9-34.7 42.6-42.6 29.5-11.7 99.5-9 132.1-9s102.7-2.6 132.1 9c19.6 7.8 34.7 22.9 42.6 42.6 11.7 29.5 9 99.5 9 132.1s2.7 102.7-9 132.1z"
              />
            </svg>
          </a>
          <a
            href="https://facebook.com"
            target="_blank"
            rel="noopener"
            class="inline-flex h-9 w-9 items-center justify-center rounded-full bg-slate-100 text-slate-500 transition hover:bg-blue-500 hover:text-white dark:bg-slate-800 dark:text-slate-300 dark:hover:bg-blue-500"
          >
            <span class="sr-only">Facebook</span>
            <svg class="h-4 w-4" fill="currentColor" viewBox="0 0 24 24">
              <path
                d="M13.3 22V12h3.2l.5-3.7h-3.7V6.2c0-1 .3-1.6 1.7-1.6h2V1.4C16.6 1.2 15.3 1 14 1c-3 0-5 1.8-5 5.1v2.2H6v3.6h3v10H13.3z"
              />
            </svg>
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
