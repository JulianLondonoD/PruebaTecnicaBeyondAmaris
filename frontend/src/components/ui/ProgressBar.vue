<template>
  <div class="w-full">
    <!-- Progress info -->
    <div class="flex justify-between items-center mb-2">
      <span class="text-sm font-medium text-gray-700"> Progress </span>
      <span
        class="text-sm text-gray-600"
        :class="{ 'text-green-600 font-semibold': progress >= 100 }"
      >
        {{ Math.round(progress) }}% / 100%
      </span>
    </div>

    <!-- Progress bar container -->
    <div class="w-full bg-gray-200 rounded-full h-3 overflow-hidden relative">
      <!-- Background progress bar -->
      <div
        class="h-full rounded-full transition-all duration-700 ease-out relative overflow-hidden"
        :class="progressColorClass"
        :style="{ width: `${Math.min(100, progress)}%` }"
      >
        <!-- Animated shine effect when progress changes -->
        <div
          v-if="isAnimating"
          class="absolute top-0 left-0 h-full w-full bg-gradient-to-r from-transparent via-white to-transparent opacity-30 animate-shine"
        ></div>

        <!-- Pulse effect for completed todos -->
        <div
          v-if="progress >= 100"
          class="absolute inset-0 bg-green-400 opacity-20 animate-pulse"
        ></div>
      </div>

      <!-- Progress text overlay -->
      <div
        v-if="showPercentage && progress > 15"
        class="absolute inset-0 flex items-center justify-center text-xs font-bold text-white mix-blend-difference"
      >
        {{ Math.round(progress) }}%
      </div>

      <!-- Completion checkmark -->
      <div v-if="progress >= 100" class="absolute right-1 top-1/2 transform -translate-y-1/2">
        <CheckCircleIcon class="w-4 h-4 text-white" />
      </div>
    </div>

    <!-- Status text -->
    <div class="flex justify-between items-center mt-1">
      <span
        class="text-xs"
        :class="progress >= 100 ? 'text-green-600 font-medium' : 'text-gray-500'"
      >
        {{ getProgressStatus(progress) }}
      </span>
      <span v-if="estimatedCompletion && progress < 100" class="text-xs text-gray-400">
        ETA: {{ estimatedCompletion }}
      </span>
      <span v-else-if="progress >= 100" class="text-xs text-green-600 font-medium">
        🎉 Completed!
      </span>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, ref, watch } from 'vue'
import { CheckCircleIcon } from '@heroicons/vue/24/solid'
import { getProgressStatus, getProgressColorClass } from '@/utils/progressHelpers'

interface Props {
  progress: number
  showPercentage?: boolean
  estimatedCompletion?: string
  animated?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  showPercentage: false,
  animated: true
})

const isAnimating = ref(false)
const previousProgress = ref(props.progress)

const progressColorClass = computed(() => {
  const baseClass = getProgressColorClass(props.progress)
  const animationClass = isAnimating.value ? 'animate-pulse' : ''
  return `${baseClass} ${animationClass}`
})

// ✅ Trigger animation when progress changes
watch(
  () => props.progress,
  (newProgress, oldProgress) => {
    if (props.animated && newProgress !== oldProgress && oldProgress !== undefined) {
      previousProgress.value = oldProgress
      isAnimating.value = true

      // Stop animation after duration
      setTimeout(() => {
        isAnimating.value = false
      }, 700)
    }
  },
  { immediate: false }
)
</script>

<style scoped>
/* Shine animation for progress updates */
@keyframes shine {
  0% {
    transform: translateX(-100%);
  }
  100% {
    transform: translateX(100%);
  }
}

.animate-shine {
  animation: shine 1.5s ease-out;
}

/* Progress color classes */
.progress-not-started {
  @apply bg-gray-300;
}
.progress-low {
  @apply bg-red-500;
}
.progress-medium {
  @apply bg-yellow-500;
}
.progress-high {
  @apply bg-blue-500;
}
.progress-almost {
  @apply bg-green-500;
}
.progress-complete {
  @apply bg-emerald-600;
}

/* Smooth transitions */
.transition-all {
  transition-property: all;
  transition-timing-function: cubic-bezier(0.4, 0, 0.2, 1);
}

/* Mix blend mode for text overlay */
.mix-blend-difference {
  mix-blend-mode: difference;
}
</style>
