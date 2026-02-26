import client from './client'
import type { Expense, ExpenseSummary, CreateExpenseRequest, UpdateExpenseRequest } from '../types/expense'

interface GetExpensesParams {
  from?: string
  to?: string
  categoryId?: string
}

export const expensesApi = {
  getAll: async (params?: GetExpensesParams) => {
    const { data } = await client.get<Expense[]>('/expenses', { params })
    return data
  },

  getById: async (id: string) => {
    const { data } = await client.get<Expense>(`/expenses/${id}`)
    return data
  },

  getSummary: async (params?: { from?: string; to?: string }) => {
    const { data } = await client.get<ExpenseSummary>('/expenses/summary', { params })
    return data
  },

  create: async (request: CreateExpenseRequest) => {
    const { data } = await client.post<Expense>('/expenses', request)
    return data
  },

  update: async (request: UpdateExpenseRequest) => {
    const { data } = await client.put<Expense>(`/expenses/${request.id}`, request)
    return data
  },

  delete: async (id: string) => {
    await client.delete(`/expenses/${id}`)
  },
}
