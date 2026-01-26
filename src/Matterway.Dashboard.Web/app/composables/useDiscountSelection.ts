export function useDiscountSelection() {
  const createIds = useState<string[]>('discount-create-ids', () => [])
  const updateIds = useState<string[]>('discount-update-ids', () => [])

  function addArticleId(target: 'create' | 'update', id: string) {
    if (!id) return
    const list = target === 'create' ? createIds : updateIds
    if (!list.value.includes(id)) list.value = [...list.value, id]
  }

  function removeArticleId(target: 'create' | 'update', id: string) {
    const list = target === 'create' ? createIds : updateIds
    list.value = list.value.filter(item => item !== id)
  }

  function setArticleIds(target: 'create' | 'update', ids: string[]) {
    const list = target === 'create' ? createIds : updateIds
    list.value = [...ids]
  }

  return {
    createIds,
    updateIds,
    addArticleId,
    removeArticleId,
    setArticleIds
  }
}
