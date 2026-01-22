/**
 * Progress Configuration
 * 
 * Configuration for progress thresholds and default categories
 */

import { safeParseInt, safeParseJSON } from './utils'

const DEFAULT_CATEGORIES_FALLBACK = ['Work', 'Personal', 'Study', 'Health', 'Finance', 'Shopping', 'Travel']

export const progressConfig = {
  /**
   * Progress Thresholds
   */
  thresholds: {
    notStarted: 0,
    low: safeParseInt(import.meta.env.VITE_PROGRESS_LOW_THRESHOLD, 25),
    medium: safeParseInt(import.meta.env.VITE_PROGRESS_MEDIUM_THRESHOLD, 50),
    high: safeParseInt(import.meta.env.VITE_PROGRESS_HIGH_THRESHOLD, 75),
    almostComplete: safeParseInt(import.meta.env.VITE_PROGRESS_ALMOST_COMPLETE_THRESHOLD, 90),
    complete: 100
  },

  /**
   * Default Categories
   */
  defaultCategories: safeParseJSON<string[]>(
    import.meta.env.VITE_DEFAULT_CATEGORIES,
    DEFAULT_CATEGORIES_FALLBACK
  )
} as const
