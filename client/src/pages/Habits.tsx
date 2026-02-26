import { useState } from 'react'
import { useHabits } from '../hooks/useHabits'
import { HabitList } from '../components/habits/HabitList'
import { HabitForm } from '../components/habits/HabitForm'
import { Modal } from '../components/ui/Modal'
import { Button } from '../components/ui/Button'
import type { Habit } from '../types/habit'

export default function Habits() {
  const { habits, loading, error, createHabit, updateHabit, deleteHabit, toggleEntry } = useHabits()
  const [showForm, setShowForm] = useState(false)
  const [editing, setEditing] = useState<Habit | null>(null)

  const today = new Date().toISOString().split('T')[0]

  const handleToggle = async (habitId: string) => {
    await toggleEntry(habitId, today)
  }

  const handleCreate = async (data: { name: string; description: string }) => {
    await createHabit(data)
    setShowForm(false)
  }

  const handleUpdate = async (data: { name: string; description: string; isArchived: boolean }) => {
    if (!editing) return
    await updateHabit({ id: editing.id, ...data })
    setEditing(null)
  }

  const handleDelete = async (id: string) => {
    if (confirm('Are you sure you want to delete this habit?')) {
      await deleteHabit(id)
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
          <h1 className="text-2xl font-bold text-gray-900">Habits</h1>
          <p className="mt-1 text-sm text-gray-500">Track your daily habits and build streaks</p>
        </div>
        <Button className="w-full sm:w-auto" onClick={() => setShowForm(true)}>New Habit</Button>
      </div>

      {error && <div className="mb-4 rounded-lg bg-red-50 px-4 py-3 text-sm text-red-700">{error}</div>}

      <HabitList
        habits={habits}
        onToggleToday={handleToggle}
        onEdit={setEditing}
        onDelete={handleDelete}
      />

      <Modal open={showForm} onClose={() => setShowForm(false)} title="New Habit">
        <HabitForm onSubmit={handleCreate} onCancel={() => setShowForm(false)} />
      </Modal>

      <Modal open={!!editing} onClose={() => setEditing(null)} title="Edit Habit">
        <HabitForm initial={editing} onSubmit={handleUpdate} onCancel={() => setEditing(null)} />
      </Modal>
    </div>
  )
}
