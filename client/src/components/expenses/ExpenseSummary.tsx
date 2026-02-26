import type { ExpenseSummary as ExpenseSummaryType } from '../../types/expense'
import { Card } from '../ui/Card'

interface ExpenseSummaryProps {
  summary: ExpenseSummaryType
}

export function ExpenseSummary({ summary }: ExpenseSummaryProps) {
  return (
    <div className="mb-6 grid gap-4 sm:grid-cols-2 lg:grid-cols-4">
      <Card>
        <div className="text-sm text-gray-500">Total Spent</div>
        <div className="mt-1 text-2xl font-bold text-gray-900">${summary.totalAmount.toFixed(2)}</div>
      </Card>
      <Card>
        <div className="text-sm text-gray-500">Transactions</div>
        <div className="mt-1 text-2xl font-bold text-gray-900">{summary.count}</div>
      </Card>
      <Card className="sm:col-span-2">
        <div className="mb-3 text-sm text-gray-500">By Category</div>
        {summary.byCategory.length === 0 ? (
          <p className="text-sm text-gray-400">No expenses in this period.</p>
        ) : (
          <div className="space-y-2">
            {summary.byCategory.map((cat) => {
              const percent = summary.totalAmount > 0 ? (cat.amount / summary.totalAmount) * 100 : 0
              return (
                <div key={cat.categoryId} className="flex items-center gap-3">
                  <div className="h-3 w-3 rounded-full shrink-0" style={{ backgroundColor: cat.categoryColor }} />
                  <div className="flex-1 text-sm text-gray-700">{cat.categoryName}</div>
                  <div className="text-sm font-medium text-gray-900">${cat.amount.toFixed(2)}</div>
                  <div className="w-16 text-right text-xs text-gray-400">{percent.toFixed(0)}%</div>
                </div>
              )
            })}
          </div>
        )}
      </Card>
    </div>
  )
}
