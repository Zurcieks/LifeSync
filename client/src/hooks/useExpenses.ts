import { useCallback, useEffect, useState } from 'react'
import { expensesApi } from '../api/expenses'
import type { Expense, ExpenseSummary, CreateExpenseRequest, UpdateExpenseRequest } from '../types/expense'

interface Filters {
  from?: string
  to?: string
  categoryId?: string
}

export function useExpenses(filters?: Filters) {
  const [expenses, setExpenses] = useState<Expense[]>([])
  const [summary, setSummary] = useState<ExpenseSummary | null>(null)
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')

  const fetchExpenses = useCallback(async () => {
    try {
      setLoading(true)
      const [expenseData, summaryData] = await Promise.all([
        expensesApi.getAll(filters),
        expensesApi.getSummary({ from: filters?.from, to: filters?.to }),
      ])
      setExpenses(expenseData)
      setSummary(summaryData)
      setError('')
    } catch {
      setError('Failed to load expenses.')
    } finally {
      setLoading(false)
    }
  }, [filters?.from, filters?.to, filters?.categoryId])

  useEffect(() => { fetchExpenses() }, [fetchExpenses])

  const createExpense = async (request: CreateExpenseRequest) => {
    await expensesApi.create(request)
    await fetchExpenses()
  }

  const updateExpense = async (request: UpdateExpenseRequest) => {
    await expensesApi.update(request)
    await fetchExpenses()
  }

  const deleteExpense = async (id: string) => {
    await expensesApi.delete(id)
    await fetchExpenses()
  }

  return { expenses, summary, loading, error, createExpense, updateExpense, deleteExpense, refetch: fetchExpenses }
}
