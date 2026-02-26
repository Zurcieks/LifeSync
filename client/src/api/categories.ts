import client from './client'
import type { Category, CreateCategoryRequest, UpdateCategoryRequest } from '../types/expense'

export const categoriesApi = {
  getAll: async () => {
    const { data } = await client.get<Category[]>('/categories')
    return data
  },

  create: async (request: CreateCategoryRequest) => {
    const { data } = await client.post<Category>('/categories', request)
    return data
  },

  update: async (request: UpdateCategoryRequest) => {
    const { data } = await client.put<Category>(`/categories/${request.id}`, request)
    return data
  },

  delete: async (id: string) => {
    await client.delete(`/categories/${id}`)
  },
}
