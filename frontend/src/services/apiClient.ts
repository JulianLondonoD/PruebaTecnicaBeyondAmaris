import axios, { AxiosError, AxiosInstance, InternalAxiosRequestConfig, AxiosResponse } from 'axios'

// Get API URL from environment or default to proxy in dev
// In production: Uses VITE_API_URL if set, otherwise falls back to '/api/v1'
// In development: Always uses '/api/v1' which is proxied to the backend
const getApiBaseURL = () => {
  if (import.meta.env.PROD) {
    return import.meta.env.VITE_API_URL || '/api/v1'
  }
  // In development, use proxy
  return '/api/v1'
}

const apiClient: AxiosInstance = axios.create({
  baseURL: getApiBaseURL(),
  headers: {
    'Content-Type': 'application/json'
  },
  timeout: 10000
})

// Request interceptor for logging
apiClient.interceptors.request.use(
  (config: InternalAxiosRequestConfig) => {
    console.log(`🚀 API Request: ${config.method?.toUpperCase()} ${config.baseURL}${config.url}`)
    return config
  },
  (error: AxiosError) => {
    return Promise.reject(error)
  }
)

interface ErrorResponse {
  message?: string
  title?: string
  errors?: Record<string, string[]>
}

// Response interceptor for error handling and logging
apiClient.interceptors.response.use(
  (response: AxiosResponse) => {
    console.log(`✅ API Response: ${response.config.url} → ${response.status}`)
    return response
  },
  (error: AxiosError<ErrorResponse>) => {
    const message = error.response?.data?.message || error.message
    console.error(`❌ API Error: ${error.config?.url} → ${error.response?.status} ${message}`)

    // Enhanced error details for development
    if (import.meta.env.DEV) {
      console.error('Full error details:', {
        url: error.config?.url,
        method: error.config?.method,
        status: error.response?.status,
        statusText: error.response?.statusText,
        data: error.response?.data
      })
    }

    let errorMessage = 'An unexpected error occurred'

    if (error.response) {
      const data = error.response.data
      errorMessage = data?.message || data?.title || `Error: ${error.response.status}`
    } else if (error.request) {
      errorMessage = 'No response from server. Please check your connection.'
    } else {
      errorMessage = error.message
    }

    return Promise.reject(new Error(errorMessage))
  }
)

export default apiClient
