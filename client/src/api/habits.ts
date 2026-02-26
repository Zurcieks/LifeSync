import client from './client'
import type { Habit, CreateHabitRequest, UpdateHabitRequest, ToggleHabitEntryRequest } from '../types/habit'

export const habitsApi = {
  getAll: async (includeArchived = false) => {
    const { data } = await client.get<Habit[]>('/habits', { params: { includeArchived } })
    return data
  },

  getById: async (id: string) => {
    const { data } = await client.get<Habit>(`/habits/${id}`)
    return data
  },

  create: async (request: CreateHabitRequest) => {
    const { data } = await client.post<Habit>('/habits', request)
    return data
  },

  update: async (request: UpdateHabitRequest) => {
    const { data } = await client.put<Habit>(`/habits/${request.id}`, request)
    return data
  },

  delete: async (id: string) => {
    await client.delete(`/habits/${id}`)
  },

  toggleEntry: async (request: ToggleHabitEntryRequest) => {
    const { data } = await client.post<Habit>(`/habits/${request.habitId}/toggle`, request)
    return data
  },
}
