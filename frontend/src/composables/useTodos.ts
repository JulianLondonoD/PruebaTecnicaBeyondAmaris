import { computed } from 'vue'
import { useQuery, useMutation, useQueryClient } from '@tanstack/vue-query'
import { todoService } from '@/services/todoService'
import { useNotifications } from './useNotifications'
import type { CreateTodoRequest, UpdateTodoRequest, AddProgressionRequest } from '@/types/todo'

export function useTodos() {
  const queryClient = useQueryClient()
  const { success, error } = useNotifications()

  const {
    data: todos,
    isLoading,
    error: queryError
  } = useQuery({
    queryKey: ['todos'],
    queryFn: todoService.getAll,
    refetchOnWindowFocus: false
  })

  const createMutation = useMutation({
    mutationFn: (data: CreateTodoRequest) => todoService.create(data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['todos'] })
      success('Todo created successfully')
    },
    onError: (err: Error) => {
      error(err.message || 'Failed to create todo')
    }
  })

  const updateMutation = useMutation({
    mutationFn: ({ id, data }: { id: number; data: UpdateTodoRequest }) =>
      todoService.update(id, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['todos'] })
      success('Todo updated successfully')
    },
    onError: (err: Error) => {
      error(err.message || 'Failed to update todo')
    }
  })

  const deleteMutation = useMutation({
    mutationFn: (id: number) => todoService.delete(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['todos'] })
      success('Todo deleted successfully')
    },
    onError: (err: Error) => {
      error(err.message || 'Failed to delete todo')
    }
  })

  const addProgressionMutation = useMutation({
    mutationFn: (data: AddProgressionRequest) => todoService.addProgression(data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['todos'] })
      success('Progress added successfully')
    },
    onError: (err: Error) => {
      error(err.message || 'Failed to add progress')
    }
  })

  const { data: categories } = useQuery({
    queryKey: ['categories'],
    queryFn: todoService.getCategories
  })

  const totalTodos = computed(() => todos.value?.length || 0)
  const completedTodos = computed(() => todos.value?.filter(todo => todo.isCompleted).length || 0)
  const inProgressTodos = computed(
    () => todos.value?.filter(todo => !todo.isCompleted && todo.totalProgress > 0).length || 0
  )
  const notStartedTodos = computed(
    () => todos.value?.filter(todo => todo.totalProgress === 0).length || 0
  )

  return {
    todos,
    isLoading,
    queryError,
    categories,
    totalTodos,
    completedTodos,
    inProgressTodos,
    notStartedTodos,
    createTodo: createMutation.mutate,
    updateTodo: updateMutation.mutate,
    deleteTodo: deleteMutation.mutate,
    addProgression: addProgressionMutation.mutate,
    isCreating: createMutation.isPending,
    isUpdating: updateMutation.isPending,
    isDeleting: deleteMutation.isPending,
    isAddingProgress: addProgressionMutation.isPending
  }
}
