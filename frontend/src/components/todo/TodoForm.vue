<template>
  <div class="bg-white rounded-lg shadow-md p-6">
    <h2 class="text-2xl font-bold text-gray-800 mb-6">Create New Todo</h2>

    <form @submit.prevent="handleSubmit" class="space-y-4">
      <BaseInput
        v-model="form.title"
        label="Title"
        placeholder="Enter todo title"
        required
        :error="errors.title"
      />

      <div class="flex flex-col">
        <label class="mb-1 text-sm font-medium text-gray-700"> Description </label>
        <textarea
          v-model="form.description"
          placeholder="Enter todo description"
          rows="3"
          class="px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
          required
        ></textarea>
        <span v-if="errors.description" class="text-red-600 text-sm mt-1">
          {{ errors.description }}
        </span>
      </div>

      <CategorySelect
        v-model="form.category"
        label="Category"
        placeholder="Select a category"
        required
        :error="errors.category"
        @change="clearCategoryError"
      />

      <div class="flex gap-3 justify-end">
        <BaseButton type="button" variant="outline" @click="handleReset"> Reset </BaseButton>
        <BaseButton type="submit" :loading="isSubmitting"> Create Todo </BaseButton>
      </div>
    </form>
  </div>
</template>

<script setup lang="ts">
import { reactive, watch } from 'vue'
import type { CreateTodoRequest } from '@/types/todo'
import BaseInput from '../ui/BaseInput.vue'
import CategorySelect from '../ui/CategorySelect.vue'
import BaseButton from '../ui/BaseButton.vue'
import { useCategories } from '@/composables/useCategories'
import { validateTitle, validateDescription, validateCategory } from '@/utils/validators'

interface Props {
  isSubmitting?: boolean
}

withDefaults(defineProps<Props>(), {
  isSubmitting: false
})

const emit = defineEmits<{
  submit: [data: CreateTodoRequest]
}>()

const { categories } = useCategories()

const form = reactive<CreateTodoRequest>({
  title: '',
  description: '',
  category: ''
})

const errors = reactive({
  title: '',
  description: '',
  category: ''
})

const validateForm = (): boolean => {
  errors.title = validateTitle(form.title) || ''
  errors.description = validateDescription(form.description) || ''
  errors.category = validateCategory(form.category, categories.value) || ''

  return !errors.title && !errors.description && !errors.category
}

const clearCategoryError = () => {
  errors.category = ''
}

// Real-time validation
watch(
  () => form.title,
  newTitle => {
    if (errors.title) {
      errors.title = validateTitle(newTitle) || ''
    }
  }
)

watch(
  () => form.description,
  newDescription => {
    if (errors.description) {
      errors.description = validateDescription(newDescription) || ''
    }
  }
)

watch(
  () => form.category,
  newCategory => {
    if (errors.category && newCategory) {
      errors.category = validateCategory(newCategory, categories.value) || ''
    }
  }
)

const handleSubmit = () => {
  if (validateForm()) {
    emit('submit', {
      title: form.title,
      description: form.description,
      category: form.category
    })

    handleReset()
  }
}

const handleReset = () => {
  form.title = ''
  form.description = ''
  form.category = ''

  errors.title = ''
  errors.description = ''
  errors.category = ''
}
</script>
