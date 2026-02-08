<script setup lang="ts">
import { useCatalogApi } from '~/composables/useCatalogApi'
import type { QueryArticleResponse, QueryDiscountResponse } from '~/types/catalog'
import { useRequestState } from '~/composables/useRequestState'
import { useAuthSession } from '~/composables/useAuthSession'
import { normalizeCode } from '~/utils/normalization'
import { parseNumberOr } from '~/utils/numbers'

type DiscountListItem = {
  key: string
  code: string
  percentage: number | string
  validFrom: string
  validTo?: string | null
  articleIds: string[]
}

type DiscountForm = {
  code: string
  percentage: number | string
  validFrom: string
  validTo: string
}

const auth = useAuthSession()
const api = useCatalogApi()

const canEdit = computed(() => auth.hasPermission('catalog', 'operator'))

const listState = useRequestState({ empty: 'No discounts found.' })
const submitState = useRequestState()
const deleteState = useRequestState()

const discounts = ref<DiscountListItem[]>([])
const searchInput = ref('')
const activeSearch = ref('')

const selectedKey = ref<string | null>(null)
const selectedDiscount = ref<DiscountListItem | null>(null)
const isCreateMode = computed(() => !selectedDiscount.value)

const form = ref<DiscountForm>({
  code: '',
  percentage: '',
  validFrom: getDefaultDateTimeLocal(),
  validTo: ''
})
const selectedArticleIds = ref<string[]>([])

function getDefaultDateTimeLocal() {
  const now = new Date()
  const year = now.getFullYear()
  const month = String(now.getMonth() + 1).padStart(2, '0')
  const day = String(now.getDate()).padStart(2, '0')
  const hour = String(now.getHours()).padStart(2, '0')
  const minute = String(now.getMinutes()).padStart(2, '0')
  return `${year}-${month}-${day}T${hour}:${minute}`
}

function toIsoDateTime(value: string) {
  const parsed = new Date(value)
  if (Number.isNaN(parsed.getTime())) return null
  return parsed.toISOString()
}

function toLocalDateTimeInput(value?: string | null) {
  if (!value) return ''
  const parsed = new Date(value)
  if (Number.isNaN(parsed.getTime())) return ''
  const year = parsed.getFullYear()
  const month = String(parsed.getMonth() + 1).padStart(2, '0')
  const day = String(parsed.getDate()).padStart(2, '0')
  const hour = String(parsed.getHours()).padStart(2, '0')
  const minute = String(parsed.getMinutes()).padStart(2, '0')
  return `${year}-${month}-${day}T${hour}:${minute}`
}

function sortDiscounts(items: DiscountListItem[]) {
  return [...items].sort((a, b) => {
    const leftCode = (a.code || '').toLowerCase()
    const rightCode = (b.code || '').toLowerCase()
    if (leftCode !== rightCode) return leftCode.localeCompare(rightCode)
    return (b.validFrom || '').localeCompare(a.validFrom || '')
  })
}

function resetMessages() {
  submitState.error = ''
  submitState.success = ''
  deleteState.error = ''
  deleteState.success = ''
}

function beginCreate() {
  selectedKey.value = null
  selectedDiscount.value = null
  form.value = {
    code: '',
    percentage: '',
    validFrom: getDefaultDateTimeLocal(),
    validTo: ''
  }
  selectedArticleIds.value = []
  resetMessages()
}

function applyDiscountToEditor(discount: DiscountListItem) {
  selectedKey.value = discount.key
  selectedDiscount.value = discount
  form.value = {
    code: discount.code,
    percentage: discount.percentage,
    validFrom: toLocalDateTimeInput(discount.validFrom) || getDefaultDateTimeLocal(),
    validTo: toLocalDateTimeInput(discount.validTo)
  }
  selectedArticleIds.value = [...discount.articleIds]
  resetMessages()
}

function selectDiscount(key: string) {
  const discount = discounts.value.find(item => item.key === key)
  if (!discount) return
  applyDiscountToEditor(discount)
}

function applySearch() {
  activeSearch.value = searchInput.value.trim().toLowerCase()
}

const filteredDiscounts = computed(() => {
  const search = activeSearch.value
  if (!search) return discounts.value
  return discounts.value.filter(item =>
    item.code.toLowerCase().includes(search)
    || item.validFrom.toLowerCase().includes(search)
    || `${item.percentage}`.toLowerCase().includes(search)
  )
})

async function loadDiscountsFromArticlesFallback() {
  const pageSize = 100
  let page = 1
  const articleRows: QueryArticleResponse[] = []

  while (true) {
    const result = await api.queryArticles({ page, pageSize })

    if (!result.ok) {
      listState.error = result.error || 'Unable to load discounts.'
      return
    }

    if (result.status === 204 || !result.data) break

    articleRows.push(...(result.data.data || []))
    const totalPages = Math.max(1, parseNumberOr(result.data.meta?.totalPages, 1))
    if (page >= totalPages) break
    page += 1
  }

  const grouped = new Map<string, DiscountListItem>()

  for (const article of articleRows) {
    if (!article.discount || !article.id) continue
    const code = normalizeCode((article.discount.code ?? '').toString())
    const percentage = article.discount.percentage
    const validFrom = article.discount.validFrom
    const validTo = article.discount.validTo ?? null
    const key = code || `${percentage}|${validFrom}|${validTo || ''}`

    const existing = grouped.get(key)
    if (!existing) {
      grouped.set(key, {
        key,
        code,
        percentage,
        validFrom,
        validTo,
        articleIds: [article.id]
      })
      continue
    }
    if (!existing.articleIds.includes(article.id)) {
      existing.articleIds.push(article.id)
    }
  }

  discounts.value = sortDiscounts(Array.from(grouped.values()))
}

function buildDiscountList(rows: QueryDiscountResponse[]) {
  const grouped = new Map<string, DiscountListItem>()

  for (const row of rows) {
    const code = normalizeCode((row.code ?? '').toString())
    const percentage = row.percentage ?? ''
    const validFrom = row.validFrom ?? ''
    const validTo = row.validTo ?? null
    const key = code || `${percentage}|${validFrom}|${validTo || ''}`
    const articleId = row.articleId?.toString() ?? ''

    const existing = grouped.get(key)
    if (!existing) {
      grouped.set(key, {
        key,
        code,
        percentage,
        validFrom,
        validTo,
        articleIds: articleId ? [articleId] : []
      })
      continue
    }
    if (articleId && !existing.articleIds.includes(articleId)) {
      existing.articleIds.push(articleId)
    }
  }

  return sortDiscounts(Array.from(grouped.values()))
}

async function loadDiscounts() {
  listState.loading = true
  listState.error = ''

  const result = await api.queryDiscounts()

  if (result.ok) {
    discounts.value = buildDiscountList(result.data || [])
    listState.loading = false
  } else if (result.status === 404 || result.status === 405) {
    await loadDiscountsFromArticlesFallback()
    listState.loading = false
  } else {
    listState.error = result.error || 'Unable to load discounts.'
    discounts.value = []
    listState.loading = false
  }

  if (!selectedKey.value) return

  const selected = discounts.value.find(item => item.key === selectedKey.value) || null
  if (!selected) {
    beginCreate()
    return
  }

  applyDiscountToEditor(selected)
}

function buildPayload() {
  const code = normalizeCode(form.value.code)
  const percentage = Number(form.value.percentage)
  const validFrom = toIsoDateTime(form.value.validFrom)
  const validToInput = form.value.validTo.trim()
  const validTo = validToInput ? toIsoDateTime(validToInput) : null

  if (!code) {
    submitState.error = 'Code is required.'
    return null
  }
  if (!Number.isFinite(percentage) || percentage < 0.01 || percentage > 1) {
    submitState.error = 'Percentage must be between 0.01 and 1.'
    return null
  }
  if (!validFrom) {
    submitState.error = 'Valid From must be a valid date-time.'
    return null
  }
  if (validToInput && !validTo) {
    submitState.error = 'Valid To must be a valid date-time.'
    return null
  }
  if (validTo && new Date(validTo) < new Date(validFrom)) {
    submitState.error = 'Valid To must be greater than or equal to Valid From.'
    return null
  }

  const articleIds = [...new Set(selectedArticleIds.value)]
  if (!articleIds.length) {
    submitState.error = 'Select at least one article.'
    return null
  }

  return {
    code,
    payload: {
      percentage,
      validFrom,
      validTo,
      articleIds
    }
  }
}

async function saveDiscount() {
  resetMessages()
  if (!canEdit.value) return

  const built = buildPayload()
  if (!built) return

  submitState.loading = true
  const result = await api.updateDiscount(built.code, built.payload)
  submitState.loading = false

  if (!result.ok) {
    submitState.error = result.error || 'Unable to save discount.'
    return
  }

  const key = built.code
  const next: DiscountListItem = {
    key,
    code: built.code,
    percentage: built.payload.percentage,
    validFrom: built.payload.validFrom,
    validTo: built.payload.validTo,
    articleIds: built.payload.articleIds
  }

  discounts.value = sortDiscounts([
    ...discounts.value.filter(item => item.key !== key && item.code !== built.code),
    next
  ])

  applyDiscountToEditor(next)
  submitState.success = `Discount ${built.code} saved.`
}

async function removeDiscount() {
  deleteState.error = ''
  deleteState.success = ''
  if (!canEdit.value) return

  const code = normalizeCode(form.value.code)
  if (!code) {
    deleteState.error = 'Provide a code to delete.'
    return
  }

  deleteState.loading = true
  const result = await api.deleteDiscount(code)
  deleteState.loading = false

  if (!result.ok) {
    deleteState.error = result.error || 'Unable to delete discount.'
    return
  }

  discounts.value = discounts.value.filter(item => item.code !== code && item.key !== code)
  beginCreate()
  deleteState.success = result.data?.message || `Discount ${code} removed.`
}

onMounted(() => {
  beginCreate()
  void loadDiscounts()
})
</script>

<template>
  <div class="flex h-full min-h-0 flex-col gap-4 overflow-hidden">
    <div class="shrink-0 flex flex-wrap items-center justify-between gap-3">
      <div>
        <h2 class="text-base font-semibold text-foreground">
          Manage Discounts
        </h2>
        <p class="text-sm text-muted">
          Pick an existing discount from the list or create a new code, then update article links and validity.
        </p>
      </div>
      <UButton
        color="primary"
        :disabled="!canEdit || isCreateMode"
        @click="beginCreate"
      >
        Create New
      </UButton>
    </div>

    <EntitiesSplitView
      class="min-h-0 flex-1"
      list-class="overflow-y-auto"
      detail-class="overflow-y-auto"
    >
      <template #list>
        <div class="grid gap-3">
          <UInput
            v-model="searchInput"
            placeholder="Search existing discounts by code or value."
            size="lg"
            class="w-full"
            @keydown.enter.prevent="applySearch"
          />
          <UButton
            color="primary"
            :loading="listState.loading"
            @click="applySearch"
          >
            Search
          </UButton>
        </div>

        <div class="mt-3 grid gap-2">
          <StatusMessages
            v-if="listState.error || listState.loading || !filteredDiscounts.length"
            :error="listState.error"
            :loading="listState.loading ? 'Loading discounts.' : false"
            :empty="!listState.loading && !listState.error && !filteredDiscounts.length ? listState.empty : false"
          />
          <div
            v-else
            class="grid gap-2"
          >
            <button
              v-for="discount in filteredDiscounts"
              :key="discount.key"
              type="button"
              class="w-full rounded-xl border px-4 py-3 text-left transition"
              :class="selectedKey === discount.key
                ? 'border-primary/40 bg-primary/5'
                : 'border-transparent bg-background hover:border-default hover:bg-muted/40'"
              @click="selectDiscount(discount.key)"
            >
              <CatalogDiscountsDiscountListItem :item="discount" />
            </button>
          </div>
        </div>
      </template>

      <template #detail>
        <div class="grid gap-5">
          <div class="space-y-1">
            <h3 class="text-base font-semibold text-foreground">
              {{ selectedDiscount ? 'Edit Discount' : 'Create Discount' }}
            </h3>
            <p class="text-sm text-muted">
              PUT is used for save operations. Delete removes all rows by code.
            </p>
          </div>

          <div class="grid gap-4 md:grid-cols-2">
            <UFormField
              label="Code"
              required
              help="3-50 chars, uppercase letters, numbers, underscore, hyphen."
            >
              <UInput
                v-model="form.code"
                placeholder="SPRING25"
                :disabled="!canEdit"
                class="w-full"
              />
            </UFormField>

            <UFormField
              label="Percentage"
              required
              help="Decimal range: 0.01 to 1."
            >
              <UInput
                v-model="form.percentage"
                type="number"
                min="0.01"
                max="1"
                step="0.01"
                placeholder="0.15"
                :disabled="!canEdit"
                class="w-full"
              />
            </UFormField>

            <UFormField
              label="Valid From"
              required
            >
              <UInput
                v-model="form.validFrom"
                type="datetime-local"
                :disabled="!canEdit"
                class="w-full"
              />
            </UFormField>

            <UFormField label="Valid To">
              <UInput
                v-model="form.validTo"
                type="datetime-local"
                :disabled="!canEdit"
                class="w-full"
              />
            </UFormField>
          </div>

          <CatalogDiscountsArticlesView
            v-model:model-value="selectedArticleIds"
            :can-edit="canEdit"
          />

          <div class="flex flex-wrap items-center gap-3">
            <UButton
              color="primary"
              :loading="submitState.loading"
              :disabled="!canEdit"
              @click="saveDiscount"
            >
              Save Discount
            </UButton>
            <UButton
              color="error"
              variant="outline"
              :loading="deleteState.loading"
              :disabled="!canEdit"
              @click="removeDiscount"
            >
              Delete Discount
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
</template>
