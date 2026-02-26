import { useState, type FormEvent } from 'react'
import { Button } from '../ui/Button'
import { Input } from '../ui/Input'
import type { TrainingPlan, CreateExerciseRequest } from '../../types/workout'

interface TrainingPlanFormProps {
  initial?: TrainingPlan | null
  onSubmit: (data: { name: string; description: string; exercises: CreateExerciseRequest[] }) => Promise<void>
  onCancel: () => void
}

const emptyExercise = (): CreateExerciseRequest => ({
  name: '', sets: 3, reps: 10, weight: 0, orderIndex: 0,
})

export function TrainingPlanForm({ initial, onSubmit, onCancel }: TrainingPlanFormProps) {
  const [name, setName] = useState(initial?.name ?? '')
  const [description, setDescription] = useState(initial?.description ?? '')
  const [exercises, setExercises] = useState<CreateExerciseRequest[]>(
    initial?.exercises.map((e, i) => ({ name: e.name, sets: e.sets, reps: e.reps, weight: e.weight, orderIndex: i }))
    ?? [emptyExercise()]
  )
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState('')

  const updateExercise = (index: number, field: keyof CreateExerciseRequest, value: string | number) => {
    setExercises((prev) => prev.map((ex, i) => i === index ? { ...ex, [field]: value } : ex))
  }

  const addExercise = () => {
    setExercises((prev) => [...prev, { ...emptyExercise(), orderIndex: prev.length }])
  }

  const removeExercise = (index: number) => {
    setExercises((prev) => prev.filter((_, i) => i !== index).map((ex, i) => ({ ...ex, orderIndex: i })))
  }

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault()
    setError('')
    setLoading(true)
    try {
      await onSubmit({ name, description, exercises: exercises.map((ex, i) => ({ ...ex, orderIndex: i })) })
    } catch {
      setError('Failed to save training plan.')
    } finally {
      setLoading(false)
    }
  }

  return (
    <form onSubmit={handleSubmit} className="space-y-4">
      {error && <div className="rounded-lg bg-red-50 px-4 py-3 text-sm text-red-700">{error}</div>}

      <Input
        id="plan-name"
        label="Plan Name"
        required
        maxLength={100}
        value={name}
        onChange={(e) => setName(e.target.value)}
        placeholder="e.g. Push/Pull Strength"
      />

      <div>
        <label htmlFor="plan-desc" className="mb-1 block text-sm font-medium text-gray-700">Description</label>
        <textarea
          id="plan-desc"
          maxLength={500}
          rows={2}
          value={description}
          onChange={(e) => setDescription(e.target.value)}
          className="w-full rounded-lg border border-gray-300 px-3 py-2 text-sm focus:border-indigo-500 focus:outline-none focus:ring-1 focus:ring-indigo-500"
          placeholder="Optional description"
        />
      </div>

      <div>
        <div className="mb-2 flex items-center justify-between">
          <label className="text-sm font-medium text-gray-700">Exercises</label>
          <Button type="button" variant="secondary" size="sm" onClick={addExercise}>+ Add</Button>
        </div>

        <div className="space-y-2">
          {exercises.map((ex, i) => (
            <div key={i} className="rounded-lg border border-gray-200 bg-gray-50 p-3">
              {/* Exercise name — full width */}
              <div className="flex items-center gap-2">
                <input
                  required
                  placeholder="Exercise name"
                  maxLength={100}
                  value={ex.name}
                  onChange={(e) => updateExercise(i, 'name', e.target.value)}
                  className="flex-1 rounded border border-gray-300 px-2 py-1.5 text-sm focus:border-indigo-500 focus:outline-none focus:ring-1 focus:ring-indigo-500"
                />
                {exercises.length > 1 && (
                  <button
                    type="button"
                    onClick={() => removeExercise(i)}
                    className="rounded p-1 text-gray-400 hover:bg-gray-200 hover:text-red-500 transition-colors"
                  >
                    ✕
                  </button>
                )}
              </div>

              {/* Numeric inputs — row that wraps on very small screens */}
              <div className="mt-2 flex flex-wrap gap-2">
                <div className="flex-1 min-w-[4.5rem]">
                  <label className="mb-0.5 block text-xs text-gray-400">Sets</label>
                  <input
                    type="number"
                    required
                    min={1}
                    value={ex.sets}
                    onChange={(e) => updateExercise(i, 'sets', parseInt(e.target.value) || 1)}
                    className="w-full rounded border border-gray-300 px-2 py-1.5 text-center text-sm focus:border-indigo-500 focus:outline-none focus:ring-1 focus:ring-indigo-500"
                  />
                </div>
                <div className="flex-1 min-w-[4.5rem]">
                  <label className="mb-0.5 block text-xs text-gray-400">Reps</label>
                  <input
                    type="number"
                    required
                    min={1}
                    value={ex.reps}
                    onChange={(e) => updateExercise(i, 'reps', parseInt(e.target.value) || 1)}
                    className="w-full rounded border border-gray-300 px-2 py-1.5 text-center text-sm focus:border-indigo-500 focus:outline-none focus:ring-1 focus:ring-indigo-500"
                  />
                </div>
                <div className="flex-1 min-w-[5rem]">
                  <label className="mb-0.5 block text-xs text-gray-400">Weight (kg)</label>
                  <input
                    type="number"
                    min={0}
                    step="0.5"
                    value={ex.weight}
                    onChange={(e) => updateExercise(i, 'weight', parseFloat(e.target.value) || 0)}
                    className="w-full rounded border border-gray-300 px-2 py-1.5 text-center text-sm focus:border-indigo-500 focus:outline-none focus:ring-1 focus:ring-indigo-500"
                  />
                </div>
              </div>
            </div>
          ))}
        </div>
      </div>

      <div className="flex justify-end gap-2">
        <Button type="button" variant="secondary" onClick={onCancel}>Cancel</Button>
        <Button type="submit" disabled={loading}>
          {loading ? 'Saving...' : initial ? 'Update Plan' : 'Create Plan'}
        </Button>
      </div>
    </form>
  )
}
