import type { Habit } from '../../types/habit'
import { Card } from '../ui/Card'
import { Button } from '../ui/Button'

interface HabitCardProps {
  habit: Habit
  onToggleToday: (habitId: string) => void
  onEdit: (habit: Habit) => void
  onDelete: (id: string) => void
}

export function HabitCard({ habit, onToggleToday, onEdit, onDelete }: HabitCardProps) {
  const today = new Date().toISOString().split('T')[0]

  return (
    <Card>
      <div className="flex items-start gap-3 sm:items-center">
        <button
          onClick={() => onToggleToday(habit.id)}
          className={`flex h-10 w-10 shrink-0 items-center justify-center rounded-full border-2 text-lg transition-colors ${
            habit.completedToday
              ? 'border-green-500 bg-green-50 text-green-600'
              : 'border-gray-300 bg-white text-gray-300 hover:border-green-400 hover:text-green-400'
          }`}
          title={habit.completedToday ? `Completed ${today}` : `Mark complete for ${today}`}
        >
          ✓
        </button>

        <div className="min-w-0 flex-1">
          <h3 className="font-medium text-gray-900">{habit.name}</h3>
          {habit.description && (
            <p className="truncate text-sm text-gray-500">{habit.description}</p>
          )}

          {/* Stats row — visible on all sizes, wraps naturally */}
          <div className="mt-2 flex flex-wrap items-center gap-x-4 gap-y-1">
            <span className="text-sm font-semibold text-indigo-600">
              {habit.currentStreak} day{habit.currentStreak !== 1 ? 's' : ''} streak
            </span>
            <span className="text-sm text-gray-500">
              {habit.totalCompletions} total
            </span>
          </div>
        </div>

        <div className="flex shrink-0 gap-1">
          <Button variant="ghost" size="sm" onClick={() => onEdit(habit)}>Edit</Button>
          <Button variant="ghost" size="sm" onClick={() => onDelete(habit.id)}>Delete</Button>
        </div>
      </div>
    </Card>
  )
}
