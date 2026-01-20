import { test, expect } from '@playwright/test';

test.describe('Kanban Board E2E Tests', () => {
  test.beforeEach(async ({ page }) => {
    // Clear localStorage before each test
    await page.goto('/');
    await page.evaluate(() => localStorage.clear());
    await page.reload();
  });

  test('should display empty board with three columns', async ({ page }) => {
    await page.goto('/');

    // Check header
    await expect(page.locator('h1')).toContainText('Kanban Board');

    // Check three columns exist
    const columns = page.locator('app-task-column');
    await expect(columns).toHaveCount(3);

    // Check column titles
    await expect(page.getByText('To Do')).toBeVisible();
    await expect(page.getByText('In Progress')).toBeVisible();
    await expect(page.getByText('Done')).toBeVisible();
  });

  test('should create a new task', async ({ page }) => {
    await page.goto('/');

    // Click add task button
    await page.click('.btn-add');

    // Fill form
    await page.fill('#title', 'My New Task');
    await page.fill('#description', 'Task description');
    await page.selectOption('#priority', 'high');

    // Submit
    await page.click('button[type="submit"]');

    // Verify task appears in To Do column
    await expect(page.locator('.task-card')).toHaveCount(1);
    await expect(page.getByText('My New Task')).toBeVisible();
  });

  test('should edit an existing task', async ({ page }) => {
    await page.goto('/');

    // Create a task first
    await page.click('.btn-add');
    await page.fill('#title', 'Original Title');
    await page.click('button[type="submit"]');

    // Click edit button
    await page.click('.task-card .btn-icon[title="Edit"]');

    // Update title
    await page.fill('#title', 'Updated Title');
    await page.click('button[type="submit"]');

    // Verify updated title
    await expect(page.getByText('Updated Title')).toBeVisible();
    await expect(page.getByText('Original Title')).not.toBeVisible();
  });

  test('should delete a task', async ({ page }) => {
    await page.goto('/');

    // Create a task
    await page.click('.btn-add');
    await page.fill('#title', 'Task to Delete');
    await page.click('button[type="submit"]');

    // Accept the confirmation dialog
    page.on('dialog', dialog => dialog.accept());

    // Click delete button
    await page.click('.task-card .btn-icon[title="Delete"]');

    // Verify task is removed
    await expect(page.locator('.task-card')).toHaveCount(0);
  });

  // EXERCISE 15: Write E2E test for drag and drop
  // Test that a task can be dragged from "To Do" to "In Progress"
  test.skip('should move task via drag and drop', async ({ page }) => {
    // TODO: Implement this test
    // Hints:
    // 1. Create a task first
    // 2. Use page.dragAndDrop() or manual drag simulation
    // 3. Verify the task appears in the new column
  });

  // EXERCISE 16: Write E2E test for form validation
  // Test that submitting with empty title shows error message
  test.skip('should show validation error for empty title', async ({ page }) => {
    // TODO: Implement this test
    // Hints:
    // 1. Open the task form
    // 2. Try to submit without entering title
    // 3. Verify error message is displayed
  });

  // EXERCISE 17: Write E2E test for task count updates
  // Test that header stats update correctly when tasks are created/moved
  test.skip('should update task counts in header', async ({ page }) => {
    // TODO: Implement this test
    // Hints:
    // 1. Check initial counts (all zeros)
    // 2. Create tasks
    // 3. Verify counts update correctly
  });

  // EXERCISE 18: Write E2E test for persistence
  // Test that tasks persist after page reload (localStorage)
  test.skip('should persist tasks after page reload', async ({ page }) => {
    // TODO: Implement this test
    // Hints:
    // 1. Create a task
    // 2. Reload the page
    // 3. Verify task still exists
  });
});

