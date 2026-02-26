import type { ReactNode } from 'react'

interface CardProps {
  children: ReactNode
  className?: string
}

export function Card({ children, className = '' }: CardProps) {
  return (
    <div className={`rounded-xl bg-white p-6 shadow-sm ring-1 ring-gray-200 ${className}`}>
      {children}
    </div>
  )
}
