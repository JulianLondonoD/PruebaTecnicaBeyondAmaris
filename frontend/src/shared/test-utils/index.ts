/**
 * Test Utilities
 * Provides helper functions and custom render utilities for testing
 */

import { render } from '@testing-library/vue'
import { createPinia } from 'pinia'
import { VueQueryPlugin } from '@tanstack/vue-query'
import { vi } from 'vitest'

/**
 * Custom render function that includes all necessary providers
 * This ensures components are tested with the same setup as production
 */
export function renderWithProviders(component: any, options: any = {}) {
  const pinia = createPinia()

  return render(component, {
    ...options,
    global: {
      ...options.global,
      plugins: [
        pinia,
        [
          VueQueryPlugin,
          {
            queryClientConfig: {
              defaultOptions: {
                queries: {
                  retry: false,
                  cacheTime: 0
                }
              }
            }
          }
        ],
        ...(options.global?.plugins || [])
      ]
    }
  })
}

/**
 * Creates a mock for Pinia store testing
 */
export function createMockStore() {
  return createPinia()
}

/**
 * Wait for async updates in tests
 */
export async function flushPromises() {
  return new Promise(resolve => setTimeout(resolve, 0))
}

/**
 * Mock fetch for API testing
 */
export function createMockFetch(responses: Record<string, any>) {
  return vi.fn((url: string) => {
    const response = responses[url]
    if (!response) {
      return Promise.reject(new Error(`No mock response for ${url}`))
    }

    return Promise.resolve({
      ok: true,
      status: 200,
      json: () => Promise.resolve(response)
    })
  })
}
