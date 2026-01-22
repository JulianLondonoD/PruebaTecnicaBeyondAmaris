<template>
  <div
    class="bg-white rounded-lg shadow-md border border-gray-200 p-6 hover:shadow-lg transition-all duration-200"
    :class="{
      'ring-2 ring-green-200 bg-green-50': isCompleted,
      'ring-2 ring-blue-200 bg-blue-50': isInProgress && !isCompleted
    }"
  >
    <!-- Header -->
    <div class="flex justify-between items-start mb-4">
      <div class="flex-1">
        <div class="flex items-center space-x-2">
          <h3
            class="text-lg font-semibold text-gray-900 mb-1"
            :class="{ 'line-through text-gray-600': isCompleted }"
          >
            {{ todo.title }}
          </h3>
          <span v-if="isCompleted" class="text-green-600">
            <svg class="w-5 h-5" fill="currentColor" viewBox="0 0 20 20">
              <path
                fill-rule="evenodd"
                d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z"
                clip-rule="evenodd"
              />
            </svg>
          </span>
        </div>
        <p class="text-gray-600 text-sm leading-relaxed" :class="{ 'line-through': isCompleted }">
          {{ todo.description }}
        </p>
      </div>

      <!-- Category badge -->
      <span
        class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-blue-100 text-blue-800"
      >
        {{ todo.category }}
      </span>
    </div>

    <!-- Progress Section -->
    <div class="mb-4">
      <div class="flex justify-between items-center mb-2">
        <span class="text-sm font-medium text-gray-700">Progress</span>
        <span
          class="text-sm font-semibold"
          :class="{
            'text-green-600': isCompleted,
            'text-blue-600': isInProgress && !isCompleted,
            'text-gray-500': !isInProgress && !isCompleted
          }"
        >
          {{ Math.round(todo.totalProgress) }}% / 100%
        </span>
      </div>

      <div class="w-full bg-gray-200 rounded-full h-3 overflow-hidden">
        <div
          class="h-3 rounded-full transition-all duration-700 ease-out"
          :class="progressColorClass"
          :style="{ width: `${Math.min(100, todo.totalProgress)}%` }"
        ></div>
      </div>

      <div class="flex justify-between items-center mt-2">
        <span class="text-xs" :class="statusTextClass">
          {{ statusText }}
        </span>
        <span v-if="isCompleted" class="text-xs text-green-600 font-medium"> 🎉 Completed! </span>
        <span v-else class="text-xs text-gray-400">
          {{ Math.round(100 - todo.totalProgress) }}% remaining
        </span>
      </div>
    </div>

    <!-- Recent Progressions -->
    <div v-if="hasProgressions" class="mb-4">
      <h4 class="text-sm font-medium text-gray-700 mb-2">Recent Updates</h4>
      <div class="space-y-1">
        <div
          v-for="(progression, index) in recentProgressions"
          :key="index"
          class="flex justify-between items-center text-xs bg-gray-50 rounded px-2 py-1"
        >
          <span class="text-green-600 font-medium">+{{ progression.percent }}%</span>
          <span class="text-gray-500">{{ formatRelativeTime(progression.dateTime) }}</span>
        </div>
      </div>
    </div>

    <!-- Completion Celebration -->
    <div
      v-if="isCompleted"
      class="mb-4 bg-green-100 border border-green-200 rounded-lg p-3 text-center"
    >
      <div class="flex items-center justify-center space-x-2">
        <span class="text-2xl">🎉</span>
        <div>
          <p class="text-green-800 font-semibold text-sm">Task Completed!</p>
          <p class="text-green-600 text-xs">Congratulations on finishing this task.</p>
        </div>
      </div>
    </div>

    <!-- Action Buttons -->
    <div class="flex justify-between items-center pt-4 border-t border-gray-200">
      <div class="flex space-x-2">
        <button
          @click="openProgressModal"
          :disabled="isCompleted"
          class="px-3 py-2 text-sm rounded-md transition-colors focus:ring-2 focus:ring-offset-1"
          :class="
            isCompleted
              ? 'bg-green-100 text-green-700 cursor-not-allowed'
              : 'bg-blue-600 text-white hover:bg-blue-700 focus:ring-blue-500'
          "
        >
          <span v-if="isCompleted" class="flex items-center space-x-1">
            <svg class="w-4 h-4" fill="currentColor" viewBox="0 0 20 20">
              <path
                fill-rule="evenodd"
                d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z"
                clip-rule="evenodd"
              />
            </svg>
            <span>Complete</span>
          </span>
          <span v-else>Add Progress</span>
        </button>

        <button
          @click="openEditModal"
          class="px-3 py-2 text-sm bg-gray-600 text-white rounded-md hover:bg-gray-700 transition-colors focus:ring-2 focus:ring-gray-500 focus:ring-offset-1"
        >
          <span class="flex items-center space-x-1">
            <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path
                stroke-linecap="round"
                stroke-linejoin="round"
                stroke-width="2"
                d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z"
              ></path>
            </svg>
            <span>Edit</span>
          </span>
        </button>
      </div>

      <div class="flex items-center space-x-3">
        <span class="text-xs text-gray-400">ID: {{ todo.id }}</span>
        <button
          @click="handleDelete"
          class="p-2 text-red-600 hover:text-red-700 hover:bg-red-50 rounded-full transition-colors focus:ring-2 focus:ring-red-500 focus:ring-offset-1"
          title="Delete todo"
        >
          <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path
              stroke-linecap="round"
              stroke-linejoin="round"
              stroke-width="2"
              d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16"
            ></path>
          </svg>
        </button>
      </div>
    </div>

    <!-- Progress Modal -->
    <AddProgressModal
      :is-open="showProgressModal"
      :todo="todo"
      @close="showProgressModal = false"
      @success="handleProgressSuccess"
    />

    <!-- Edit Modal -->
    <EditTodoModal
      :is-open="showEditModal"
      :todo="todo"
      @close="showEditModal = false"
      @success="handleEditSuccess"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import AddProgressModal from './AddProgressModal.vue'
import EditTodoModal from './EditTodoModal.vue'

// Types
interface TodoItem {
  id: number
  title: string
  description: string
  category: string
  totalProgress: number
  isCompleted: boolean
  progressions?: Array<{
    dateTime: string
    percent: number
    accumulatedPercent: number
  }>
}

interface Props {
  todo: TodoItem
}

interface Emits {
  (e: 'edit', todo: TodoItem): void
  (e: 'delete', id: number): void
  (e: 'progressUpdated', todo: TodoItem): void
  (e: 'todoUpdated', todo: TodoItem): void
}

// Props and Emits
const props = defineProps<Props>()
const emit = defineEmits<Emits>()

// Reactive Data
const showProgressModal = ref(false)
const showEditModal = ref(false)

// Computed Properties
const isCompleted = computed(() => props.todo.totalProgress >= 100 || props.todo.isCompleted)
const isInProgress = computed(() => props.todo.totalProgress > 0 && !isCompleted.value)

const progressColorClass = computed(() => {
  const progress = props.todo.totalProgress
  if (progress === 0) return 'bg-gray-300'
  if (progress < 25) return 'bg-red-500'
  if (progress < 50) return 'bg-yellow-500'
  if (progress < 75) return 'bg-blue-500'
  if (progress < 100) return 'bg-green-500'
  return 'bg-emerald-600'
})

const statusText = computed(() => {
  if (isCompleted.value) return 'Completed'
  if (isInProgress.value) return 'In Progress'
  return 'Not Started'
})

const statusTextClass = computed(() => {
  if (isCompleted.value) return 'text-green-600 font-medium'
  if (isInProgress.value) return 'text-blue-600 font-medium'
  return 'text-gray-500'
})

const hasProgressions = computed(() => {
  return props.todo.progressions && props.todo.progressions.length > 0
})

const recentProgressions = computed(() => {
  if (!props.todo.progressions) return []
  return [...props.todo.progressions]
    .sort((a, b) => new Date(b.dateTime).getTime() - new Date(a.dateTime).getTime())
    .slice(0, 3)
})

// Methods
const openProgressModal = () => {
  if (!isCompleted.value) {
    showProgressModal.value = true
  }
}

const openEditModal = () => {
  console.log('🖊️ Opening edit modal for todo:', props.todo.id)
  showEditModal.value = true
}

const handleProgressSuccess = (result?: any) => {
  console.log('Progress updated successfully:', result)
  emit('progressUpdated', props.todo)
  showProgressModal.value = false
}

const handleEditSuccess = (updatedTodo: TodoItem) => {
  console.log('✅ Todo updated successfully:', updatedTodo)
  emit('todoUpdated', updatedTodo)
  showEditModal.value = false
}

const handleDelete = () => {
  const confirmMessage = `Are you sure you want to delete "${props.todo.title}"?`

  if (confirm(confirmMessage)) {
    console.log('🗑️ Deleting todo:', props.todo.id)
    emit('delete', props.todo.id)
  }
}

const formatRelativeTime = (dateTime: string): string => {
  const date = new Date(dateTime)
  const now = new Date()
  const diffMs = now.getTime() - date.getTime()

  const diffMinutes = Math.floor(diffMs / (1000 * 60))
  const diffHours = Math.floor(diffMs / (1000 * 60 * 60))
  const diffDays = Math.floor(diffMs / (1000 * 60 * 60 * 24))

  if (diffMinutes < 1) return 'Just now'
  if (diffMinutes < 60) return `${diffMinutes}m ago`
  if (diffHours < 24) return `${diffHours}h ago`
  if (diffDays < 7) return `${diffDays}d ago`

  return date.toLocaleDateString()
}
</script>

<style scoped>
/* Smooth line-through animation */
.line-through {
  text-decoration-line: line-through;
  text-decoration-thickness: 2px;
  text-decoration-color: currentColor;
}

/* Button hover effects */
button:hover:not(:disabled) {
  transform: translateY(-1px);
  box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1);
}

button:active:not(:disabled) {
  transform: translateY(0);
}

/* Focus styles */
button:focus {
  outline: none;
}

/* Disabled button styles */
button:disabled {
  transform: none;
  box-shadow: none;
}
</style>
