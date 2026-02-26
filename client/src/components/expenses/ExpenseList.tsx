import type { Expense } from '../../types/expense'
import { Button } from '../ui/Button'

interface ExpenseListProps {
  expenses: Expense[]
  onEdit: (expense: Expense) => void
  onDelete: (id: string) => void
}

export function ExpenseList({ expenses, onEdit, onDelete }: ExpenseListProps) {
  if (expenses.length === 0) {
    return (
      <div className="py-12 text-center text-sm text-gray-400">
        No expenses found for the selected period.
      </div>
    )
  }

  return (
    <>
      {/* Desktop table */}
      <div className="hidden overflow-hidden rounded-xl bg-white shadow-sm ring-1 ring-gray-200 md:block">
        <table className="w-full text-left text-sm">
          <thead className="border-b border-gray-200 bg-gray-50">
            <tr>
              <th className="px-4 py-3 font-medium text-gray-500">Date</th>
              <th className="px-4 py-3 font-medium text-gray-500">Description</th>
              <th className="px-4 py-3 font-medium text-gray-500">Category</th>
              <th className="px-4 py-3 text-right font-medium text-gray-500">Amount</th>
              <th className="px-4 py-3 text-right font-medium text-gray-500">Actions</th>
            </tr>
          </thead>
          <tbody className="divide-y divide-gray-100">
            {expenses.map((expense) => (
              <tr key={expense.id} className="hover:bg-gray-50 transition-colors">
                <td className="px-4 py-3 text-gray-500">
                  {new Date(expense.date + 'T00:00:00').toLocaleDateString()}
                </td>
                <td className="px-4 py-3 text-gray-900">{expense.description || '—'}</td>
                <td className="px-4 py-3">
                  <span className="inline-flex items-center gap-1.5">
                    <span
                      className="h-2.5 w-2.5 rounded-full"
                      style={{ backgroundColor: expense.categoryColor }}
                    />
                    {expense.categoryName}
                  </span>
                </td>
                <td className="px-4 py-3 text-right font-medium text-gray-900">
                  ${expense.amount.toFixed(2)}
                </td>
                <td className="px-4 py-3 text-right">
                  <div className="flex justify-end gap-1">
                    <Button variant="ghost" size="sm" onClick={() => onEdit(expense)}>Edit</Button>
                    <Button variant="ghost" size="sm" onClick={() => onDelete(expense.id)}>Delete</Button>
                  </div>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      {/* Mobile cards */}
      <div className="space-y-3 md:hidden">
        {expenses.map((expense) => (
          <div
            key={expense.id}
            className="rounded-xl bg-white p-4 shadow-sm ring-1 ring-gray-200"
          >
            <div className="flex items-start justify-between">
              <div className="min-w-0 flex-1">
                <div className="flex items-center gap-2">
                  <span
                    className="h-2.5 w-2.5 shrink-0 rounded-full"
                    style={{ backgroundColor: expense.categoryColor }}
                  />
                  <span className="truncate text-sm text-gray-500">{expense.categoryName}</span>
                </div>
                <p className="mt-1 font-medium text-gray-900">
                  {expense.description || '—'}
                </p>
                <p className="mt-0.5 text-xs text-gray-400">
                  {new Date(expense.date + 'T00:00:00').toLocaleDateString()}
                </p>
              </div>
              <span className="shrink-0 text-lg font-semibold text-gray-900">
                ${expense.amount.toFixed(2)}
              </span>
            </div>
            <div className="mt-3 flex justify-end gap-1 border-t border-gray-100 pt-2">
              <Button variant="ghost" size="sm" onClick={() => onEdit(expense)}>Edit</Button>
              <Button variant="ghost" size="sm" onClick={() => onDelete(expense.id)}>Delete</Button>
            </div>
          </div>
        ))}
      </div>
    </>
  )
}
