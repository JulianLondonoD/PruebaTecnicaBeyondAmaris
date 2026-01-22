/**
 * Configuration Utility Functions
 * 
 * Helper functions for safely parsing and validating configuration values
 */

/**
 * Safely parse an integer from environment variable with fallback
 * @param value - The string value to parse
 * @param fallback - The fallback value if parsing fails
 * @returns The parsed integer or fallback value
 */
export function safeParseInt(value: string | undefined, fallback: number): number {
  if (!value) return fallback
  const parsed = parseInt(value, 10)
  return isNaN(parsed) ? fallback : parsed
}

/**
 * Safely parse a boolean from environment variable with fallback
 * @param value - The string value to parse
 * @param fallback - The fallback value if parsing fails
 * @returns The parsed boolean or fallback value
 */
export function safeParseBool(value: string | undefined, fallback: boolean): boolean {
  if (!value) return fallback
  return value === 'true'
}

/**
 * Safely parse JSON from environment variable with fallback
 * @param value - The JSON string to parse
 * @param fallback - The fallback value if parsing fails
 * @returns The parsed object or fallback value
 */
export function safeParseJSON<T>(value: string | undefined, fallback: T): T {
  if (!value) return fallback
  try {
    return JSON.parse(value) as T
  } catch (error) {
    console.warn(`Failed to parse JSON: ${value}, using fallback`)
    return fallback
  }
}
