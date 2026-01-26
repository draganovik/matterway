import { useApiClient } from '~/composables/useApiClient'
import { useRequestState } from '~/composables/useRequestState'
import { normalizeList } from '~/utils/http'

type ArticleDetail = {
  detailSlug?: string
  title?: string
  value?: string
}

export function useArticleDetails() {
  const api = useApiClient()
  const previewState = useRequestState()
  const articlePreview = ref<{ details: ArticleDetail[] } | null>(null)

  async function loadArticleDetails(articleId?: string) {
    if (!articleId) return
    previewState.error = ''
    previewState.loading = true
    try {
      const result = await api.request<unknown>('catalog', `admin/articles/${articleId}/details`)
      if (!result.ok) {
        previewState.error = result.error || 'Failed to load details.'
        articlePreview.value = null
        return
      }
      const details = normalizeList<ArticleDetail>(result.data)
      articlePreview.value = { details }
    } catch (err) {
      previewState.error = err instanceof Error ? err.message : 'Failed to load details.'
      articlePreview.value = null
    } finally {
      previewState.loading = false
    }
  }

  return { articlePreview, previewState, loadArticleDetails }
}
