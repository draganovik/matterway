import { DEFAULT_PAGINATION_PAGE_SIZE } from "~/constants/pagination"
import type { PaginationMeta } from "~/types/common/pagination"

export function usePaginationState() {
  const pagination = reactive({
    page: 1,
    pageSize: DEFAULT_PAGINATION_PAGE_SIZE,
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
    meta: PaginationMeta | null | undefined,
    fallbackTotalCount = 0,
  ) {
    pagination.totalCount = meta?.totalCount ?? fallbackTotalCount
    pagination.totalPages = Math.max(1, meta?.totalPages ?? 1)
    pagination.page = Math.max(1, meta?.currentPage ?? pagination.page)
    pagination.pageSize = Math.max(1, meta?.pageSize ?? pagination.pageSize)
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
