import type { WorkoutLog } from '../../types/workout'

interface WorkoutLogListProps {
  logs: WorkoutLog[]
}

export function WorkoutLogList({ logs }: WorkoutLogListProps) {
  if (logs.length === 0) {
    return (
      <div className="py-8 text-center text-sm text-gray-400">
        No workouts logged yet.
      </div>
    )
  }

  return (
    <>
      {/* Desktop table */}
      <div className="hidden overflow-hidden rounded-xl bg-white shadow-sm ring-1 ring-gray-200 md:block">
        <table className="w-full text-left text-sm">
          <thead className="border-b border-gray-200 bg-gray-50">
            <tr>
              <th className="px-4 py-3 font-medium text-gray-500">Date</th>
              <th className="px-4 py-3 font-medium text-gray-500">Plan</th>
              <th className="px-4 py-3 font-medium text-gray-500">Duration</th>
              <th className="px-4 py-3 font-medium text-gray-500">Notes</th>
            </tr>
          </thead>
          <tbody className="divide-y divide-gray-100">
            {logs.map((log) => (
              <tr key={log.id} className="hover:bg-gray-50 transition-colors">
                <td className="px-4 py-3 text-gray-500">
                  {new Date(log.completedAt).toLocaleDateString()}
                </td>
                <td className="px-4 py-3 font-medium text-gray-900">{log.trainingPlanName}</td>
                <td className="px-4 py-3 text-gray-700">{log.durationMinutes} min</td>
                <td className="px-4 py-3 text-gray-500">{log.notes || '—'}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      {/* Mobile cards */}
      <div className="space-y-3 md:hidden">
        {logs.map((log) => (
          <div
            key={log.id}
            className="rounded-xl bg-white p-4 shadow-sm ring-1 ring-gray-200"
          >
            <div className="flex items-start justify-between">
              <div>
                <p className="font-medium text-gray-900">{log.trainingPlanName}</p>
                <p className="mt-0.5 text-xs text-gray-400">
                  {new Date(log.completedAt).toLocaleDateString()}
                </p>
              </div>
              <span className="shrink-0 rounded-full bg-indigo-50 px-2.5 py-0.5 text-xs font-medium text-indigo-700">
                {log.durationMinutes} min
              </span>
            </div>
            {log.notes && (
              <p className="mt-2 text-sm text-gray-500">{log.notes}</p>
            )}
          </div>
        ))}
      </div>
    </>
  )
}
