import { describe, it, expect } from 'vitest'
import {
  calculateProgressColor,
  getProgressStatus,
  getProgressColorClass,
  calculateEstimatedCompletion
} from '@/utils/progressHelpers'

describe('progressHelpers', () => {
  describe('getProgressStatus', () => {
    it('should return correct progress status', () => {
      expect(getProgressStatus(0)).toBe('Not Started')
      expect(getProgressStatus(50)).toBe('In Progress')
      expect(getProgressStatus(100)).toBe('Completed')
    })
  })

  describe('getProgressColorClass', () => {
    it('should return correct color class for different progress levels', () => {
      const testCases = [
        { progress: 0, expectedClass: 'progress-not-started' },
        { progress: 10, expectedClass: 'progress-low' },
        { progress: 30, expectedClass: 'progress-medium' },
        { progress: 60, expectedClass: 'progress-high' },
        { progress: 85, expectedClass: 'progress-almost' },
        { progress: 100, expectedClass: 'progress-complete' }
      ]

      testCases.forEach(({ progress, expectedClass }) => {
        expect(getProgressColorClass(progress)).toBe(expectedClass)
      })
    })
  })

  describe('calculateProgressColor', () => {
    it('should calculate correct progress color', () => {
      const testCases = [
        { progress: 0, expectedColor: 'bg-gray-200' },
        { progress: 10, expectedColor: 'bg-red-500' },
        { progress: 30, expectedColor: 'bg-yellow-500' },
        { progress: 60, expectedColor: 'bg-blue-500' },
        { progress: 85, expectedColor: 'bg-green-500' },
        { progress: 100, expectedColor: 'bg-emerald-600' }
      ]

      testCases.forEach(({ progress, expectedColor }) => {
        expect(calculateProgressColor(progress)).toBe(expectedColor)
      })
    })
  })

  describe('calculateEstimatedCompletion', () => {
    it('should calculate estimated completion', () => {
      expect(calculateEstimatedCompletion(0, [])).toBe('Not started')
      expect(calculateEstimatedCompletion(100, [])).toBe('Completed')
      expect(calculateEstimatedCompletion(50, [])).toContain('About')
    })
  })

  describe('edge cases', () => {
    it('should handle zero progress', () => {
      expect(getProgressStatus(0)).toBe('Not Started')
      expect(getProgressColorClass(0)).toBe('progress-not-started')
    })

    it('should handle completed progress', () => {
      expect(getProgressStatus(100)).toBe('Completed')
      expect(getProgressColorClass(100)).toBe('progress-complete')
    })
  })
})
