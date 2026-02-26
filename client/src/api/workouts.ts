import client from './client'
import type {
  TrainingPlan,
  WorkoutLog,
  CreateTrainingPlanRequest,
  UpdateTrainingPlanRequest,
  LogWorkoutRequest,
} from '../types/workout'

export const workoutsApi = {
  getPlans: async () => {
    const { data } = await client.get<TrainingPlan[]>('/workouts/plans')
    return data
  },

  getPlanById: async (id: string) => {
    const { data } = await client.get<TrainingPlan>(`/workouts/plans/${id}`)
    return data
  },

  createPlan: async (request: CreateTrainingPlanRequest) => {
    const { data } = await client.post<TrainingPlan>('/workouts/plans', request)
    return data
  },

  updatePlan: async (request: UpdateTrainingPlanRequest) => {
    const { data } = await client.put<TrainingPlan>(`/workouts/plans/${request.id}`, request)
    return data
  },

  deletePlan: async (id: string) => {
    await client.delete(`/workouts/plans/${id}`)
  },

  getLogs: async (trainingPlanId?: string) => {
    const { data } = await client.get<WorkoutLog[]>('/workouts/logs', {
      params: trainingPlanId ? { trainingPlanId } : undefined,
    })
    return data
  },

  logWorkout: async (request: LogWorkoutRequest) => {
    const { data } = await client.post<WorkoutLog>('/workouts/logs', request)
    return data
  },
}
