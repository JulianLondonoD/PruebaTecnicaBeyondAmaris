import { ref } from 'vue'
import type { Notification, NotificationType } from '@/types/ui'
import { NOTIFICATION_DURATION } from '@/utils/constants'

const notifications = ref<Notification[]>([])

// Helper function to get default duration based on notification type
function getDefaultDuration(type: NotificationType): number {
  const durationMap: Record<NotificationType, number> = {
    success: NOTIFICATION_DURATION.success,
    error: NOTIFICATION_DURATION.error,
    warning: NOTIFICATION_DURATION.warning,
    info: NOTIFICATION_DURATION.info
  }
  return durationMap[type] ?? 5000
}

export function useNotifications() {
  let idCounter = 0

  const addNotification = (options: {
    type: NotificationType
    title?: string
    message: string
    duration?: number
  }) => {
    const id = `notification-${Date.now()}-${++idCounter}`
    
    // Use configured duration based on type if not explicitly provided
    const defaultDuration = options.duration ?? getDefaultDuration(options.type)
    
    const notification: Notification = {
      id,
      type: options.type,
      title: options.title,
      message: options.message,
      duration: defaultDuration
    }

    notifications.value.push(notification)

    const duration = notification.duration ?? 5000
    if (duration > 0) {
      setTimeout(() => {
        removeNotification(id)
      }, duration)
    }

    return id
  }

  const removeNotification = (id: string) => {
    const index = notifications.value.findIndex(n => n.id === id)
    if (index > -1) {
      notifications.value.splice(index, 1)
    }
  }

  const success = (message: string, duration?: number) => {
    addNotification({ type: 'success', message, duration })
  }

  const error = (message: string, duration?: number) => {
    addNotification({ type: 'error', message, duration })
  }

  const warning = (message: string, duration?: number) => {
    addNotification({ type: 'warning', message, duration })
  }

  const info = (message: string, duration?: number) => {
    addNotification({ type: 'info', message, duration })
  }

  return {
    notifications,
    addNotification,
    removeNotification,
    success,
    error,
    warning,
    info
  }
}
