/**
 * Todo Store Tests
 */

import { describe, it, expect, beforeEach } from 'vitest'
import { setActivePinia, createPinia } from 'pinia'
import { useTodoStore } from '@/features/todos/stores/useTodoStore'
import { createMockTodo } from '../mocks/factories'

describe('useTodoStore', () => {
  beforeEach(() => {
    setActivePinia(createPinia())
  })

  it('should initialize with empty state', () => {
    const store = useTodoStore()
    expect(store.todos).toEqual([])
    expect(store.loading).toBe(false)
    expect(store.error).toBe(null)
    expect(store.filter).toBe('all')
  })

  it('should add todos', () => {
    const store = useTodoStore()
    const todo = createMockTodo({ title: 'Test Todo' })
    
    store.addTodo(todo)
    
    expect(store.todos).toHaveLength(1)
    expect(store.todos[0].title).toBe('Test Todo')
  })

  it('should update todos', () => {
    const store = useTodoStore()
    const todo = createMockTodo({ id: 1, title: 'Original' })
    
    store.addTodo(todo)
    store.updateTodo(1, { title: 'Updated' })
    
    expect(store.todos[0].title).toBe('Updated')
  })

  it('should remove todos', () => {
    const store = useTodoStore()
    const todo = createMockTodo({ id: 1 })
    
    store.addTodo(todo)
    expect(store.todos).toHaveLength(1)
    
    store.removeTodo(1)
    expect(store.todos).toHaveLength(0)
  })

  it('should toggle todo completion', () => {
    const store = useTodoStore()
    const todo = createMockTodo({ id: 1, isCompleted: false })
    
    store.addTodo(todo)
    store.toggleTodo(1)
    
    expect(store.todos[0].isCompleted).toBe(true)
    
    store.toggleTodo(1)
    expect(store.todos[0].isCompleted).toBe(false)
  })

  it('should filter todos by status', () => {
    const store = useTodoStore()
    store.addTodo(createMockTodo({ id: 1, isCompleted: false }))
    store.addTodo(createMockTodo({ id: 2, isCompleted: true }))
    store.addTodo(createMockTodo({ id: 3, isCompleted: false }))
    
    store.setFilter('active')
    expect(store.filteredTodos).toHaveLength(2)
    
    store.setFilter('completed')
    expect(store.filteredTodos).toHaveLength(1)
    
    store.setFilter('all')
    expect(store.filteredTodos).toHaveLength(3)
  })

  it('should filter todos by search query', () => {
    const store = useTodoStore()
    store.addTodo(createMockTodo({ title: 'Buy groceries' }))
    store.addTodo(createMockTodo({ title: 'Write code' }))
    store.addTodo(createMockTodo({ title: 'Buy tickets' }))
    
    store.setSearchQuery('buy')
    expect(store.filteredTodos).toHaveLength(2)
    
    store.setSearchQuery('code')
    expect(store.filteredTodos).toHaveLength(1)
  })

  it('should filter todos by category', () => {
    const store = useTodoStore()
    store.addTodo(createMockTodo({ category: 'Work' }))
    store.addTodo(createMockTodo({ category: 'Personal' }))
    store.addTodo(createMockTodo({ category: 'Work' }))
    
    store.setSelectedCategory('Work')
    expect(store.filteredTodos).toHaveLength(2)
    
    store.setSelectedCategory('Personal')
    expect(store.filteredTodos).toHaveLength(1)
  })

  it('should count completed and active todos', () => {
    const store = useTodoStore()
    store.addTodo(createMockTodo({ isCompleted: false }))
    store.addTodo(createMockTodo({ isCompleted: true }))
    store.addTodo(createMockTodo({ isCompleted: true }))
    
    expect(store.activeCount).toBe(1)
    expect(store.completedCount).toBe(2)
    expect(store.totalCount).toBe(3)
  })

  it('should extract unique categories', () => {
    const store = useTodoStore()
    store.addTodo(createMockTodo({ category: 'Work' }))
    store.addTodo(createMockTodo({ category: 'Personal' }))
    store.addTodo(createMockTodo({ category: 'Work' }))
    
    expect(store.categories).toHaveLength(2)
    expect(store.categories).toContain('Work')
    expect(store.categories).toContain('Personal')
  })

  it('should clear completed todos', () => {
    const store = useTodoStore()
    store.addTodo(createMockTodo({ isCompleted: false }))
    store.addTodo(createMockTodo({ isCompleted: true }))
    store.addTodo(createMockTodo({ isCompleted: true }))
    
    store.clearCompleted()
    
    expect(store.todos).toHaveLength(1)
    expect(store.todos[0].isCompleted).toBe(false)
  })

  it('should reset store to initial state', () => {
    const store = useTodoStore()
    store.addTodo(createMockTodo())
    store.setFilter('active')
    store.setSearchQuery('test')
    store.setLoading(true)
    
    store.$reset()
    
    expect(store.todos).toEqual([])
    expect(store.filter).toBe('all')
    expect(store.searchQuery).toBe('')
    expect(store.loading).toBe(false)
  })
})
