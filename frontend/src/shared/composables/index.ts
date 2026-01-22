/**
 * Shared Composables Barrel Export
 * Provides a centralized export point for all shared composables
 */

export { usePerformance, useComponentPerformance } from './usePerformance'
export {
  useFocusTrap,
  useSkipLink,
  useAutoFocus,
  useScreenReaderAnnounce
} from './useFocusManagement'
export { useVirtualList, useInfiniteScroll } from './useVirtualList'
