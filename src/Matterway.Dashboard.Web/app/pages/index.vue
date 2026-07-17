<script setup lang="ts">
import { useDashboardOverviewPage } from "~/composables/features/overview/useDashboardOverviewPage"

definePageMeta({
  title: "Pregled",
})

const { sections } = useDashboardOverviewPage()
</script>

<template>
  <UDashboardPanel id="home">
    <template #header>
      <UDashboardNavbar title="Pregled">
        <template #leading>
          <UDashboardSidebarCollapse />
        </template>
      </UDashboardNavbar>
    </template>

    <template #body>
      <div class="space-y-4">
        <div class="grid gap-4 xl:grid-cols-3">
          <article
            v-for="section in sections"
            :key="section.key"
            class="border-default bg-elevated rounded-lg border p-4 shadow-sm"
          >
            <div class="space-y-4">
              <div class="flex items-start justify-between gap-4">
                <div class="flex items-center gap-3">
                  <span
                    class="bg-muted text-toned flex h-11 w-11 items-center justify-center rounded-lg"
                  >
                    <UIcon :name="section.icon" class="size-5" />
                  </span>
                  <h2 class="text-foreground text-xl font-semibold">
                    {{ section.label }}
                  </h2>
                </div>

                <UBadge color="neutral" variant="soft">
                  {{ section.featureCount }} opcija
                </UBadge>
              </div>

              <p class="text-muted text-sm leading-6">
                {{ section.description }}
              </p>

              <div class="space-y-2">
                <NuxtLink
                  v-for="feature in section.features"
                  :key="feature.key"
                  :to="feature.route"
                  class="border-default hover:bg-muted flex items-center justify-between rounded-lg border px-4 py-3 text-sm transition-colors"
                >
                  <span class="text-foreground font-medium">
                    {{ feature.label }}
                  </span>

                  <UIcon
                    name="i-lucide-arrow-up-right"
                    class="text-muted size-4"
                  />
                </NuxtLink>
              </div>
            </div>
          </article>
        </div>

        <UCard v-if="!sections.length" class="!bg-elevated">
          <div class="space-y-2">
            <p class="text-foreground text-lg font-semibold">
              Nema dostupnih sekcija
            </p>
            <p class="text-muted text-sm">
              Trenutni nivo pristupa ovog naloga ne uključuje nijedan deo
              administracije. Ažurirajte dozvole pa osvežite stranicu.
            </p>
          </div>
        </UCard>
      </div>
    </template>
  </UDashboardPanel>
</template>
