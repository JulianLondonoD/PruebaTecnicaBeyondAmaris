import { computed } from 'vue'
import { useMutation, useQueryClient } from '@tanstack/vue-query'
import { todoService } from '@/services/todoService'
import { useNotifications } from './useNotifications'
import type { AddProgressionRequest, TodoItem } from '@/types/todo'
import {
  calculateProgressColor,
  getProgressStatus,
  getProgressColorClass,
  calculateEstimatedCompletion
} from '@/utils/progressHelpers'

export function useProgressions() {
  const queryClient = useQueryClient()
  const { addNotification } = useNotifications()

  const addProgressionMutation = useMutation({
    mutationFn: todoService.addProgression,

    // ✅ OPTIMISTIC UPDATE - Actualiza UI inmediatamente
    onMutate: async (variables: AddProgressionRequest) => {
      // Cancelar queries en curso para evitar conflictos
      await queryClient.cancelQueries({ queryKey: ['todos'] })
      await queryClient.cancelQueries({ queryKey: ['todos', variables.id] })

      // Snapshot de datos actuales para rollback
      const previousTodos = queryClient.getQueryData(['todos'])
      const previousTodo = queryClient.getQueryData(['todos', variables.id])

      // Actualizar cache optimísticamente
      queryClient.setQueryData(['todos'], (oldTodos: TodoItem[] | undefined) => {
        if (!oldTodos) return oldTodos

        return oldTodos.map(todo => {
          if (todo.id === variables.id) {
            const newProgress = Math.min(100, todo.totalProgress + variables.percent)
            return {
              ...todo,
              totalProgress: newProgress,
              isCompleted: newProgress >= 100,
              progressions: [
                ...todo.progressions,
                {
                  dateTime: variables.dateTime,
                  percent: variables.percent,
                  accumulatedPercent: newProgress
                }
              ].sort((a, b) => new Date(b.dateTime).getTime() - new Date(a.dateTime).getTime())
            }
          }
          return todo
        })
      })

      // Actualizar cache individual del todo
      queryClient.setQueryData(['todos', variables.id], (oldTodo: TodoItem | undefined) => {
        if (!oldTodo) return oldTodo

        const newProgress = Math.min(100, oldTodo.totalProgress + variables.percent)
        return {
          ...oldTodo,
          totalProgress: newProgress,
          isCompleted: newProgress >= 100,
          progressions: [
            ...oldTodo.progressions,
            {
              dateTime: variables.dateTime,
              percent: variables.percent,
              accumulatedPercent: newProgress
            }
          ].sort((a, b) => new Date(b.dateTime).getTime() - new Date(a.dateTime).getTime())
        }
      })

      // Mostrar feedback inmediato
      addNotification({
        type: 'info',
        title: 'Adding Progress',
        message: `Adding ${variables.percent}% progress...`,
        duration: 2000
      })

      return { previousTodos, previousTodo, todoId: variables.id }
    },

    // ✅ SUCCESS - Confirmar cambios y sincronizar
    onSuccess: (_, variables) => {
      // Invalidar queries para sincronizar con server
      queryClient.invalidateQueries({ queryKey: ['todos'] })
      queryClient.invalidateQueries({ queryKey: ['todos', variables.id] })

      addNotification({
        type: 'success',
        title: 'Progress Updated',
        message: `Successfully added ${variables.percent}% progress`,
        duration: 3000
      })
    },

    // ✅ ERROR - Rollback cambios optimistas
    onError: (error: any, _variables, context) => {
      // Rollback a estado anterior
      if (context?.previousTodos) {
        queryClient.setQueryData(['todos'], context.previousTodos)
      }
      if (context?.previousTodo) {
        queryClient.setQueryData(['todos', context.todoId], context.previousTodo)
      }

      addNotification({
        type: 'error',
        title: 'Progress Update Failed',
        message: error.message || 'Failed to update progress',
        duration: 5000
      })
    },

    // ✅ SETTLED - Cleanup
    onSettled: () => {
      queryClient.invalidateQueries({ queryKey: ['todos'] })
    }
  })

  const addProgression = async (data: Omit<AddProgressionRequest, 'dateTime'>) => {
    return addProgressionMutation.mutateAsync({
      ...data,
      dateTime: new Date().toISOString()
    })
  }

  return {
    addProgression,
    calculateProgressColor,
    getProgressStatus,
    getProgressColorClass,
    calculateEstimatedCompletion,
    isAddingProgression: computed(() => addProgressionMutation.isPending),
    addProgressionError: computed(() => addProgressionMutation.error)
  }
}
