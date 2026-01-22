export const formatDate = (dateString: string, options?: Intl.DateTimeFormatOptions): string => {
  const date = new Date(dateString)
  const defaultOptions: Intl.DateTimeFormatOptions = {
    year: 'numeric',
    month: 'short',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  }
  return date.toLocaleDateString('en-US', { ...defaultOptions, ...options })
}

export const formatProgress = (progress: number): string => {
  return `${Math.round(progress)}%`
}

export const formatCategory = (category: string): string => {
  return category.charAt(0).toUpperCase() + category.slice(1).toLowerCase()
}

export const truncateText = (text: string, maxLength: number = 100): string => {
  if (text.length <= maxLength) return text
  return text.substring(0, maxLength) + '...'
}
