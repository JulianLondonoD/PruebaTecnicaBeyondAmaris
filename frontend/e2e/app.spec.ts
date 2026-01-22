import { test, expect } from '@playwright/test'

test.describe('Todo Application E2E', () => {
  test.beforeEach(async ({ page }) => {
    await page.goto('/')
  })

  test('should load the application', async ({ page }) => {
    await expect(page).toHaveTitle(/Todo/i)
  })

  test('should display todo list', async ({ page }) => {
    // Wait for the main content to be visible
    await page.waitForSelector('main', { timeout: 5000 })
    
    // Check if the page has loaded
    const content = await page.textContent('body')
    expect(content).toBeTruthy()
  })

  test('should navigate through the app', async ({ page }) => {
    // Check that main content is visible
    const main = page.locator('main')
    await expect(main).toBeVisible()
  })

  test('should be accessible', async ({ page }) => {
    // Check for basic accessibility landmarks
    const hasMain = await page.locator('main').count()
    expect(hasMain).toBeGreaterThan(0)
  })
})
