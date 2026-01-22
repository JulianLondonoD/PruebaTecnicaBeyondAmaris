export interface Progression {
  dateTime: string
  percent: number
  accumulatedPercent: number
}

export interface TodoItem {
  id: number
  title: string
  description: string
  category: string
  isCompleted: boolean
  totalProgress: number
  progressions: Progression[]
}

export interface CreateTodoRequest {
  title: string
  description: string
  category: string
}

export interface UpdateTodoRequest {
  description: string
}

export interface AddProgressionRequest {
  id: number
  percent: number
  dateTime: string
}

export interface ApiResponse<T> {
  data: T
  success: boolean
  message?: string
}

export interface PaginatedResponse<T> {
  data: T[]
  total: number
  page: number
  pageSize: number
}
