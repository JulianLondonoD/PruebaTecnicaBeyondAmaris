import { ref } from 'vue'
import apiClient from '@/services/apiClient'
import { useNotifications } from './useNotifications'
import type { ApiError } from '@/types/api'
import type { AxiosResponse } from 'axios'

export function useApi() {
  const isLoading = ref(false)
  const error = ref<ApiError | null>(null)
  const { error: addError, success: addSuccess } = useNotifications()

  const handleRequest = async <T>(
    request: () => Promise<T>,
    options?: {
      showSuccess?: boolean
      successMessage?: string
      showError?: boolean
      errorTitle?: string
    }
  ): Promise<T> => {
    const config = {
      showSuccess: false,
      showError: true,
      errorTitle: 'Request Failed',
      ...options
    }

    isLoading.value = true
    error.value = null

    try {
      const result = await request()

      if (config.showSuccess) {
        addSuccess(config.successMessage || 'Operation completed successfully')
      }

      return result
    } catch (err: any) {
      const apiError: ApiError = {
        message: err.response?.data?.message || err.message || 'An unexpected error occurred',
        code: err.response?.status?.toString(),
        details: err.response?.data
      }

      error.value = apiError

      if (config.showError) {
        addError(apiError.message)
      }

      throw apiError
    } finally {
      isLoading.value = false
    }
  }

  const get = <T>(url: string, config?: any) => {
    return handleRequest(() =>
      apiClient.get<T>(url, config).then((res: AxiosResponse<T>) => res.data)
    )
  }

  const post = <T>(url: string, data?: any, config?: any) => {
    return handleRequest(() =>
      apiClient.post<T>(url, data, config).then((res: AxiosResponse<T>) => res.data)
    )
  }

  const put = <T>(url: string, data?: any, config?: any) => {
    return handleRequest(() =>
      apiClient.put<T>(url, data, config).then((res: AxiosResponse<T>) => res.data)
    )
  }

  const del = <T>(url: string, config?: any) => {
    return handleRequest(() =>
      apiClient.delete<T>(url, config).then((res: AxiosResponse<T>) => res.data)
    )
  }

  return {
    isLoading,
    error,
    handleRequest,
    get,
    post,
    put,
    delete: del,
    client: apiClient
  }
}
