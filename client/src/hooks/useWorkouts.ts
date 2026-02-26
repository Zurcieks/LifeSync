import { useCallback, useEffect, useState } from 'react'
import { workoutsApi } from '../api/workouts'
import type {
  TrainingPlan,
  WorkoutLog,
  CreateTrainingPlanRequest,
  UpdateTrainingPlanRequest,
  LogWorkoutRequest,
} from '../types/workout'

export function useWorkouts() {
  const [plans, setPlans] = useState<TrainingPlan[]>([])
  const [logs, setLogs] = useState<WorkoutLog[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')

  const fetchAll = useCallback(async () => {
    try {
      setLoading(true)
      const [planData, logData] = await Promise.all([
        workoutsApi.getPlans(),
        workoutsApi.getLogs(),
      ])
      setPlans(planData)
      setLogs(logData)
      setError('')
    } catch {
      setError('Failed to load workouts.')
    } finally {
      setLoading(false)
    }
  }, [])

  useEffect(() => { fetchAll() }, [fetchAll])

  const createPlan = async (request: CreateTrainingPlanRequest) => {
    await workoutsApi.createPlan(request)
    await fetchAll()
  }

  const updatePlan = async (request: UpdateTrainingPlanRequest) => {
    await workoutsApi.updatePlan(request)
    await fetchAll()
  }

  const deletePlan = async (id: string) => {
    await workoutsApi.deletePlan(id)
    await fetchAll()
  }

  const logWorkout = async (request: LogWorkoutRequest) => {
    await workoutsApi.logWorkout(request)
    await fetchAll()
  }

  return { plans, logs, loading, error, createPlan, updatePlan, deletePlan, logWorkout, refetch: fetchAll }
}
