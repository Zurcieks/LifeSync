import { useState } from 'react'
import { useWorkouts } from '../hooks/useWorkouts'
import { TrainingPlanList } from '../components/workouts/TrainingPlanList'
import { TrainingPlanForm } from '../components/workouts/TrainingPlanForm'
import { LogWorkoutForm } from '../components/workouts/LogWorkoutForm'
import { WorkoutLogList } from '../components/workouts/WorkoutLogList'
import { Modal } from '../components/ui/Modal'
import { Button } from '../components/ui/Button'
import type { TrainingPlan, CreateExerciseRequest } from '../types/workout'

export default function Workouts() {
  const { plans, logs, loading, error, createPlan, updatePlan, deletePlan, logWorkout } = useWorkouts()
  const [showPlanForm, setShowPlanForm] = useState(false)
  const [editing, setEditing] = useState<TrainingPlan | null>(null)
  const [loggingPlanId, setLoggingPlanId] = useState<string | null>(null)

  const handleCreatePlan = async (data: { name: string; description: string; exercises: CreateExerciseRequest[] }) => {
    await createPlan(data)
    setShowPlanForm(false)
  }

  const handleUpdatePlan = async (data: { name: string; description: string; exercises: CreateExerciseRequest[] }) => {
    if (!editing) return
    await updatePlan({ id: editing.id, ...data })
    setEditing(null)
  }

  const handleDeletePlan = async (id: string) => {
    if (confirm('Delete this training plan and all its workout logs?')) {
      await deletePlan(id)
    }
  }

  const handleLogWorkout = async (data: { durationMinutes: number; notes: string }) => {
    if (!loggingPlanId) return
    await logWorkout({ trainingPlanId: loggingPlanId, ...data })
    setLoggingPlanId(null)
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
          <h1 className="text-2xl font-bold text-gray-900">Workouts</h1>
          <p className="mt-1 text-sm text-gray-500">Create training plans and log your sessions</p>
        </div>
        <Button className="w-full sm:w-auto" onClick={() => setShowPlanForm(true)}>New Plan</Button>
      </div>

      {error && <div className="mb-4 rounded-lg bg-red-50 px-4 py-3 text-sm text-red-700">{error}</div>}

      <TrainingPlanList
        plans={plans}
        onEdit={setEditing}
        onDelete={handleDeletePlan}
        onLog={setLoggingPlanId}
      />

      {logs.length > 0 && (
        <div className="mt-8">
          <h2 className="mb-4 text-lg font-semibold text-gray-900">Recent Workouts</h2>
          <WorkoutLogList logs={logs} />
        </div>
      )}

      <Modal open={showPlanForm} onClose={() => setShowPlanForm(false)} title="New Training Plan">
        <TrainingPlanForm onSubmit={handleCreatePlan} onCancel={() => setShowPlanForm(false)} />
      </Modal>

      <Modal open={!!editing} onClose={() => setEditing(null)} title="Edit Training Plan">
        <TrainingPlanForm initial={editing} onSubmit={handleUpdatePlan} onCancel={() => setEditing(null)} />
      </Modal>

      <Modal open={!!loggingPlanId} onClose={() => setLoggingPlanId(null)} title="Log Workout">
        <LogWorkoutForm onSubmit={handleLogWorkout} onCancel={() => setLoggingPlanId(null)} />
      </Modal>
    </div>
  )
}
