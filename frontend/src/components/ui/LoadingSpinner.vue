<template>
  <div class="flex items-center justify-center">
    <div :class="['animate-spin rounded-full border-t-2 border-b-2', sizeClass, colorClass]"></div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'

interface Props {
  size?: 'sm' | 'md' | 'lg' | 'xl'
  color?: 'blue' | 'white' | 'gray'
}

const props = withDefaults(defineProps<Props>(), {
  size: 'md',
  color: 'blue'
})

const sizeClass = computed(() => {
  const sizes = {
    sm: 'h-4 w-4',
    md: 'h-8 w-8',
    lg: 'h-12 w-12',
    xl: 'h-16 w-16'
  }
  return sizes[props.size]
})

const colorClass = computed(() => {
  const colors = {
    blue: 'border-blue-500',
    white: 'border-white',
    gray: 'border-gray-500'
  }
  return colors[props.color]
})
</script>
