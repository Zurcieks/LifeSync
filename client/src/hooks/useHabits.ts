import { useCallback, useEffect, useState } from 'react'
import { habitsApi } from '../api/habits'
import type { Habit, CreateHabitRequest, UpdateHabitRequest } from '../types/habit'

export function useHabits() {
  const [habits, setHabits] = useState<Habit[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')

  const fetchHabits = useCallback(async () => {
    try {
      setLoading(true)
      const data = await habitsApi.getAll()
      setHabits(data)
      setError('')
    } catch {
      setError('Failed to load habits.')
    } finally {
      setLoading(false)
    }
  }, [])

  useEffect(() => { fetchHabits() }, [fetchHabits])

  const createHabit = async (request: CreateHabitRequest) => {
    const habit = await habitsApi.create(request)
    setHabits((prev) => [...prev, habit])
  }

  const updateHabit = async (request: UpdateHabitRequest) => {
    const updated = await habitsApi.update(request)
    setHabits((prev) => prev.map((h) => (h.id === updated.id ? updated : h)))
  }

  const deleteHabit = async (id: string) => {
    await habitsApi.delete(id)
    setHabits((prev) => prev.filter((h) => h.id !== id))
  }

  const toggleEntry = async (habitId: string, date: string) => {
    const updated = await habitsApi.toggleEntry({ habitId, date })
    setHabits((prev) => prev.map((h) => (h.id === updated.id ? updated : h)))
  }

  return { habits, loading, error, createHabit, updateHabit, deleteHabit, toggleEntry, refetch: fetchHabits }
}
