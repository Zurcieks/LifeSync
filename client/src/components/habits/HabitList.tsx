import type { Habit } from '../../types/habit'
import { HabitCard } from './HabitCard'

interface HabitListProps {
  habits: Habit[]
  onToggleToday: (habitId: string) => void
  onEdit: (habit: Habit) => void
  onDelete: (id: string) => void
}

export function HabitList({ habits, onToggleToday, onEdit, onDelete }: HabitListProps) {
  if (habits.length === 0) {
    return (
      <div className="py-12 text-center text-sm text-gray-400">
        No habits yet. Create one to get started.
      </div>
    )
  }

  return (
    <div className="space-y-3">
      {habits.map((habit) => (
        <HabitCard
          key={habit.id}
          habit={habit}
          onToggleToday={onToggleToday}
          onEdit={onEdit}
          onDelete={onDelete}
        />
      ))}
    </div>
  )
}
