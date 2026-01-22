<template>
  <router-view />

  <Teleport to="body">
    <div class="fixed top-4 right-4 z-50 space-y-2">
      <TransitionGroup name="notification">
        <div
          v-for="notification in notifications"
          :key="notification.id"
          :class="[
            'px-6 py-4 rounded-lg shadow-lg text-white max-w-sm',
            notificationClass(notification.type)
          ]"
        >
          <div class="flex items-start justify-between">
            <p class="flex-1">{{ notification.message }}</p>
            <button
              @click="removeNotification(notification.id)"
              class="ml-4 text-white hover:text-gray-200"
            >
              ✕
            </button>
          </div>
        </div>
      </TransitionGroup>
    </div>
  </Teleport>
</template>

<script setup lang="ts">
import { useNotifications } from '@/composables/useNotifications'
import type { NotificationType } from '@/types/ui'

const { notifications, removeNotification } = useNotifications()

const notificationClass = (type: NotificationType) => {
  const classes = {
    success: 'bg-green-600',
    error: 'bg-red-600',
    warning: 'bg-yellow-600',
    info: 'bg-blue-600'
  }
  return classes[type]
}
</script>

<style scoped>
.notification-enter-active,
.notification-leave-active {
  transition: all 0.3s ease;
}

.notification-enter-from {
  opacity: 0;
  transform: translateX(100%);
}

.notification-leave-to {
  opacity: 0;
  transform: translateY(-20px);
}

.notification-move {
  transition: transform 0.3s ease;
}
</style>
