/**
 * Mock Data Factories
 * Provides factory functions to generate mock data for testing
 */

import type { TodoItem } from '@/types/todo'

let idCounter = 1

/**
 * Factory function to create mock todos
 */
export function createMockTodo(overrides: Partial<TodoItem> = {}): TodoItem {
  const id = idCounter++
  
  return {
    id,
    title: `Mock Todo ${id}`,
    description: `Description for mock todo ${id}`,
    category: 'Work',
    isCompleted: false,
    totalProgress: 0,
    progressions: [],
    ...overrides,
  }
}

/**
 * Factory to create multiple mock todos
 */
export function createMockTodos(count: number, overrides: Partial<TodoItem> = {}): TodoItem[] {
  return Array.from({ length: count }, () => createMockTodo(overrides))
}

/**
 * Reset the ID counter (useful between tests)
 */
export function resetMockIds() {
  idCounter = 1
}

/**
 * Mock API responses
 */
export const mockApiResponses = {
  todos: {
    list: () => createMockTodos(5),
    single: () => createMockTodo(),
    create: (data: Partial<TodoItem>) => createMockTodo(data),
    update: (id: number, data: Partial<TodoItem>) => createMockTodo({ id, ...data }),
  },
  categories: {
    list: () => ['Work', 'Personal', 'Shopping', 'Health'],
  },
}
