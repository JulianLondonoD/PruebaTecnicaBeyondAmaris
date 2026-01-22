<template>
  <div v-if="isOpen" class="fixed inset-0 z-50 flex items-center justify-center">
    <!-- Backdrop -->
    <div class="bg-black bg-opacity-50 absolute inset-0" @click="closeModal"></div>

    <!-- Modal Content -->
    <div
      class="bg-white rounded-lg p-6 max-w-lg w-full mx-4 relative z-10 max-h-[90vh] overflow-y-auto"
    >
      <!-- Header -->
      <div class="flex justify-between items-center mb-6">
        <h3 class="text-xl font-semibold text-gray-900">Edit Todo</h3>
        <button @click="closeModal" class="text-gray-400 hover:text-gray-600 transition-colors">
          <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path
              stroke-linecap="round"
              stroke-linejoin="round"
              stroke-width="2"
              d="M6 18L18 6M6 6l12 12"
            ></path>
          </svg>
        </button>
      </div>

      <!-- Form -->
      <form @submit.prevent="handleSubmit" class="space-y-6">
        <!-- Todo ID (Read-only) -->
        <div>
          <label class="block text-sm font-medium text-gray-700 mb-2"> Todo ID </label>
          <div class="relative">
            <input
              :value="todo.id"
              type="number"
              readonly
              class="w-full px-3 py-2 border border-gray-300 rounded-md bg-gray-50 text-gray-600 cursor-not-allowed"
            />
            <div class="absolute right-3 top-1/2 transform -translate-y-1/2">
              <svg
                class="w-4 h-4 text-gray-400"
                fill="none"
                stroke="currentColor"
                viewBox="0 0 24 24"
              >
                <path
                  stroke-linecap="round"
                  stroke-linejoin="round"
                  stroke-width="2"
                  d="M12 15v2m-6 4h12a2 2 0 002-2v-6a2 2 0 00-2-2H6a2 2 0 00-2 2v6a2 2 0 002 2zm10-10V7a4 4 0 00-8 0v4h8z"
                ></path>
              </svg>
            </div>
          </div>
          <p class="text-xs text-gray-500 mt-1">Todo ID cannot be changed</p>
        </div>

        <!-- Title (Read-only) -->
        <div>
          <label class="block text-sm font-medium text-gray-700 mb-2"> Title </label>
          <input
            :value="todo.title"
            type="text"
            readonly
            class="w-full px-3 py-2 border border-gray-300 rounded-md bg-gray-50 text-gray-600 cursor-not-allowed"
          />
          <p class="text-xs text-gray-500 mt-1">Title cannot be changed</p>
        </div>

        <!-- Description (Editable) -->
        <div>
          <label for="description-input" class="block text-sm font-medium text-gray-700 mb-2">
            Description <span class="text-red-500">*</span>
          </label>
          <div class="relative">
            <textarea
              id="description-input"
              ref="descriptionInput"
              v-model="editedDescription"
              rows="5"
              maxlength="1000"
              required
              class="w-full px-3 py-2 border border-gray-300 rounded-md focus:ring-2 focus:ring-blue-500 focus:border-blue-500 resize-none"
              :class="{
                'border-red-300 focus:ring-red-500 focus:border-red-500': hasDescriptionError,
                'border-green-300 focus:ring-green-500 focus:border-green-500':
                  editedDescription.length > 10 && !hasDescriptionError
              }"
              placeholder="Enter todo description..."
            ></textarea>
            <div class="absolute bottom-2 right-2 text-xs text-gray-400 bg-white px-1">
              {{ editedDescription.length }}/1000
            </div>
          </div>
          <div class="flex justify-between items-center mt-1">
            <p v-if="descriptionError" class="text-sm text-red-600 flex items-center">
              {{ descriptionError }}
            </p>
            <p
              v-else-if="editedDescription.length > 10"
              class="text-sm text-green-600 flex items-center"
            >
              Valid description
            </p>
          </div>
        </div>

        <!-- Category (Read-only) -->
        <div>
          <label class="block text-sm font-medium text-gray-700 mb-2"> Category </label>
          <input
            :value="todo.category"
            type="text"
            readonly
            class="w-full px-3 py-2 border border-gray-300 rounded-md bg-gray-50 text-gray-600 cursor-not-allowed"
          />
          <p class="text-xs text-gray-500 mt-1">Category cannot be changed</p>
        </div>

        <!-- Current Progress (Read-only) -->
        <div>
          <label class="block text-sm font-medium text-gray-700 mb-2"> Current Progress </label>
          <div class="space-y-2">
            <div class="w-full bg-gray-200 rounded-full h-4 overflow-hidden">
              <div
                class="h-4 rounded-full transition-all duration-500 ease-out"
                :class="getProgressColorClass(todo.totalProgress)"
                :style="{ width: `${todo.totalProgress}%` }"
              ></div>
            </div>
            <div class="flex justify-between text-sm">
              <span class="text-gray-600 font-medium"
                >{{ Math.round(todo.totalProgress) }}% completed</span
              >
              <span
                :class="
                  todo.totalProgress >= 100 ? 'text-green-600 font-semibold' : 'text-gray-500'
                "
              >
                {{
                  todo.totalProgress >= 100
                    ? '🎉 Completed!'
                    : `${Math.round(100 - todo.totalProgress)}% remaining`
                }}
              </span>
            </div>
          </div>
        </div>

        <!-- Changes Summary -->
        <div v-if="hasChanges" class="p-4 bg-blue-50 border border-blue-200 rounded-lg">
          <h4 class="text-sm font-medium text-blue-800 mb-2">Changes to be saved:</h4>
          <div class="space-y-2">
            <div class="flex flex-col space-y-1">
              <div class="flex items-center justify-between text-sm">
                <span class="text-blue-700 font-medium">Description:</span>
                <span class="text-blue-800 bg-blue-100 px-2 py-1 rounded text-xs">Modified</span>
              </div>
              <div class="text-xs text-blue-600 bg-blue-100 p-2 rounded border-l-4 border-blue-400">
                <p class="font-medium">Preview:</p>
                <p class="truncate">
                  {{ editedDescription.substring(0, 100)
                  }}{{ editedDescription.length > 100 ? '...' : '' }}
                </p>
              </div>
            </div>
          </div>
        </div>

        <!-- Error Message -->
        <div v-if="errorMessage" class="p-4 bg-red-50 border border-red-200 rounded-lg">
          <div class="flex items-start">
            <div>
              <h4 class="text-sm font-medium text-red-800">Update Failed</h4>
              <p class="text-sm text-red-700 mt-1">{{ errorMessage }}</p>
            </div>
          </div>
        </div>

        <!-- Action Buttons -->
        <div class="flex justify-end space-x-3 pt-6 border-t border-gray-200">
          <button
            type="button"
            @click="closeModal"
            class="px-4 py-2 text-gray-700 bg-gray-100 hover:bg-gray-200 rounded-md transition-colors"
          >
            Cancel
          </button>
          <button
            type="submit"
            :disabled="!hasChanges || !isValidForm || isSubmitting"
            class="px-6 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700 disabled:opacity-50 disabled:cursor-not-allowed transition-colors flex items-center"
          >
            <svg
              v-if="isSubmitting"
              class="animate-spin -ml-1 mr-3 h-4 w-4 text-white"
              xmlns="http://www.w3.org/2000/svg"
              fill="none"
              viewBox="0 0 24 24"
            >
              <circle
                class="opacity-25"
                cx="12"
                cy="12"
                r="10"
                stroke="currentColor"
                stroke-width="4"
              ></circle>
              <path
                class="opacity-75"
                fill="currentColor"
                d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"
              ></path>
            </svg>
            <span>{{ isSubmitting ? 'Updating...' : 'Update Todo' }}</span>
          </button>
        </div>
      </form>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, nextTick, watch } from 'vue'

interface TodoItem {
  id: number
  title: string
  description: string
  category: string
  totalProgress: number
  isCompleted: boolean
}

interface Props {
  isOpen: boolean
  todo: TodoItem
}

interface Emits {
  (e: 'close'): void
  (e: 'success', updatedTodo: TodoItem): void
}

const props = defineProps<Props>()
const emit = defineEmits<Emits>()

const descriptionInput = ref<HTMLTextAreaElement>()
const editedDescription = ref('')
const isSubmitting = ref(false)
const errorMessage = ref('')

const hasChanges = computed(() => {
  return editedDescription.value.trim() !== props.todo.description.trim()
})

const descriptionError = computed(() => {
  const trimmed = editedDescription.value.trim()
  if (!trimmed) return 'Description is required'
  if (trimmed.length < 10) return 'Description must be at least 10 characters'
  if (trimmed.length > 1000) return 'Description cannot exceed 1000 characters'
  return ''
})

const hasDescriptionError = computed(() => !!descriptionError.value)

const isValidForm = computed(() => {
  return !hasDescriptionError.value && editedDescription.value.trim().length > 0
})

const getProgressColorClass = (progress: number): string => {
  if (progress === 0) return 'bg-gray-300'
  if (progress < 25) return 'bg-red-500'
  if (progress < 50) return 'bg-yellow-500'
  if (progress < 75) return 'bg-blue-500'
  if (progress < 100) return 'bg-green-500'
  return 'bg-emerald-600'
}

const resetForm = () => {
  editedDescription.value = props.todo.description
  errorMessage.value = ''
  isSubmitting.value = false
}

const closeModal = () => {
  resetForm()
  emit('close')
}

const handleSubmit = async () => {
  if (!isValidForm.value || !hasChanges.value || isSubmitting.value) return

  isSubmitting.value = true
  errorMessage.value = ''

  try {
    const updateData = {
      description: editedDescription.value.trim()
    }

    console.log('🚀 Updating todo:', {
      todoId: props.todo.id,
      data: updateData,
      url: `/api/v1/todolists/${props.todo.id}`
    })

    const response = await fetch(`/api/v1/todolists/${props.todo.id}`, {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json',
        Accept: 'application/json'
      },
      body: JSON.stringify(updateData)
    })

    console.log('📡 Response Status:', response.status, response.statusText)

    if (!response.ok) {
      let errorData: any = {}
      try {
        errorData = await response.json()
      } catch (parseError) {
        console.warn('Could not parse error response:', parseError)
      }

      const errorMsg =
        errorData?.message || errorData?.error || `HTTP ${response.status}: ${response.statusText}`
      throw new Error(errorMsg)
    }

    const result = await response.json()
    console.log('✅ Todo updated successfully:', result)

    const updatedTodo = {
      ...props.todo,
      description: editedDescription.value.trim()
    }

    emit('success', updatedTodo)

    const message = `✅ Successfully updated "${props.todo.title}"!`
    alert(message)

    closeModal()

    setTimeout(() => {
      window.location.reload()
    }, 1000)
  } catch (error: any) {
    console.error('❌ Error updating todo:', error)
    errorMessage.value = error.message || 'Failed to update todo. Please try again.'
  } finally {
    isSubmitting.value = false
  }
}

watch(
  () => props.isOpen,
  isOpen => {
    if (isOpen) {
      resetForm()
      nextTick(() => {
        descriptionInput.value?.focus()
        descriptionInput.value?.select()
      })
    }
  }
)

watch(
  () => props.todo.id,
  () => {
    if (props.isOpen) {
      resetForm()
    }
  }
)
</script>

<style scoped>
@keyframes spin {
  to {
    transform: rotate(360deg);
  }
}

.animate-spin {
  animation: spin 1s linear infinite;
}

textarea:focus {
  outline: none;
}

input:read-only {
  background-color: #f9fafb;
  cursor: not-allowed;
}

button:hover:not(:disabled) {
  transform: translateY(-1px);
}

button:active:not(:disabled) {
  transform: translateY(0);
}

button:disabled {
  transform: none;
}

textarea {
  padding-bottom: 2.5rem;
}
</style>
