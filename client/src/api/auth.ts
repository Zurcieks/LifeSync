import axios from 'axios'

export interface User {
  id: string
  email: string
  firstName: string
  lastName: string
}

export interface AuthResponse {
  accessToken: string
  refreshToken: string
  user: User
}

const baseClient = axios.create({
  baseURL: '/api/auth',
  headers: { 'Content-Type': 'application/json' },
})

export const authApi = {
  login: async (email: string, password: string): Promise<AuthResponse> => {
    const { data } = await baseClient.post<AuthResponse>('/login', { email, password })
    return data
  },

  register: async (email: string, password: string, firstName: string, lastName: string): Promise<AuthResponse> => {
    const { data } = await baseClient.post<AuthResponse>('/register', { email, password, firstName, lastName })
    return data
  },

  refresh: async (refreshToken: string): Promise<AuthResponse> => {
    const { data } = await baseClient.post<AuthResponse>('/refresh', { refreshToken })
    return data
  },

  revoke: async (refreshToken: string): Promise<void> => {
    await baseClient.post('/revoke', { refreshToken })
  },
}
