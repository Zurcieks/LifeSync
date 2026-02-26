import type { TrainingPlan } from '../../types/workout'
import { Card } from '../ui/Card'
import { Button } from '../ui/Button'

interface TrainingPlanListProps {
  plans: TrainingPlan[]
  onEdit: (plan: TrainingPlan) => void
  onDelete: (id: string) => void
  onLog: (planId: string) => void
}

export function TrainingPlanList({ plans, onEdit, onDelete, onLog }: TrainingPlanListProps) {
  if (plans.length === 0) {
    return (
      <div className="py-12 text-center text-sm text-gray-400">
        No training plans yet. Create one to get started.
      </div>
    )
  }

  return (
    <div className="grid gap-4 sm:grid-cols-2">
      {plans.map((plan) => (
        <Card key={plan.id}>
          <div className="mb-3 flex items-start justify-between">
            <div>
              <h3 className="font-semibold text-gray-900">{plan.name}</h3>
              {plan.description && <p className="mt-0.5 text-sm text-gray-500">{plan.description}</p>}
            </div>
            <div className="flex gap-1">
              <Button variant="ghost" size="sm" onClick={() => onEdit(plan)}>Edit</Button>
              <Button variant="ghost" size="sm" onClick={() => onDelete(plan.id)}>Delete</Button>
            </div>
          </div>

          <div className="mb-3 space-y-1">
            {plan.exercises.map((ex) => (
              <div key={ex.id} className="flex items-center justify-between text-sm">
                <span className="text-gray-700">{ex.name}</span>
                <span className="text-gray-400">
                  {ex.sets}×{ex.reps}
                  {ex.weight > 0 && ` @ ${ex.weight}kg`}
                </span>
              </div>
            ))}
          </div>

          <div className="flex items-center justify-between border-t border-gray-100 pt-3">
            <span className="text-xs text-gray-400">{plan.totalWorkouts} workout{plan.totalWorkouts !== 1 ? 's' : ''} logged</span>
            <Button size="sm" onClick={() => onLog(plan.id)}>Log Workout</Button>
          </div>
        </Card>
      ))}
    </div>
  )
}
