import { useState, type FormEvent } from 'react'
import { Button } from '../ui/Button'
import { Input } from '../ui/Input'
import type { Category, Expense } from '../../types/expense'

interface ExpenseFormProps {
  categories: Category[]
  initial?: Expense | null
  onSubmit: (data: { amount: number; description: string; categoryId: string; date: string }) => Promise<void>
  onCancel: () => void
}

export function ExpenseForm({ categories, initial, onSubmit, onCancel }: ExpenseFormProps) {
  const [amount, setAmount] = useState(initial?.amount?.toString() ?? '')
  const [description, setDescription] = useState(initial?.description ?? '')
  const [categoryId, setCategoryId] = useState(initial?.categoryId ?? (categories[0]?.id ?? ''))
  const [date, setDate] = useState(initial?.date ?? new Date().toISOString().split('T')[0])
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState('')

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault()
    setError('')
    setLoading(true)
    try {
      await onSubmit({ amount: parseFloat(amount), description, categoryId, date })
    } catch {
      setError('Failed to save expense.')
    } finally {
      setLoading(false)
    }
  }

  return (
    <form onSubmit={handleSubmit} className="space-y-4">
      {error && <div className="rounded-lg bg-red-50 px-4 py-3 text-sm text-red-700">{error}</div>}

      <div className="grid grid-cols-2 gap-3">
        <Input
          id="expense-amount"
          label="Amount"
          type="number"
          required
          min="0.01"
          max="999999.99"
          step="0.01"
          value={amount}
          onChange={(e) => setAmount(e.target.value)}
          placeholder="0.00"
        />
        <Input
          id="expense-date"
          label="Date"
          type="date"
          required
          value={date}
          onChange={(e) => setDate(e.target.value)}
        />
      </div>

      <div>
        <label htmlFor="expense-category" className="mb-1 block text-sm font-medium text-gray-700">Category</label>
        <select
          id="expense-category"
          required
          value={categoryId}
          onChange={(e) => setCategoryId(e.target.value)}
          className="w-full rounded-lg border border-gray-300 px-3 py-2 text-sm focus:border-indigo-500 focus:outline-none focus:ring-1 focus:ring-indigo-500"
        >
          {categories.map((cat) => (
            <option key={cat.id} value={cat.id}>{cat.name}</option>
          ))}
        </select>
      </div>

      <Input
        id="expense-desc"
        label="Description"
        maxLength={200}
        value={description}
        onChange={(e) => setDescription(e.target.value)}
        placeholder="What was this for?"
      />

      <div className="flex justify-end gap-2">
        <Button type="button" variant="secondary" onClick={onCancel}>Cancel</Button>
        <Button type="submit" disabled={loading}>
          {loading ? 'Saving...' : initial ? 'Update' : 'Add Expense'}
        </Button>
      </div>
    </form>
  )
}
