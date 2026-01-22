<template>
  <header class="bg-blue-600 text-white shadow-lg">
    <div class="container mx-auto px-4 py-6">
      <div class="flex justify-between items-center">
        <h1 class="text-3xl font-bold">📝 TodoApp</h1>

        <div v-if="stats" class="flex gap-6 text-sm">
          <div class="text-center">
            <div class="font-semibold text-lg">{{ stats.total }}</div>
            <div class="text-blue-200">Total</div>
          </div>
          <div class="text-center">
            <div class="font-semibold text-lg text-green-200">{{ stats.completed }}</div>
            <div class="text-blue-200">Completed</div>
          </div>
          <div class="text-center">
            <div class="font-semibold text-lg text-yellow-200">{{ stats.inProgress }}</div>
            <div class="text-blue-200">In Progress</div>
          </div>
          <div class="text-center">
            <div class="font-semibold text-lg text-gray-200">{{ stats.notStarted }}</div>
            <div class="text-blue-200">Not Started</div>
          </div>
        </div>
      </div>
    </div>
  </header>
</template>

<script setup lang="ts">
interface Stats {
  total: number
  completed: number
  inProgress: number
  notStarted: number
}

interface Props {
  stats?: Stats | null
}

defineProps<Props>()
</script>
