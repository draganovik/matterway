import { useApiClient } from '~/composables/useApiClient'
import { useRequestState } from '~/composables/useRequestState'
import { normalizeList } from '~/utils/http'

type ArticleImage = {
  id?: string
  orderIndex?: number
  imageUrl?: string
  imageAlt?: string
}

export function useArticleImages() {
  const api = useApiClient()
  const previewState = useRequestState()
  const imagePreview = ref<ArticleImage[]>([])

  async function loadImages(articleId?: string) {
    if (!articleId) return
    previewState.error = ''
    previewState.loading = true
    try {
      const result = await api.request<unknown>('catalog', `admin/articles/${articleId}/images`)
      if (!result.ok) {
        previewState.error = result.error || 'Failed to load images.'
        imagePreview.value = []
        return
      }
      imagePreview.value = normalizeList<ArticleImage>(result.data)
    } catch (err) {
      previewState.error = err instanceof Error ? err.message : 'Failed to load images.'
      imagePreview.value = []
    } finally {
      previewState.loading = false
    }
  }

  return { imagePreview, previewState, loadImages }
}
