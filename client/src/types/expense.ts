export interface Expense {
  id: string
  amount: number
  description: string
  categoryId: string
  categoryName: string
  categoryColor: string
  date: string
  createdAt: string
}

export interface Category {
  id: string
  name: string
  color: string
}

export interface ExpenseSummary {
  totalAmount: number
  count: number
  byCategory: CategorySummary[]
}

export interface CategorySummary {
  categoryId: string
  categoryName: string
  categoryColor: string
  amount: number
  count: number
}

export interface CreateExpenseRequest {
  amount: number
  description: string
  categoryId: string
  date: string
}

export interface UpdateExpenseRequest extends CreateExpenseRequest {
  id: string
}

export interface CreateCategoryRequest {
  name: string
  color: string
}

export interface UpdateCategoryRequest extends CreateCategoryRequest {
  id: string
}
