import { test, expect } from '@playwright/test';

test.describe('Kanban Board E2E Tests', () => {
  test('should display the kanban board', async ({ page }) => {
    await page.goto('/');
    await expect(page.locator('h1')).toContainText('Kanban Board');
  });
});
