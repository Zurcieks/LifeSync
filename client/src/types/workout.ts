export interface TrainingPlan {
  id: string
  name: string
  description: string
  createdAt: string
  exercises: Exercise[]
  totalWorkouts: number
}

export interface Exercise {
  id: string
  name: string
  sets: number
  reps: number
  weight: number
  orderIndex: number
}

export interface WorkoutLog {
  id: string
  trainingPlanId: string
  trainingPlanName: string
  completedAt: string
  durationMinutes: number
  notes: string
}

export interface CreateExerciseRequest {
  name: string
  sets: number
  reps: number
  weight: number
  orderIndex: number
}

export interface CreateTrainingPlanRequest {
  name: string
  description: string
  exercises: CreateExerciseRequest[]
}

export interface UpdateTrainingPlanRequest extends CreateTrainingPlanRequest {
  id: string
}

export interface LogWorkoutRequest {
  trainingPlanId: string
  durationMinutes: number
  notes: string
}
