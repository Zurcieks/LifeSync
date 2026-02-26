export interface Habit {
  id: string
  name: string
  description: string
  isArchived: boolean
  createdAt: string
  currentStreak: number
  totalCompletions: number
  completedToday: boolean
}

export interface CreateHabitRequest {
  name: string
  description: string
}

export interface UpdateHabitRequest {
  id: string
  name: string
  description: string
  isArchived: boolean
}

export interface ToggleHabitEntryRequest {
  habitId: string
  date: string
}
