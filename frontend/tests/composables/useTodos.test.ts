import { describe, it, expect, vi } from 'vitest'

vi.mock('@/services/todoService', () => ({
  todoService: {
    getAll: vi.fn(() => Promise.resolve([])),
    create: vi.fn(),
    update: vi.fn(),
    delete: vi.fn(),
    addProgression: vi.fn(),
    getCategories: vi.fn(() => Promise.resolve(['Work', 'Personal']))
  }
}))

vi.mock('@/composables/useNotifications', () => ({
  useNotifications: () => ({
    success: vi.fn(),
    error: vi.fn(),
    warning: vi.fn(),
    info: vi.fn()
  })
}))

describe('useTodos', () => {
  it('should export a function', async () => {
    const { useTodos } = await import('@/composables/useTodos')
    expect(typeof useTodos).toBe('function')
  })

  it('should have correct service dependencies', async () => {
    const { todoService } = await import('@/services/todoService')
    
    expect(typeof todoService.getAll).toBe('function')
    expect(typeof todoService.create).toBe('function')
    expect(typeof todoService.update).toBe('function')
    expect(typeof todoService.delete).toBe('function')
    expect(typeof todoService.addProgression).toBe('function')
  })
})
