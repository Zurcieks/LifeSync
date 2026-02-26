import { useCallback, useEffect, useState } from 'react'
import { categoriesApi } from '../api/categories'
import type { Category, CreateCategoryRequest } from '../types/expense'

export function useCategories() {
  const [categories, setCategories] = useState<Category[]>([])
  const [loading, setLoading] = useState(true)

  const fetchCategories = useCallback(async () => {
    try {
      setLoading(true)
      const data = await categoriesApi.getAll()
      setCategories(data)
    } catch {
      // categories are non-critical, fail silently
    } finally {
      setLoading(false)
    }
  }, [])

  useEffect(() => { fetchCategories() }, [fetchCategories])

  const createCategory = async (request: CreateCategoryRequest) => {
    const category = await categoriesApi.create(request)
    setCategories((prev) => [...prev, category])
    return category
  }

  return { categories, loading, createCategory, refetch: fetchCategories }
}
