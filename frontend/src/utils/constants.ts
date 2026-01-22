import { appConfig } from '@/config/app.config'
import { progressConfig } from '@/config/progress.config'

export const API_ENDPOINTS = {
  TODOS: '/todolists',
  CATEGORIES: '/categories',
  PROGRESSIONS: (id: number) => `/todolists/${id}/progressions`
} as const

export const QUERY_KEYS = {
  TODOS: ['todos'],
  CATEGORIES: ['categories'],
  TODO: (id: number) => ['todos', id]
} as const

// Use configuration instead of hardcoded values
export const NOTIFICATION_DURATION = appConfig.notifications.duration

export const PROGRESS_THRESHOLDS = {
  NOT_STARTED: progressConfig.thresholds.notStarted,
  LOW: progressConfig.thresholds.low,
  MEDIUM: progressConfig.thresholds.medium,
  HIGH: progressConfig.thresholds.high,
  ALMOST_COMPLETE: progressConfig.thresholds.almostComplete,
  COMPLETE: progressConfig.thresholds.complete
} as const

export const DEFAULT_CATEGORIES = progressConfig.defaultCategories
