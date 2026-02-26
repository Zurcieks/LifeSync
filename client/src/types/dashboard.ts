export interface DashboardSummary {
  habits: HabitsSummary
  expenses: ExpensesSummary
  workouts: WorkoutsSummary
}

export interface HabitsSummary {
  totalHabits: number
  completedToday: number
  totalToday: number
  bestCurrentStreak: number
  bestStreakHabitName: string
}

export interface ExpensesSummary {
  monthTotal: number
  monthCount: number
  topCategoryName: string
  topCategoryAmount: number
}

export interface WorkoutsSummary {
  workoutsThisWeek: number
  totalPlans: number
  lastPlanName: string
  lastWorkoutDate: string | null
}
