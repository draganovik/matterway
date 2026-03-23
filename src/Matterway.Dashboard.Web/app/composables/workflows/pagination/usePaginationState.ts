import { DEFAULT_PAGINATION_PAGE_SIZE } from "~/constants/pagination"
import { parseNumberOr } from "~/utils/numbers"

type NumericLike = string | number | null | undefined

type MetaLike = {
  totalCount?: NumericLike
  totalPages?: NumericLike
  currentPage?: NumericLike
  pageSize?: NumericLike
}

type UsePaginationStateOptions = {
  page?: number
  pageSize?: number
}

export function usePaginationState(options: UsePaginationStateOptions = {}) {
  const pagination = reactive({
    page: Math.max(1, options.page ?? 1),
    pageSize: Math.max(1, options.pageSize ?? DEFAULT_PAGINATION_PAGE_SIZE),
    totalCount: 0,
    totalPages: 1,
  })

  function resetTotals(totalCount = 0) {
    pagination.totalCount = Math.max(0, totalCount)
    pagination.totalPages = 1
  }

  function setSinglePageTotal(totalCount: number) {
    pagination.page = 1
    resetTotals(totalCount)
  }

  function applyMeta(
    meta: MetaLike | null | undefined,
    fallbackTotalCount = 0,
  ) {
    pagination.totalCount = parseNumberOr(meta?.totalCount, fallbackTotalCount)
    pagination.totalPages = Math.max(1, parseNumberOr(meta?.totalPages, 1))
    pagination.page = Math.max(
      1,
      parseNumberOr(meta?.currentPage, pagination.page),
    )
    pagination.pageSize = Math.max(
      1,
      parseNumberOr(meta?.pageSize, pagination.pageSize),
    )
  }

  function changePage(page: number) {
    pagination.page = page
  }

  function changePageSize(pageSize: number) {
    pagination.pageSize = pageSize
    pagination.page = 1
  }

  function searchWithPageReset(
    load: () => void | Promise<void>,
    shouldResetPage: () => boolean = () => pagination.page !== 1,
  ) {
    if (shouldResetPage()) {
      pagination.page = 1
      return
    }

    void load()
  }

  function watchPagination(
    load: () => void | Promise<void>,
    shouldLoad: () => boolean = () => true,
  ) {
    watch([() => pagination.page, () => pagination.pageSize], () => {
      if (!shouldLoad()) return
      void load()
    })
  }

  return {
    pagination,
    resetTotals,
    setSinglePageTotal,
    applyMeta,
    changePage,
    changePageSize,
    searchWithPageReset,
    watchPagination,
  }
}
