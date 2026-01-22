<template>
  <div v-if="isOpen" class="fixed inset-0 z-50 flex items-center justify-center">
    <!-- Backdrop -->
    <div class="bg-black bg-opacity-50 absolute inset-0" @click="closeModal"></div>

    <!-- Modal Content -->
    <div class="bg-white rounded-lg p-6 max-w-md w-full mx-4 relative z-10">
      <!-- Header -->
      <div class="flex justify-between items-center mb-6">
        <h3 class="text-lg font-semibold text-gray-900">Add Progress to "{{ todo.title }}"</h3>
        <button @click="closeModal" class="text-gray-400 hover:text-gray-600 transition-colors">
          <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path
              stroke-linecap="round"
              stroke-linejoin="round"
              stroke-width="2"
              d="M6 18L18 6M6 6l12 12"
            ></path>
          </svg>
        </button>
      </div>

      <!-- Current Progress Section -->
      <div class="mb-6">
        <h4 class="text-sm font-medium text-gray-700 mb-3">Current Progress</h4>
        <div class="w-full bg-gray-200 rounded-full h-3 overflow-hidden">
          <div
            class="h-3 rounded-full transition-all duration-500 ease-out"
            :class="getProgressColorClass(todo.totalProgress)"
            :style="{ width: `${todo.totalProgress}%` }"
          ></div>
        </div>
        <div class="flex justify-between text-sm text-gray-600 mt-2">
          <span class="font-medium">{{ todo.totalProgress }}% completed</span>
          <span>{{ 100 - todo.totalProgress }}% remaining</span>
        </div>
      </div>

      <!-- Progress Input Form -->
      <form @submit.prevent="handleSubmit" class="space-y-4">
        <!-- Input Field -->
        <div>
          <label for="progress-input" class="block text-sm font-medium text-gray-700 mb-2">
            Add Progress (%)
          </label>
          <div class="relative">
            <input
              id="progress-input"
              ref="progressInput"
              v-model.number="progressAmount"
              type="number"
              min="1"
              :max="maxAllowed"
              step="1"
              required
              class="w-full px-3 py-2 pr-10 border border-gray-300 rounded-md shadow-sm focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors"
              :class="{
                'border-red-300 focus:ring-red-500 focus:border-red-500': hasInputError,
                'border-green-300 focus:ring-green-500 focus:border-green-500':
                  progressAmount > 0 && !hasInputError
              }"
              placeholder="Enter progress percentage"
            />
            <div
              class="absolute right-3 top-1/2 transform -translate-y-1/2 text-gray-400 pointer-events-none"
            >
              %
            </div>
          </div>
          <div class="flex justify-between items-center mt-1">
            <p class="text-xs text-gray-500">Maximum: {{ maxAllowed }}%</p>
            <p class="text-xs text-gray-400">Current: {{ todo.totalProgress }}%</p>
          </div>
        </div>

        <!-- Quick Add Buttons -->
        <div class="space-y-2">
          <p class="text-sm font-medium text-gray-700">Quick Add:</p>
          <div class="grid grid-cols-4 gap-2">
            <button
              v-for="amount in availableQuickAmounts"
              :key="amount"
              type="button"
              @click="setProgressAmount(amount)"
              class="px-3 py-2 text-sm border border-gray-300 rounded-md hover:bg-gray-50 hover:border-gray-400 transition-colors focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
            >
              +{{ amount }}%
            </button>
          </div>
        </div>

        <!-- Progress Preview -->
        <Transition
          enter-active-class="transition duration-300 ease-out"
          enter-from-class="transform scale-95 opacity-0"
          enter-to-class="transform scale-100 opacity-100"
          leave-active-class="transition duration-200 ease-in"
          leave-from-class="transform scale-100 opacity-100"
          leave-to-class="transform scale-95 opacity-0"
        >
          <div v-if="showPreview" class="p-4 bg-blue-50 border border-blue-200 rounded-lg">
            <h4 class="text-sm font-medium text-blue-800 mb-3">Preview</h4>
            <div class="w-full bg-gray-200 rounded-full h-3 overflow-hidden">
              <div
                class="h-3 rounded-full transition-all duration-500 ease-out"
                :class="getProgressColorClass(newTotalProgress)"
                :style="{ width: `${newTotalProgress}%` }"
              >
                <!-- Shine effect for preview -->
                <div
                  class="h-full w-full bg-gradient-to-r from-transparent via-white to-transparent opacity-30 animate-shine"
                ></div>
              </div>
            </div>
            <div class="flex justify-between items-center mt-2">
              <p class="text-sm text-blue-700 font-medium">New total: {{ newTotalProgress }}%</p>
              <p v-if="willComplete" class="text-sm text-green-700 font-bold">
                🎉 Task will be completed!
              </p>
            </div>
          </div>
        </Transition>

        <!-- Error Message -->
        <div v-if="errorMessage" class="p-3 bg-red-50 border border-red-200 rounded-md">
          <div class="flex items-center">
            <svg
              class="w-4 h-4 text-red-400 mr-2"
              fill="none"
              stroke="currentColor"
              viewBox="0 0 24 24"
            >
              <path
                stroke-linecap="round"
                stroke-linejoin="round"
                stroke-width="2"
                d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"
              ></path>
            </svg>
            <p class="text-sm text-red-700">{{ errorMessage }}</p>
          </div>
        </div>

        <!-- Debug Info (Development only) -->
        <div v-if="showDebugInfo" class="p-3 bg-gray-100 border border-gray-200 rounded-md text-xs">
          <p class="font-medium text-gray-700 mb-1">Debug Info:</p>
          <div class="grid grid-cols-2 gap-2 text-gray-600">
            <span>Todo ID: {{ todo.id }}</span>
            <span>Progress Amount: {{ progressAmount }}</span>
            <span>Is Valid: {{ isValidInput }}</span>
            <span>Is Submitting: {{ isSubmitting }}</span>
            <span>Max Allowed: {{ maxAllowed }}%</span>
            <span>New Total: {{ newTotalProgress }}%</span>
          </div>
        </div>

        <!-- Action Buttons -->
        <div class="flex justify-end space-x-3 pt-4 border-t border-gray-200">
          <button
            type="button"
            @click="closeModal"
            class="px-4 py-2 text-gray-700 bg-gray-100 hover:bg-gray-200 rounded-md transition-colors focus:ring-2 focus:ring-gray-500"
          >
            Cancel
          </button>
          <button
            type="submit"
            :disabled="!isValidInput || isSubmitting"
            class="px-4 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700 disabled:opacity-50 disabled:cursor-not-allowed transition-colors focus:ring-2 focus:ring-blue-500 flex items-center"
          >
            <!-- Loading Spinner -->
            <svg
              v-if="isSubmitting"
              class="animate-spin -ml-1 mr-2 h-4 w-4 text-white"
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

            <span>{{ isSubmitting ? 'Adding Progress...' : 'Add Progress' }}</span>
          </button>
        </div>
      </form>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, nextTick, watch } from 'vue'

// Types
interface TodoItem {
  id: number
  title: string
  totalProgress: number
  description?: string
  category?: string
}

interface Props {
  isOpen: boolean
  todo: TodoItem
}

interface Emits {
  (e: 'close'): void
  (e: 'success', data?: any): void
}

// Props and Emits
const props = defineProps<Props>()
const emit = defineEmits<Emits>()

// Reactive Data
const progressInput = ref<HTMLInputElement>()
const progressAmount = ref<number>(0)
const isSubmitting = ref(false)
const errorMessage = ref('')
const showDebugInfo = ref(import.meta.env.DEV)

// Computed Properties
const maxAllowed = computed(() => Math.max(0, 100 - props.todo.totalProgress))

const availableQuickAmounts = computed(() => {
  const amounts = [5, 10, 25, 50]
  return amounts.filter(amount => amount <= maxAllowed.value)
})

const isValidInput = computed(() => {
  return (
    progressAmount.value > 0 &&
    progressAmount.value <= maxAllowed.value &&
    Number.isInteger(progressAmount.value)
  )
})

const hasInputError = computed(() => {
  return (
    progressAmount.value > 0 &&
    (progressAmount.value > maxAllowed.value || !Number.isInteger(progressAmount.value))
  )
})

const newTotalProgress = computed(() => {
  return Math.min(100, props.todo.totalProgress + (progressAmount.value || 0))
})

const willComplete = computed(() => {
  return newTotalProgress.value >= 100
})

const showPreview = computed(() => {
  return progressAmount.value > 0 && progressAmount.value <= maxAllowed.value
})

// Methods
const getProgressColorClass = (progress: number): string => {
  if (progress === 0) return 'bg-gray-300'
  if (progress < 25) return 'bg-red-500'
  if (progress < 50) return 'bg-yellow-500'
  if (progress < 75) return 'bg-blue-500'
  if (progress < 100) return 'bg-green-500'
  return 'bg-emerald-600'
}

const setProgressAmount = (amount: number) => {
  progressAmount.value = Math.min(amount, maxAllowed.value)
  nextTick(() => {
    progressInput.value?.focus()
  })
}

const resetForm = () => {
  progressAmount.value = 0
  errorMessage.value = ''
  isSubmitting.value = false
}

const closeModal = () => {
  resetForm()
  emit('close')
}

const handleSubmit = async () => {
  if (!isValidInput.value || isSubmitting.value) return

  // Validate todo ID
  if (!props.todo.id || typeof props.todo.id !== 'number' || props.todo.id <= 0) {
    errorMessage.value = 'Invalid todo ID'
    return
  }

  isSubmitting.value = true
  errorMessage.value = ''

  try {
    const requestData = {
      dateTime: new Date().toISOString(),
      percent: progressAmount.value
    }

    // Development logging only
    if (import.meta.env.DEV) {
      console.log('🚀 Adding progress:', {
        todoId: props.todo.id,
        percent: progressAmount.value,
        url: `/api/v1/todolists/${props.todo.id}/progressions`
      })
    }

    // Make API call using fetch
    const response = await fetch(`/api/v1/todolists/${props.todo.id}/progressions`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        Accept: 'application/json'
      },
      body: JSON.stringify(requestData)
    })

    if (import.meta.env.DEV) {
      console.log('📡 Response Status:', response.status, response.statusText)
    }

    if (!response.ok) {
      let errorData: any = {}
      try {
        errorData = await response.json()
      } catch (parseError) {
        if (import.meta.env.DEV) {
          console.warn('Could not parse error response:', parseError)
        }
      }

      const errorMsg =
        errorData?.message || errorData?.error || `HTTP ${response.status}: ${response.statusText}`
      throw new Error(errorMsg)
    }

    // Parse successful response
    const result = await response.json()
    if (import.meta.env.DEV) {
      console.log('✅ Progress added successfully:', result)
    }

    // Success - emit events and close modal
    emit('success', result)
    closeModal()

    // Refresh page to see changes (temporary solution)
    // TODO: Replace with proper reactive state management
    setTimeout(() => {
      window.location.reload()
    }, 500)
  } catch (error: any) {
    if (import.meta.env.DEV) {
      console.error('❌ Error adding progress:', error)
    }
    errorMessage.value = error.message || 'Failed to add progress. Please try again.'
  } finally {
    isSubmitting.value = false
  }
}

// Watchers
watch(
  () => props.isOpen,
  isOpen => {
    if (isOpen) {
      resetForm()
      nextTick(() => {
        progressInput.value?.focus()
      })
    }
  }
)

// Reset form when todo changes
watch(
  () => props.todo.id,
  () => {
    resetForm()
  }
)
</script>

<style scoped>
/* Animations */
@keyframes spin {
  to {
    transform: rotate(360deg);
  }
}

@keyframes shine {
  0% {
    transform: translateX(-100%);
  }
  100% {
    transform: translateX(100%);
  }
}

.animate-spin {
  animation: spin 1s linear infinite;
}

.animate-shine {
  animation: shine 1.5s ease-out infinite;
}

/* Input styling */
input[type='number']::-webkit-outer-spin-button,
input[type='number']::-webkit-inner-spin-button {
  -webkit-appearance: none;
  margin: 0;
}

input[type='number'] {
  -moz-appearance: textfield;
}

/* Smooth transitions */
.transition-all {
  transition-property: all;
  transition-timing-function: cubic-bezier(0.4, 0, 0.2, 1);
}

/* Focus styles */
button:focus {
  outline: none;
}

/* Hover effects */
button:hover:not(:disabled) {
  transform: translateY(-1px);
}

button:disabled {
  transform: none;
}
</style>
