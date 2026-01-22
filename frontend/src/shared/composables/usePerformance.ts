/**
 * Performance Monitoring Composable
 * Provides utilities for monitoring and optimizing performance
 */

import { onMounted, onUnmounted } from 'vue'

interface PerformanceMetrics {
  name: string
  duration: number
  timestamp: number
}

const metrics: PerformanceMetrics[] = []

/**
 * Composable for performance monitoring
 */
export function usePerformance() {
  /**
   * Measure the performance of a function
   */
  function measurePerformance<T>(name: string, fn: () => T): T {
    const start = performance.now()
    const result = fn()
    const duration = performance.now() - start

    metrics.push({
      name,
      duration,
      timestamp: Date.now()
    })

    if (duration > 100) {
      console.warn(`⚠️ Performance Warning: ${name} took ${duration.toFixed(2)}ms`)
    }

    return result
  }

  /**
   * Measure async function performance
   */
  async function measureAsync<T>(name: string, fn: () => Promise<T>): Promise<T> {
    const start = performance.now()
    const result = await fn()
    const duration = performance.now() - start

    metrics.push({
      name,
      duration,
      timestamp: Date.now()
    })

    if (duration > 500) {
      console.warn(`⚠️ Performance Warning: ${name} took ${duration.toFixed(2)}ms`)
    }

    return result
  }

  /**
   * Get all performance metrics
   */
  function getMetrics() {
    return [...metrics]
  }

  /**
   * Clear all metrics
   */
  function clearMetrics() {
    metrics.length = 0
  }

  /**
   * Get Core Web Vitals
   */
  function getCoreWebVitals() {
    const navigation = performance.getEntriesByType('navigation')[0] as PerformanceNavigationTiming

    if (!navigation) return null

    return {
      // Largest Contentful Paint
      lcp: navigation.loadEventEnd - navigation.fetchStart,
      // First Input Delay (approximation)
      fid: navigation.domInteractive - navigation.responseEnd,
      // Cumulative Layout Shift (would need PerformanceObserver for real value)
      cls: 0 // Placeholder
    }
  }

  return {
    measurePerformance,
    measureAsync,
    getMetrics,
    clearMetrics,
    getCoreWebVitals
  }
}

/**
 * Component performance tracking
 */
export function useComponentPerformance(componentName: string) {
  const mountStart = performance.now()

  onMounted(() => {
    const mountTime = performance.now() - mountStart

    if (mountTime > 50) {
      console.warn(`⚠️ Slow component mount: ${componentName} (${mountTime.toFixed(2)}ms)`)
    }

    metrics.push({
      name: `${componentName} mount`,
      duration: mountTime,
      timestamp: Date.now()
    })
  })

  onUnmounted(() => {
    console.log(`✅ ${componentName} unmounted`)
  })
}
