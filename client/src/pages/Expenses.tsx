import { useMemo, useState } from 'react'
import { useExpenses } from '../hooks/useExpenses'
import { useCategories } from '../hooks/useCategories'
import { ExpenseList } from '../components/expenses/ExpenseList'
import { ExpenseForm } from '../components/expenses/ExpenseForm'
import { ExpenseSummary } from '../components/expenses/ExpenseSummary'
import { Modal } from '../components/ui/Modal'
import { Button } from '../components/ui/Button'
import type { Expense } from '../types/expense'

function getMonthRange() {
  const now = new Date()
  const from = `${now.getFullYear()}-${String(now.getMonth() + 1).padStart(2, '0')}-01`
  const last = new Date(now.getFullYear(), now.getMonth() + 1, 0)
  const to = `${last.getFullYear()}-${String(last.getMonth() + 1).padStart(2, '0')}-${String(last.getDate()).padStart(2, '0')}`
  return { from, to }
}

export default function Expenses() {
  const { categories } = useCategories()
  const [dateFrom, setDateFrom] = useState(getMonthRange().from)
  const [dateTo, setDateTo] = useState(getMonthRange().to)
  const [filterCategory, setFilterCategory] = useState('')
  const [showForm, setShowForm] = useState(false)
  const [editing, setEditing] = useState<Expense | null>(null)

  const filters = useMemo(() => ({
    from: dateFrom || undefined,
    to: dateTo || undefined,
    categoryId: filterCategory || undefined,
  }), [dateFrom, dateTo, filterCategory])

  const { expenses, summary, loading, error, createExpense, updateExpense, deleteExpense } = useExpenses(filters)

  const handleCreate = async (data: { amount: number; description: string; categoryId: string; date: string }) => {
    await createExpense(data)
    setShowForm(false)
  }

  const handleUpdate = async (data: { amount: number; description: string; categoryId: string; date: string }) => {
    if (!editing) return
    await updateExpense({ id: editing.id, ...data })
    setEditing(null)
  }

  const handleDelete = async (id: string) => {
    if (confirm('Are you sure you want to delete this expense?')) {
      await deleteExpense(id)
    }
  }

  if (loading) {
    return (
      <div className="flex items-center justify-center py-12">
        <div className="h-8 w-8 animate-spin rounded-full border-4 border-indigo-600 border-t-transparent" />
      </div>
    )
  }

  return (
    <div>
      <div className="mb-6 flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
        <div>
          <h1 className="text-2xl font-bold text-gray-900">Expenses</h1>
          <p className="mt-1 text-sm text-gray-500">Track and categorize your spending</p>
        </div>
        <Button className="w-full sm:w-auto" onClick={() => setShowForm(true)}>Add Expense</Button>
      </div>

      {error && <div className="mb-4 rounded-lg bg-red-50 px-4 py-3 text-sm text-red-700">{error}</div>}

      {summary && <ExpenseSummary summary={summary} />}

      <div className="mb-4 grid grid-cols-1 gap-3 sm:flex sm:flex-wrap sm:items-center">
        <div className="flex items-center gap-2">
          <input
            type="date"
            value={dateFrom}
            onChange={(e) => setDateFrom(e.target.value)}
            className="w-full rounded-lg border border-gray-300 px-3 py-2 text-sm focus:border-indigo-500 focus:outline-none focus:ring-1 focus:ring-indigo-500 sm:w-auto"
          />
          <span className="text-sm text-gray-400">to</span>
          <input
            type="date"
            value={dateTo}
            onChange={(e) => setDateTo(e.target.value)}
            className="w-full rounded-lg border border-gray-300 px-3 py-2 text-sm focus:border-indigo-500 focus:outline-none focus:ring-1 focus:ring-indigo-500 sm:w-auto"
          />
        </div>
        <select
          value={filterCategory}
          onChange={(e) => setFilterCategory(e.target.value)}
          className="w-full rounded-lg border border-gray-300 px-3 py-2 text-sm focus:border-indigo-500 focus:outline-none focus:ring-1 focus:ring-indigo-500 sm:w-auto"
        >
          <option value="">All Categories</option>
          {categories.map((cat) => (
            <option key={cat.id} value={cat.id}>{cat.name}</option>
          ))}
        </select>
      </div>

      <ExpenseList expenses={expenses} onEdit={setEditing} onDelete={handleDelete} />

      <Modal open={showForm} onClose={() => setShowForm(false)} title="Add Expense">
        <ExpenseForm categories={categories} onSubmit={handleCreate} onCancel={() => setShowForm(false)} />
      </Modal>

      <Modal open={!!editing} onClose={() => setEditing(null)} title="Edit Expense">
        <ExpenseForm categories={categories} initial={editing} onSubmit={handleUpdate} onCancel={() => setEditing(null)} />
      </Modal>
    </div>
  )
}
