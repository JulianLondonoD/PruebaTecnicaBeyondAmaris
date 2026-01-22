/**
 * Focus Management Composable
 * Provides utilities for managing focus in accessible ways
 */

import { ref, onMounted, onUnmounted, type Ref } from 'vue'

/**
 * Trap focus within a container (useful for modals)
 */
export function useFocusTrap(containerRef: Ref<HTMLElement | null>) {
  const previousActiveElement = ref<HTMLElement | null>(null)

  const focusableSelector = [
    'a[href]',
    'button:not([disabled])',
    'textarea:not([disabled])',
    'input:not([disabled])',
    'select:not([disabled])',
    '[tabindex]:not([tabindex="-1"])'
  ].join(', ')

  function getFocusableElements(): HTMLElement[] {
    if (!containerRef.value) return []
    return Array.from(containerRef.value.querySelectorAll(focusableSelector)) as HTMLElement[]
  }

  function handleKeydown(event: KeyboardEvent) {
    if (event.key !== 'Tab') return

    const focusableElements = getFocusableElements()
    if (focusableElements.length === 0) return

    const firstElement = focusableElements[0]
    const lastElement = focusableElements[focusableElements.length - 1]

    if (event.shiftKey) {
      // Shift + Tab
      if (document.activeElement === firstElement) {
        event.preventDefault()
        lastElement?.focus()
      }
    } else {
      // Tab
      if (document.activeElement === lastElement) {
        event.preventDefault()
        firstElement?.focus()
      }
    }
  }

  function activate() {
    previousActiveElement.value = document.activeElement as HTMLElement

    const focusableElements = getFocusableElements()
    if (focusableElements.length > 0) {
      focusableElements[0]?.focus()
    }

    document.addEventListener('keydown', handleKeydown)
  }

  function deactivate() {
    document.removeEventListener('keydown', handleKeydown)
    previousActiveElement.value?.focus()
  }

  onUnmounted(() => {
    deactivate()
  })

  return {
    activate,
    deactivate
  }
}

/**
 * Manage focus for skip links
 */
export function useSkipLink(targetId: string) {
  function skipToContent() {
    const target = document.getElementById(targetId)
    if (target) {
      target.focus()
      target.scrollIntoView({ behavior: 'smooth' })
    }
  }

  return {
    skipToContent
  }
}

/**
 * Auto-focus on mount
 */
export function useAutoFocus(elementRef: Ref<HTMLElement | null>) {
  onMounted(() => {
    elementRef.value?.focus()
  })
}

/**
 * Announce to screen readers
 */
export function useScreenReaderAnnounce() {
  const announcer = ref<HTMLElement | null>(null)

  onMounted(() => {
    // Create live region for announcements
    const liveRegion = document.createElement('div')
    liveRegion.setAttribute('role', 'status')
    liveRegion.setAttribute('aria-live', 'polite')
    liveRegion.setAttribute('aria-atomic', 'true')
    liveRegion.className = 'sr-only'
    liveRegion.style.cssText = `
      position: absolute;
      left: -10000px;
      width: 1px;
      height: 1px;
      overflow: hidden;
    `
    document.body.appendChild(liveRegion)
    announcer.value = liveRegion
  })

  onUnmounted(() => {
    if (announcer.value) {
      document.body.removeChild(announcer.value)
    }
  })

  function announce(message: string, priority: 'polite' | 'assertive' = 'polite') {
    if (announcer.value) {
      announcer.value.setAttribute('aria-live', priority)
      announcer.value.textContent = message

      // Clear after announcement
      setTimeout(() => {
        if (announcer.value) {
          announcer.value.textContent = ''
        }
      }, 1000)
    }
  }

  return {
    announce
  }
}
