/**
 * Centralized Application Configuration
 * 
 * This file provides a centralized configuration system that reads from environment variables
 * and provides sensible defaults for all application settings.
 */

import { safeParseInt, safeParseBool } from './utils'

export const appConfig = {
  /**
   * API Configuration
   */
  api: {
    baseUrl: import.meta.env.VITE_API_URL || 'https://localhost:7025',
    port: safeParseInt(import.meta.env.VITE_API_PORT, 7025),
    timeout: safeParseInt(import.meta.env.VITE_API_TIMEOUT, 10000),
    retries: safeParseInt(import.meta.env.VITE_API_RETRIES, 2)
  },

  /**
   * Application Server Configuration
   */
  app: {
    port: safeParseInt(import.meta.env.VITE_APP_PORT, 3000),
    host: import.meta.env.VITE_APP_HOST || 'localhost',
    autoOpenBrowser: safeParseBool(import.meta.env.VITE_AUTO_OPEN_BROWSER, false)
  },

  /**
   * Cache Configuration
   */
  cache: {
    staleTime: safeParseInt(import.meta.env.VITE_CACHE_STALE_TIME, 5000),
    gcTime: safeParseInt(import.meta.env.VITE_CACHE_GC_TIME, 300000),
    categoriesStaleTime: safeParseInt(import.meta.env.VITE_CACHE_CATEGORIES_STALE_TIME, 600000), // 10 minutes
    categoriesGcTime: safeParseInt(import.meta.env.VITE_CACHE_CATEGORIES_GC_TIME, 900000) // 15 minutes
  },

  /**
   * Notification Duration Configuration
   */
  notifications: {
    duration: {
      success: safeParseInt(import.meta.env.VITE_NOTIFICATION_SUCCESS_DURATION, 3000),
      error: safeParseInt(import.meta.env.VITE_NOTIFICATION_ERROR_DURATION, 5000),
      warning: safeParseInt(import.meta.env.VITE_NOTIFICATION_WARNING_DURATION, 4000),
      info: safeParseInt(import.meta.env.VITE_NOTIFICATION_INFO_DURATION, 3000)
    }
  },

  /**
   * Build Configuration
   */
  build: {
    outputDir: import.meta.env.VITE_OUTPUT_DIR || 'dist',
    sourcemap: import.meta.env.VITE_SOURCEMAP_ENABLED !== 'false', // Default to true
    bundleAnalyzer: safeParseBool(import.meta.env.VITE_BUNDLE_ANALYZER, false),
    statsFilename: import.meta.env.VITE_STATS_FILENAME || './dist/stats.html'
  }
} as const
