/**
 * Virtual Scrolling Composable
 * Optimizes rendering of large lists by only rendering visible items
 */

import { ref, computed, onMounted, onUnmounted, type Ref } from 'vue'

interface VirtualListOptions {
  itemHeight: number
  overscan?: number
  containerHeight?: number
}

/**
 * Virtual list implementation for performance optimization
 */
export function useVirtualList<T>(items: Ref<T[]>, options: VirtualListOptions) {
  const { itemHeight, overscan = 3, containerHeight = 600 } = options

  const scrollTop = ref(0)
  const containerRef = ref<HTMLElement | null>(null)

  // Calculate visible range
  const visibleRange = computed(() => {
    const start = Math.max(0, Math.floor(scrollTop.value / itemHeight) - overscan)
    const end = Math.min(
      items.value.length,
      Math.ceil((scrollTop.value + containerHeight) / itemHeight) + overscan
    )

    return { start, end }
  })

  // Get visible items
  const visibleItems = computed(() => {
    const { start, end } = visibleRange.value
    return items.value.slice(start, end).map((item, index) => ({
      item,
      index: start + index,
      top: (start + index) * itemHeight
    }))
  })

  // Total height of all items
  const totalHeight = computed(() => items.value.length * itemHeight)

  // Handle scroll
  function handleScroll(event: Event) {
    const target = event.target as HTMLElement
    scrollTop.value = target.scrollTop
  }

  onMounted(() => {
    if (containerRef.value) {
      containerRef.value.addEventListener('scroll', handleScroll)
    }
  })

  onUnmounted(() => {
    if (containerRef.value) {
      containerRef.value.removeEventListener('scroll', handleScroll)
    }
  })

  // Scroll to specific item
  function scrollToItem(index: number) {
    if (containerRef.value) {
      containerRef.value.scrollTop = index * itemHeight
    }
  }

  return {
    containerRef,
    visibleItems,
    totalHeight,
    scrollToItem
  }
}

/**
 * Simplified infinite scroll
 */
export function useInfiniteScroll(
  callback: () => void | Promise<void>,
  options: { threshold?: number; disabled?: Ref<boolean> } = {}
) {
  const { threshold = 100, disabled = ref(false) } = options
  const containerRef = ref<HTMLElement | null>(null)
  const isLoading = ref(false)

  async function handleScroll(event: Event) {
    if (disabled.value || isLoading.value) return

    const target = event.target as HTMLElement
    const scrolledToBottom =
      target.scrollHeight - target.scrollTop - target.clientHeight < threshold

    if (scrolledToBottom) {
      isLoading.value = true
      try {
        await callback()
      } finally {
        isLoading.value = false
      }
    }
  }

  onMounted(() => {
    if (containerRef.value) {
      containerRef.value.addEventListener('scroll', handleScroll)
    }
  })

  onUnmounted(() => {
    if (containerRef.value) {
      containerRef.value.removeEventListener('scroll', handleScroll)
    }
  })

  return {
    containerRef,
    isLoading
  }
}
