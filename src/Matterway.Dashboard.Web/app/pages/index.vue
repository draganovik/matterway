<script setup lang="ts">
import { useDashboardOverviewPage } from "~/composables/features/overview/useDashboardOverviewPage"

definePageMeta({
  title: "Overview",
})

const { sections } = useDashboardOverviewPage()
</script>

<template>
  <UDashboardPanel id="home">
    <template #header>
      <UDashboardNavbar title="Overview" :ui="{ right: 'gap-3' }">
        <template #leading>
          <UDashboardSidebarCollapse />
        </template>

        <template #right />
      </UDashboardNavbar>
    </template>

    <template #body>
      <div class="space-y-8">
        <div class="grid gap-5 xl:grid-cols-3">
          <article
            v-for="section in sections"
            :key="section.key"
            class="dashboard-panel-surface group relative rounded-2xl p-5 transition duration-200 hover:-translate-y-0.5 hover:shadow-md"
          >
            <div
              class="pointer-events-none absolute -top-8 right-0 h-36 w-36 rounded-full blur-3xl"
              :class="section.glowClass"
            />

            <div class="relative space-y-5">
              <div class="flex items-start justify-between gap-4">
                <div class="flex items-center gap-3">
                  <span
                    class="ring-default flex h-12 w-12 items-center justify-center rounded-2xl ring-1 ring-inset"
                    :class="section.iconClass"
                  >
                    <UIcon :name="section.icon" class="size-5" />
                  </span>
                  <div>
                    <p
                      class="text-muted text-[11px] tracking-[0.28em] uppercase"
                    >
                      {{ section.eyebrow }}
                    </p>
                    <h2 class="text-foreground text-xl font-semibold">
                      {{ section.label }}
                    </h2>
                  </div>
                </div>

                <UBadge color="neutral" variant="soft">
                  {{ section.featureCount }} tools
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
                  class="border-default/80 hover:border-primary/30 hover:bg-primary/5 flex items-center justify-between rounded-2xl border px-4 py-3 text-sm transition"
                >
                  <div class="flex items-center gap-3">
                    <span
                      class="h-2.5 w-2.5 rounded-full"
                      :class="section.dotClass"
                    />
                    <div>
                      <p class="text-foreground font-medium">
                        {{ feature.label }}
                      </p>
                      <p class="text-muted text-xs">
                        {{ section.label }} service
                      </p>
                    </div>
                  </div>

                  <UIcon
                    name="i-lucide-arrow-up-right"
                    class="text-muted size-4"
                  />
                </NuxtLink>
              </div>
            </div>
          </article>
        </div>

        <UCard
          v-if="!sections.length"
          class="dashboard-panel-surface !border-default rounded-2xl !border !shadow-sm !ring-0"
          :ui="{ body: 'p-5 sm:p-5' }"
        >
          <div class="space-y-2">
            <p class="text-foreground text-lg font-semibold">
              No authorized domains available
            </p>
            <p class="text-muted text-sm">
              Your current employee scope does not expose any dashboard sections
              yet. Update permissions, then refresh this page.
            </p>
          </div>
        </UCard>
      </div>
    </template>
  </UDashboardPanel>
</template>
