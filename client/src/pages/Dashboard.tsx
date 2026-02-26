import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { dashboardApi } from '../api/dashboard'
import { useAuth } from '../hooks/useAuth'
import { Card } from '../components/ui/Card'
import type { DashboardSummary } from '../types/dashboard'

export default function Dashboard() {
  const { user } = useAuth()
  const [data, setData] = useState<DashboardSummary | null>(null)
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    dashboardApi.getSummary()
      .then(setData)
      .catch(() => {})
      .finally(() => setLoading(false))
  }, [])

  if (loading) {
    return (
      <div className="flex items-center justify-center py-12">
        <div className="h-8 w-8 animate-spin rounded-full border-4 border-indigo-600 border-t-transparent" />
      </div>
    )
  }

  if (!data) {
    return <div className="py-12 text-center text-sm text-gray-400">Failed to load dashboard.</div>
  }

  const { habits, expenses, workouts } = data

  return (
    <div>
      <div className="mb-8">
        <h1 className="text-2xl font-bold text-gray-900">
          Welcome back, {user?.firstName}
        </h1>
        <p className="mt-1 text-sm text-gray-500">Here's your daily overview</p>
      </div>

      <div className="grid gap-6 sm:grid-cols-2 lg:grid-cols-3">
        {/* Habits */}
        <Link to="/habits" className="group">
          <Card className="transition-shadow group-hover:shadow-md">
            <div className="mb-4 flex items-center justify-between">
              <h2 className="text-sm font-medium text-gray-500">Habits</h2>
              <span className="rounded-full bg-indigo-50 px-2.5 py-0.5 text-xs font-medium text-indigo-700">
                {habits.completedToday}/{habits.totalToday} today
              </span>
            </div>

            <div className="mb-4">
              <div className="h-2 w-full rounded-full bg-gray-100">
                <div
                  className="h-2 rounded-full bg-indigo-500 transition-all"
                  style={{ width: `${habits.totalToday > 0 ? (habits.completedToday / habits.totalToday) * 100 : 0}%` }}
                />
              </div>
            </div>

            <div className="grid grid-cols-2 gap-4">
              <div>
                <div className="text-2xl font-bold text-gray-900">{habits.bestCurrentStreak}</div>
                <div className="text-xs text-gray-500">Best streak (days)</div>
              </div>
              <div>
                <div className="text-sm font-medium text-gray-700 truncate">{habits.bestStreakHabitName}</div>
                <div className="text-xs text-gray-500">Top habit</div>
              </div>
            </div>
          </Card>
        </Link>

        {/* Expenses */}
        <Link to="/expenses" className="group">
          <Card className="transition-shadow group-hover:shadow-md">
            <div className="mb-4 flex items-center justify-between">
              <h2 className="text-sm font-medium text-gray-500">Expenses</h2>
              <span className="rounded-full bg-emerald-50 px-2.5 py-0.5 text-xs font-medium text-emerald-700">
                This month
              </span>
            </div>

            <div className="mb-4">
              <div className="text-3xl font-bold text-gray-900">${expenses.monthTotal.toFixed(2)}</div>
              <div className="text-xs text-gray-500">{expenses.monthCount} transaction{expenses.monthCount !== 1 ? 's' : ''}</div>
            </div>

            <div className="border-t border-gray-100 pt-3">
              <div className="text-xs text-gray-500">Top category</div>
              <div className="flex items-center justify-between">
                <span className="text-sm font-medium text-gray-700">{expenses.topCategoryName}</span>
                <span className="text-sm font-semibold text-gray-900">${expenses.topCategoryAmount.toFixed(2)}</span>
              </div>
            </div>
          </Card>
        </Link>

        {/* Workouts */}
        <Link to="/workouts" className="group">
          <Card className="transition-shadow group-hover:shadow-md">
            <div className="mb-4 flex items-center justify-between">
              <h2 className="text-sm font-medium text-gray-500">Workouts</h2>
              <span className="rounded-full bg-orange-50 px-2.5 py-0.5 text-xs font-medium text-orange-700">
                This week
              </span>
            </div>

            <div className="mb-4">
              <div className="text-3xl font-bold text-gray-900">{workouts.workoutsThisWeek}</div>
              <div className="text-xs text-gray-500">session{workouts.workoutsThisWeek !== 1 ? 's' : ''} completed</div>
            </div>

            <div className="border-t border-gray-100 pt-3">
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <div className="text-sm font-medium text-gray-700">{workouts.totalPlans}</div>
                  <div className="text-xs text-gray-500">Plans</div>
                </div>
                <div>
                  <div className="text-sm font-medium text-gray-700 truncate">{workouts.lastPlanName}</div>
                  <div className="text-xs text-gray-500">
                    {workouts.lastWorkoutDate
                      ? `Last: ${new Date(workouts.lastWorkoutDate).toLocaleDateString()}`
                      : 'No workouts yet'}
                  </div>
                </div>
              </div>
            </div>
          </Card>
        </Link>
      </div>
    </div>
  )
}
