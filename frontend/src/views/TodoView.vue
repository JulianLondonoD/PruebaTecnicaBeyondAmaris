<template>
  <div class="min-h-screen bg-gray-100">
    <AppHeader :stats="stats" />

    <main class="container mx-auto px-4 py-8">
      <div class="mb-8">
        <TodoForm :is-submitting="isCreating" @submit="handleCreateTodo" />
      </div>

      <div>
        <TodoList
          :todos="todos"
          :is-loading="isLoading"
          :error="queryError"
          @update="handleUpdateTodo"
          @delete="handleDeleteTodo"
        />
      </div>
    </main>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useTodos } from '@/composables/useTodos'
import AppHeader from '@/components/layout/AppHeader.vue'
import TodoForm from '@/components/todo/TodoForm.vue'
import TodoList from '@/components/todo/TodoList.vue'
import type { CreateTodoRequest, UpdateTodoRequest } from '@/types/todo'

const {
  todos,
  isLoading,
  queryError,
  totalTodos,
  completedTodos,
  inProgressTodos,
  notStartedTodos,
  createTodo,
  updateTodo,
  deleteTodo,
  isCreating
} = useTodos()

const stats = computed(() => ({
  total: totalTodos.value,
  completed: completedTodos.value,
  inProgress: inProgressTodos.value,
  notStarted: notStartedTodos.value
}))

const handleCreateTodo = (data: CreateTodoRequest) => {
  createTodo(data)
}

const handleUpdateTodo = (id: number, data: UpdateTodoRequest) => {
  updateTodo({ id, data })
}

const handleDeleteTodo = (id: number) => {
  deleteTodo(id)
}
</script>
