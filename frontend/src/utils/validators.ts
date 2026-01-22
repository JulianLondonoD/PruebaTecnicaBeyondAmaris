export const validateTitle = (title: string): string | null => {
  if (!title.trim()) return 'Title is required'
  if (title.length < 3) return 'Title must be at least 3 characters'
  if (title.length > 200) return 'Title must be less than 200 characters'
  return null
}

export const validateDescription = (description: string): string | null => {
  if (!description.trim()) return 'Description is required'
  if (description.length < 10) return 'Description must be at least 10 characters'
  if (description.length > 1000) return 'Description must be less than 1000 characters'
  return null
}

export const validateCategory = (
  category: string,
  availableCategories: string[]
): string | null => {
  if (!category) return 'Category is required'
  if (!availableCategories.includes(category)) return 'Please select a valid category'
  return null
}

export const validateProgressPercent = (
  percent: number,
  currentProgress: number
): string | null => {
  if (percent < 0) return 'Progress must be greater than or equal to 0'
  if (percent > 100) return 'Progress cannot exceed 100%'
  if (currentProgress + percent > 100) return 'Total progress cannot exceed 100%'
  return null
}
