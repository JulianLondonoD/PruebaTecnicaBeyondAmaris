// Debug helpers for development
export const logApiCall = (method: string, url: string, data?: any) => {
  if (import.meta.env.DEV) {
    console.group(`🌐 API Call: ${method} ${url}`)
    if (data) {
      console.log('📤 Request Data:', data)
    }
    console.groupEnd()
  }
}

export const logApiResponse = (url: string, status: number, data?: any) => {
  if (import.meta.env.DEV) {
    console.group(`📡 API Response: ${status} ${url}`)
    if (data) {
      console.log('📥 Response Data:', data)
    }
    console.groupEnd()
  }
}

export const logApiError = (url: string, error: any) => {
  if (import.meta.env.DEV) {
    console.group(`❌ API Error: ${url}`)
    console.error('Error Details:', error)
    console.groupEnd()
  }
}

// Test function to verify API connectivity
export const testApiEndpoint = async (endpoint: string) => {
  // Validate endpoint is relative and starts with /api/
  if (!endpoint.startsWith('/api/')) {
    console.error('❌ Invalid endpoint: Must start with /api/')
    return false
  }

  try {
    console.log(`🧪 Testing endpoint: ${endpoint}`)
    const response = await fetch(endpoint, { method: 'GET' })
    console.log(`✅ Endpoint ${endpoint} responded with status: ${response.status}`)
    return response.ok
  } catch (error) {
    console.error(`❌ Endpoint ${endpoint} failed:`, error)
    return false
  }
}

// Validate todo data structure
export const validateTodoData = (todo: any): boolean => {
  const requiredFields = ['id', 'title', 'description', 'category', 'totalProgress']
  const missingFields = requiredFields.filter(field => !(field in todo))

  if (missingFields.length > 0) {
    console.warn('Invalid todo data - missing fields:', missingFields)
    return false
  }

  if (
    typeof todo.totalProgress !== 'number' ||
    todo.totalProgress < 0 ||
    todo.totalProgress > 100
  ) {
    console.warn('Invalid totalProgress value:', todo.totalProgress)
    return false
  }

  return true
}
