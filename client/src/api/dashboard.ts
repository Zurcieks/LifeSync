import client from './client'
import type { DashboardSummary } from '../types/dashboard'

export const dashboardApi = {
  getSummary: async () => {
    const { data } = await client.get<DashboardSummary>('/dashboard')
    return data
  },
}
