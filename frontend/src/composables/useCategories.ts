import { computed } from 'vue'
import { useQuery } from '@tanstack/vue-query'
import { todoService } from '@/services/todoService'
import { appConfig } from '@/config/app.config'

export function useCategories() {
  const {
    data: categoriesData,
    isLoading: categoriesLoading,
    error: categoriesError,
    refetch: refetchCategories
  } = useQuery({
    queryKey: ['categories'],
    queryFn: todoService.getCategories,
    staleTime: appConfig.cache.categoriesStaleTime,
    gcTime: appConfig.cache.categoriesGcTime,
    retry: appConfig.api.retries,
    retryDelay: attemptIndex => Math.min(1000 * 2 ** attemptIndex, 30000)
  })

  const categories = computed(() => categoriesData.value || [])

  const categoryOptions = computed(() =>
    categories.value.map(category => ({
      value: category,
      label: category,
      text: category
    }))
  )

  const isValidCategory = (category: string) => {
    return categories.value.includes(category)
  }

  const getCategoryCount = () => categories.value.length

  return {
    categories,
    categoryOptions,
    categoriesLoading,
    categoriesError,
    refetchCategories,
    isValidCategory,
    getCategoryCount
  }
}
