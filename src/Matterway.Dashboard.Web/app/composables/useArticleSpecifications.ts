import { useApiClient } from '~/composables/useApiClient'
import { useRequestState } from '~/composables/useRequestState'
import { normalizeList } from '~/utils/http'

type ArticleSpecification = {
  specificationSlug?: string
  title?: string
  value?: number
  unit?: string
}

export function useArticleSpecifications() {
  const api = useApiClient()
  const previewState = useRequestState()
  const articlePreview = ref<{ specifications: ArticleSpecification[] } | null>(null)

  async function loadArticleSpecs(articleId?: string) {
    if (!articleId) return
    previewState.error = ''
    previewState.loading = true
    try {
      const result = await api.request<unknown>('catalog', `admin/articles/${articleId}/specifications`)
      if (!result.ok) {
        previewState.error = result.error || 'Failed to load specifications.'
        articlePreview.value = null
        return
      }
      const specifications = normalizeList<ArticleSpecification>(result.data)
      articlePreview.value = { specifications }
    } catch (err) {
      previewState.error = err instanceof Error ? err.message : 'Failed to load specifications.'
      articlePreview.value = null
    } finally {
      previewState.loading = false
    }
  }

  return { articlePreview, previewState, loadArticleSpecs }
}
