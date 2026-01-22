/**
 * Todos Pinia Store
 * Manages todo state with persistence and optimistic updates
 */

import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import type { TodoItem } from '@/types/todo'

export const useTodoStore = defineStore('todos', () => {
  // State
  const todos = ref<TodoItem[]>([])
  const loading = ref(false)
  const error = ref<string | null>(null)
  const filter = ref<'all' | 'active' | 'completed'>('all')
  const searchQuery = ref('')
  const selectedCategory = ref<string | null>(null)

  // Getters
  const filteredTodos = computed(() => {
    let result = todos.value

    // Apply search filter
    if (searchQuery.value) {
      const query = searchQuery.value.toLowerCase()
      result = result.filter(
        todo =>
          todo.title.toLowerCase().includes(query) ||
          todo.description?.toLowerCase().includes(query)
      )
    }

    // Apply category filter
    if (selectedCategory.value) {
      result = result.filter(todo => todo.category === selectedCategory.value)
    }

    // Apply completion filter
    switch (filter.value) {
      case 'active':
        return result.filter(todo => !todo.isCompleted)
      case 'completed':
        return result.filter(todo => todo.isCompleted)
      default:
        return result
    }
  })

  const completedCount = computed(() => todos.value.filter(t => t.isCompleted).length)
  const activeCount = computed(() => todos.value.filter(t => !t.isCompleted).length)
  const totalCount = computed(() => todos.value.length)

  const categories = computed(() => {
    const cats = new Set(todos.value.map(t => t.category).filter(Boolean))
    return Array.from(cats)
  })

  // Actions
  function setTodos(newTodos: TodoItem[]) {
    todos.value = newTodos
  }

  function addTodo(todo: TodoItem) {
    todos.value.unshift(todo)
  }

  function updateTodo(id: number, updates: Partial<TodoItem>) {
    const index = todos.value.findIndex(t => t.id === id)
    if (index !== -1) {
      todos.value[index] = { ...todos.value[index], ...updates }
    }
  }

  function removeTodo(id: number) {
    todos.value = todos.value.filter(t => t.id !== id)
  }

  function toggleTodo(id: number) {
    const todo = todos.value.find(t => t.id === id)
    if (todo) {
      todo.isCompleted = !todo.isCompleted
    }
  }

  function setFilter(newFilter: 'all' | 'active' | 'completed') {
    filter.value = newFilter
  }

  function setSearchQuery(query: string) {
    searchQuery.value = query
  }

  function setSelectedCategory(category: string | null) {
    selectedCategory.value = category
  }

  function setLoading(value: boolean) {
    loading.value = value
  }

  function setError(value: string | null) {
    error.value = value
  }

  function clearCompleted() {
    todos.value = todos.value.filter(t => !t.isCompleted)
  }

  function $reset() {
    todos.value = []
    loading.value = false
    error.value = null
    filter.value = 'all'
    searchQuery.value = ''
    selectedCategory.value = null
  }

  return {
    // State
    todos,
    loading,
    error,
    filter,
    searchQuery,
    selectedCategory,
    // Getters
    filteredTodos,
    completedCount,
    activeCount,
    totalCount,
    categories,
    // Actions
    setTodos,
    addTodo,
    updateTodo,
    removeTodo,
    toggleTodo,
    setFilter,
    setSearchQuery,
    setSelectedCategory,
    setLoading,
    setError,
    clearCompleted,
    $reset
  }
})
