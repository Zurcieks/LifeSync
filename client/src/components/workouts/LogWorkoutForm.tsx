import { useState, type FormEvent } from 'react'
import { Button } from '../ui/Button'
import { Input } from '../ui/Input'

interface LogWorkoutFormProps {
  onSubmit: (data: { durationMinutes: number; notes: string }) => Promise<void>
  onCancel: () => void
}

export function LogWorkoutForm({ onSubmit, onCancel }: LogWorkoutFormProps) {
  const [duration, setDuration] = useState('30')
  const [notes, setNotes] = useState('')
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState('')

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault()
    setError('')
    setLoading(true)
    try {
      await onSubmit({ durationMinutes: parseInt(duration), notes })
    } catch {
      setError('Failed to log workout.')
    } finally {
      setLoading(false)
    }
  }

  return (
    <form onSubmit={handleSubmit} className="space-y-4">
      {error && <div className="rounded-lg bg-red-50 px-4 py-3 text-sm text-red-700">{error}</div>}

      <Input
        id="log-duration"
        label="Duration (minutes)"
        type="number"
        required
        min={1}
        value={duration}
        onChange={(e) => setDuration(e.target.value)}
      />

      <div>
        <label htmlFor="log-notes" className="mb-1 block text-sm font-medium text-gray-700">Notes</label>
        <textarea
          id="log-notes"
          maxLength={500}
          rows={3}
          value={notes}
          onChange={(e) => setNotes(e.target.value)}
          className="w-full rounded-lg border border-gray-300 px-3 py-2 text-sm focus:border-indigo-500 focus:outline-none focus:ring-1 focus:ring-indigo-500"
          placeholder="How did it go?"
        />
      </div>

      <div className="flex justify-end gap-2">
        <Button type="button" variant="secondary" onClick={onCancel}>Cancel</Button>
        <Button type="submit" disabled={loading}>
          {loading ? 'Logging...' : 'Log Workout'}
        </Button>
      </div>
    </form>
  )
}
