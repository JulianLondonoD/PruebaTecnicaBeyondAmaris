<template>
  <div class="space-y-4">
    <div v-if="isLoading" class="flex justify-center py-12">
      <LoadingSpinner size="lg" />
    </div>

    <div v-else-if="error" class="text-center py-12">
      <p class="text-red-600">{{ error.message }}</p>
    </div>

    <div v-else-if="!todos || todos.length === 0" class="text-center py-12">
      <p class="text-gray-500 text-lg">No todos yet. Create your first one!</p>
    </div>

    <div v-else class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
      <TodoCard
        v-for="todo in todos"
        :key="todo.id"
        :todo="todo"
        @update="handleUpdate"
        @delete="handleDelete"
      />
    </div>
  </div>
</template>

<script setup lang="ts">
import type { TodoItem } from '@/types/todo'
import TodoCard from './TodoCard.vue'
import LoadingSpinner from '../ui/LoadingSpinner.vue'

interface Props {
  todos?: TodoItem[]
  isLoading?: boolean
  error?: Error | null
}

defineProps<Props>()

const emit = defineEmits<{
  update: [id: number, data: any]
  delete: [id: number]
}>()

const handleUpdate = (id: number, data: any) => {
  emit('update', id, data)
}

const handleDelete = (id: number) => {
  emit('delete', id)
}
</script>
