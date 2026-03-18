<script setup lang="ts">
import { useCatalogDiscountsPage } from "~/composables/features/catalog/useCatalogDiscountsPage"

definePageMeta({
  title: "Popusti",
  service: "catalog",
  permissions: ["observer", "operator", "manager"],
})

const {
  canEdit,
  listState,
  submitState,
  deleteState,
  discountFilter,
  listPage,
  listPageSize,
  selectedKey,
  selectedDiscount,
  isCreateMode,
  form,
  selectedArticleCodes,
  filteredDiscountCount,
  filteredDiscountPages,
  visibleDiscounts,
  updateDiscountFilter,
  searchDiscounts,
  changeListPage,
  changeListPageSize,
  beginCreate,
  selectDiscount,
  saveDiscount,
  removeDiscount,
} = useCatalogDiscountsPage()
</script>

<template>
  <UDashboardPanel
    id="catalog-discounts"
    :ui="{ body: 'py-3 sm:py-4 lg:py-6 min-h-0 overflow-hidden' }"
  >
    <template #header>
      <UDashboardNavbar title="Popusti">
        <template #leading>
          <UDashboardSidebarCollapse />
        </template>
      </UDashboardNavbar>
    </template>

    <template #body>
      <div class="h-full min-h-0">
        <div class="flex h-full min-h-0 flex-col gap-4 overflow-hidden">
          <div
            class="flex shrink-0 flex-wrap items-center justify-between gap-3"
          >
            <div>
              <h2 class="text-foreground text-base font-semibold">
                Upravljanje popustima
              </h2>
              <p class="text-muted text-sm">
                Izaberite postojeći popust sa liste ili kreirajte novi kod,
                pa zatim ažurirajte povezane artikle i period važenja.
              </p>
            </div>
            <UButton
              color="primary"
              :disabled="!canEdit || isCreateMode"
              @click="beginCreate"
            >
              Novi popust
            </UButton>
          </div>

          <EntitiesSplitView
            class="min-h-0 flex-1"
            list-class="overflow-hidden"
            detail-class="overflow-y-auto"
          >
            <template #list>
              <CatalogDiscountsListView
                :items="visibleDiscounts"
                :selected-id="selectedKey"
                :filter="discountFilter"
                :loading="listState.loading"
                :error="listState.error"
                :empty-message="listState.empty"
                :page="listPage"
                :page-size="listPageSize"
                :total-count="filteredDiscountCount"
                :total-pages="filteredDiscountPages"
                @update:filter="updateDiscountFilter"
                @search="searchDiscounts"
                @update:page="changeListPage"
                @update:page-size="changeListPageSize"
                @select="selectDiscount"
              />
            </template>

            <template #detail>
              <div class="grid gap-5">
                <div class="space-y-1">
                  <h3 class="text-foreground text-base font-semibold">
                    {{ selectedDiscount ? "Izmena popusta" : "Novi popust" }}
                  </h3>
                  <p class="text-muted text-sm">
                    PUT metoda se koristi za čuvanje izmena. Brisanje uklanja
                    sve redove za izabrani kod.
                  </p>
                </div>

                <div class="grid gap-4 md:grid-cols-2">
                  <UFormField
                    label="Kod"
                    required
                    help="3-50 karaktera, velika slova, cifre, donja crta ili crtica."
                  >
                    <UInput
                      v-model="form.code"
                      placeholder="SPRING25"
                      :disabled="!canEdit"
                      class="w-full"
                    />
                  </UFormField>

                  <UFormField
                    label="Procenat"
                    required
                    help="Opseg decimalne vrednosti: 0,01 do 1."
                  >
                    <UInputNumber
                      v-model="form.percentage"
                      orientation="vertical"
                      :min="0.01"
                      :max="1"
                      :step="0.01"
                      variant="outline"
                      placeholder="0.15"
                      :disabled="!canEdit"
                      class="w-full"
                      :ui="{ root: 'w-full', base: 'w-full text-left' }"
                    />
                  </UFormField>

                  <UFormField label="Važi od" required>
                    <UInput
                      v-model="form.validFrom"
                      type="datetime-local"
                      placeholder="YYYY-MM-DDTHH:mm"
                      :disabled="!canEdit"
                      class="w-full"
                    />
                  </UFormField>

                  <UFormField label="Važi do">
                    <UInput
                      v-model="form.validTo"
                      type="datetime-local"
                      placeholder="YYYY-MM-DDTHH:mm"
                      :disabled="!canEdit"
                      class="w-full"
                    />
                  </UFormField>
                </div>

                <CatalogDiscountsArticleSelectionPanel
                  v-model:model-value="selectedArticleCodes"
                  :can-edit="canEdit"
                />

                <div class="flex flex-wrap items-center gap-3">
                  <UButton
                    color="primary"
                    :loading="submitState.loading"
                    :disabled="!canEdit"
                    @click="saveDiscount"
                  >
                    {{
                      submitState.loading
                        ? "Čuvanje popusta"
                        : "Sačuvaj popust"
                    }}
                  </UButton>
                  <UButton
                    color="error"
                    variant="outline"
                    :loading="deleteState.loading"
                    :disabled="!canEdit"
                    @click="removeDiscount"
                  >
                    {{
                      deleteState.loading
                        ? "Brisanje popusta"
                        : "Obriši popust"
                    }}
                  </UButton>
                </div>

                <StatusMessages
                  :error="submitState.error || deleteState.error"
                  :success="submitState.success || deleteState.success"
                />
              </div>
            </template>
          </EntitiesSplitView>
        </div>
      </div>
    </template>
  </UDashboardPanel>
</template>
