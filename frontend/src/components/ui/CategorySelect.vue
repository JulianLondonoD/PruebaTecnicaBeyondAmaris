<template>
  <div class="w-full">
    <label v-if="label" :for="inputId" class="block text-sm font-medium text-gray-700 mb-1">
      {{ label }}
      <span v-if="required" class="text-red-500 ml-1">*</span>
    </label>

    <div class="relative">
      <select
        :id="inputId"
        v-model="selectedValue"
        :disabled="disabled || categoriesLoading"
        :required="required"
        class="w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:ring-2 focus:ring-blue-500 focus:border-transparent appearance-none bg-white"
        :class="{
          'cursor-not-allowed opacity-50': disabled || categoriesLoading,
          'border-red-300 focus:ring-red-500': error,
          'border-green-300 focus:ring-green-500': selectedValue && !error
        }"
        @change="handleChange"
      >
        <option value="" disabled>
          {{ categoriesLoading ? 'Loading categories...' : placeholder }}
        </option>
        <option v-for="option in categoryOptions" :key="option.value" :value="option.value">
          {{ option.label }}
        </option>
      </select>

      <!-- Loading spinner -->
      <div v-if="categoriesLoading" class="absolute right-8 top-1/2 transform -translate-y-1/2">
        <div class="animate-spin rounded-full h-4 w-4 border-b-2 border-blue-600"></div>
      </div>

      <!-- Dropdown arrow -->
      <div class="absolute right-3 top-1/2 transform -translate-y-1/2 pointer-events-none">
        <ChevronDownIcon class="h-4 w-4 text-gray-400" />
      </div>
    </div>

    <!-- Error message -->
    <p v-if="error" class="mt-1 text-sm text-red-600">
      {{ error }}
    </p>

    <!-- Categories loading error -->
    <div v-if="categoriesError" class="mt-1 text-sm text-red-600 flex items-center justify-between">
      <span>Failed to load categories.</span>
      <button
        type="button"
        @click="() => refetchCategories()"
        class="text-blue-600 hover:text-blue-700 hover:underline font-medium"
      >
        Retry
      </button>
    </div>

    <!-- Categories count (dev info) -->
    <p
      v-if="!categoriesLoading && !categoriesError && categories.length > 0"
      class="mt-1 text-xs text-gray-400"
    >
      {{ categories.length }} categories available
    </p>
  </div>
</template>

<script setup lang="ts">
import { computed, ref } from 'vue'
import { ChevronDownIcon } from '@heroicons/vue/24/outline'
import { useCategories } from '@/composables/useCategories'

interface Props {
  modelValue?: string
  label?: string
  placeholder?: string
  required?: boolean
  disabled?: boolean
  error?: string
}

interface Emits {
  (e: 'update:modelValue', value: string): void
  (e: 'change', value: string): void
}

const props = withDefaults(defineProps<Props>(), {
  label: '',
  placeholder: 'Select a category',
  required: false,
  disabled: false,
  error: ''
})

const emit = defineEmits<Emits>()

const { categories, categoryOptions, categoriesLoading, categoriesError, refetchCategories } =
  useCategories()

const inputId = ref(`category-select-${Math.random().toString(36).substring(7)}`)

const selectedValue = computed({
  get: () => props.modelValue || '',
  set: (value: string) => {
    emit('update:modelValue', value)
    emit('change', value)
  }
})

const handleChange = (event: Event) => {
  const target = event.target as HTMLSelectElement
  selectedValue.value = target.value
}
</script>

<style scoped>
/* Custom select styling */
select {
  background-image: none;
}

select:focus {
  outline: none;
}

/* Loading animation */
@keyframes spin {
  to {
    transform: rotate(360deg);
  }
}

.animate-spin {
  animation: spin 1s linear infinite;
}

/* Hover effects */
select:hover:not(:disabled) {
  border-color: #9ca3af;
}

/* Focus states */
select:focus {
  box-shadow: 0 0 0 3px rgba(59, 130, 246, 0.1);
}
</style>
