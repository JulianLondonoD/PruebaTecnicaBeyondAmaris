/**
 * Helper utilities for progress calculations
 * These are pure functions that don't depend on Vue or Vue Query
 */

export const calculateProgressColor = (progress: number): string => {
  if (progress === 0) return 'bg-gray-200'
  if (progress < 25) return 'bg-red-500'
  if (progress < 50) return 'bg-yellow-500'
  if (progress < 75) return 'bg-blue-500'
  if (progress < 100) return 'bg-green-500'
  return 'bg-emerald-600'
}

export const getProgressStatus = (progress: number): string => {
  if (progress === 0) return 'Not Started'
  if (progress < 100) return 'In Progress'
  return 'Completed'
}

export const getProgressColorClass = (progress: number): string => {
  if (progress === 0) return 'progress-not-started'
  if (progress < 25) return 'progress-low'
  if (progress < 50) return 'progress-medium'
  if (progress < 75) return 'progress-high'
  if (progress < 100) return 'progress-almost'
  return 'progress-complete'
}

export const calculateEstimatedCompletion = (progress: number, progressions?: any[]): string => {
  if (progress >= 100) return 'Completed'
  if (progress === 0) return 'Not started'

  if (progressions && progressions.length > 1) {
    // Calculate based on recent progress velocity
    const recentProgressions = progressions.slice(0, 5)
    const totalDays = recentProgressions.length
    const avgProgressPerDay = recentProgressions.reduce((acc, p) => acc + p.percent, 0) / totalDays

    if (avgProgressPerDay > 0) {
      const remainingProgress = 100 - progress
      const estimatedDays = Math.ceil(remainingProgress / avgProgressPerDay)

      if (estimatedDays === 1) return 'About 1 day'
      if (estimatedDays < 7) return `About ${estimatedDays} days`
      if (estimatedDays < 30) return `About ${Math.ceil(estimatedDays / 7)} weeks`
      return `About ${Math.ceil(estimatedDays / 30)} months`
    }
  }

  // Fallback calculation
  const remainingProgress = 100 - progress
  const estimatedDays = Math.ceil(remainingProgress / 10)

  if (estimatedDays === 1) return 'About 1 day'
  if (estimatedDays < 7) return `About ${estimatedDays} days`
  if (estimatedDays < 30) return `About ${Math.ceil(estimatedDays / 7)} weeks`
  return `About ${Math.ceil(estimatedDays / 30)} months`
}
